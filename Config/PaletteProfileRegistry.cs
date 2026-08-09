using Celeste.Mod;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.ParticlePaletteHelper.Config;

internal static class PaletteProfileRegistry
{
    private const string ConfigVirtualPath = "config/ParticlePaletteHelper/palette";

    private static readonly Dictionary<string, PaletteProfile> ExactCharacters = new(StringComparer.Ordinal);
    private static PaletteProfile? globalCharacterProfile;

    public static void Reload()
    {
        Clear();

        int filesLoaded = 0;
        int profilesLoaded = 0;

        // Everest.Content.Mods preserves the loaded ModContent instances. Reading each
        // ModContent.Map separately allows multiple mods to ship the same virtual path.
        foreach (ModContent content in Everest.Content.Mods)
        {
            if (!TryFindPaletteAsset(content, out ModAsset? asset) || asset is null)
                continue;

            string sourceMod = content.Mod?.Name ?? content.Name ?? "<unknown mod>";
            if (!PaletteConfigParser.TryParse(asset, sourceMod, out List<PaletteProfile> profiles))
                continue;

            filesLoaded++;
            foreach (PaletteProfile profile in profiles)
            {
                profilesLoaded++;
                RegisterCharacterProfile(profile);
            }
        }

        Logger.Info(
            "ParticlePaletteHelper",
            $"Loaded {profilesLoaded} palette profile(s) from {filesLoaded} palette.yaml file(s): " +
            $"{ExactCharacters.Count} explicit character mapping(s), " +
            $"{(globalCharacterProfile is null ? 0 : 1)} global character profile."
        );

        ParticleEffects.ParticlePaletteResolver.ClearCache();
    }

    public static void Clear()
    {
        ExactCharacters.Clear();
        globalCharacterProfile = null;
        ParticleEffects.ParticlePaletteResolver.ClearCache();
    }

    public static bool TryResolveCurrentCharacterProfile(out PaletteProfile? profile)
    {
        profile = null;
        if (!CharacterResolver.TryGetCurrentCharacterId(out string? characterId))
            return false;

        if (characterId is not null && ExactCharacters.TryGetValue(characterId, out PaletteProfile? exact))
        {
            profile = exact;
            return true;
        }

        if (globalCharacterProfile is not null)
        {
            profile = globalCharacterProfile;
            return true;
        }

        return false;
    }

    private static void RegisterCharacterProfile(PaletteProfile profile)
    {
        IReadOnlyList<string>? ids = profile.CharacterIds;

        // No characters section: valid profile, but no character effects in v1.
        if (ids is null)
            return;

        // Empty list is the runtime representation of a global character profile.
        if (ids.Count == 0)
        {
            if (globalCharacterProfile is not null)
            {
                Logger.Warn(
                    "ParticlePaletteHelper",
                    $"Global character profile {profile.Describe()} overrides {globalCharacterProfile.Describe()}."
                );
            }

            globalCharacterProfile = profile;
            return;
        }

        foreach (string id in ids)
        {
            if (ExactCharacters.TryGetValue(id, out PaletteProfile? previous))
            {
                Logger.Warn(
                    "ParticlePaletteHelper",
                    $"Character ID '{id}' from {profile.Describe()} overrides {previous.Describe()}."
                );
            }

            ExactCharacters[id] = profile;
        }
    }

    private static bool TryFindPaletteAsset(ModContent content, out ModAsset? asset)
    {
        if (content.Map.TryGetValue(ConfigVirtualPath, out ModAsset? exact) && exact is not null)
        {
            asset = exact;
            return true;
        }

        // Keep the documented path canonical, but tolerate path casing differences.
        foreach (KeyValuePair<string, ModAsset> pair in content.Map)
        {
            if (string.Equals(pair.Key, ConfigVirtualPath, StringComparison.OrdinalIgnoreCase) && pair.Value is not null)
            {
                asset = pair.Value;
                return true;
            }
        }

        asset = null;
        return false;
    }
}
