using Celeste.Mod;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.ParticlePaletteHelper.Config;

internal static class PaletteConfigParser
{
    public static bool TryParse(ModAsset asset, string sourceMod, out List<PaletteProfile> profiles)
    {
        profiles = new List<PaletteProfile>();

        if (!asset.TryDeserialize<Dictionary<object, object>>(out Dictionary<object, object>? root) || root is null)
        {
            Logger.Error("ParticlePaletteHelper", $"Failed to deserialize palette config from mod '{sourceMod}'.");
            return false;
        }

        if (!TryGet(root, "Palette", out object? paletteNode))
        {
            Logger.Error("ParticlePaletteHelper", $"Palette config from mod '{sourceMod}' is missing root key 'Palette'.");
            return false;
        }

        if (paletteNode is not IList<object> entries)
        {
            Logger.Error("ParticlePaletteHelper", $"'Palette' in mod '{sourceMod}' must be a YAML sequence.");
            return false;
        }

        for (int i = 0; i < entries.Count; i++)
        {
            int profileNumber = i + 1;
            if (!TryMapping(entries[i], out IDictionary<object, object>? entryMap) ||
                !TryGet(entryMap, "profile", out object? profileNode) ||
                !TryMapping(profileNode, out IDictionary<object, object>? profileMap))
            {
                Logger.Error("ParticlePaletteHelper", $"{sourceMod} profile #{profileNumber} must contain a mapping named 'profile'. Skipping it.");
                continue;
            }

            if (!TryParseCharacterScope(profileMap, sourceMod, profileNumber, out IReadOnlyList<string>? characterIds, out bool scopeValid))
            {
                if (!scopeValid)
                    continue;
            }

            Palette5? self = ParseOptionalPalette(profileMap, "Self", sourceMod, profileNumber);
            Palette5? another = ParseOptionalPalette(profileMap, "Another", sourceMod, profileNumber);
            Palette5? levelUp = ParseOptionalPalette(profileMap, "LevelUp", sourceMod, profileNumber);

            profiles.Add(new PaletteProfile(sourceMod, profileNumber, characterIds, self, another, levelUp));
        }

        return true;
    }

    private static bool TryParseCharacterScope(
        IDictionary<object, object> profileMap,
        string sourceMod,
        int profileNumber,
        out IReadOnlyList<string>? characterIds,
        out bool valid
    )
    {
        valid = true;
        characterIds = null;

        // Missing characters = no character particle configuration for this profile.
        if (!TryGet(profileMap, "characters", out object? charactersNode))
            return true;

        // Explicit `characters:` with no value, or `characters: {}`, means global.
        if (charactersNode is null)
        {
            characterIds = Array.Empty<string>();
            return true;
        }

        if (!TryMapping(charactersNode, out IDictionary<object, object>? charactersMap))
        {
            Logger.Error("ParticlePaletteHelper", $"{sourceMod} profile #{profileNumber}: 'characters' must be a mapping. Skipping profile.");
            valid = false;
            return false;
        }

        // characters exists but character_id is missing/null => global.
        if (!TryGet(charactersMap, "character_id", out object? idsNode) || idsNode is null)
        {
            characterIds = Array.Empty<string>();
            return true;
        }

        if (idsNode is not IList<object> ids || ids.Count == 0)
        {
            Logger.Error("ParticlePaletteHelper", $"{sourceMod} profile #{profileNumber}: 'character_id' must be a non-empty YAML sequence. Skipping profile.");
            valid = false;
            return false;
        }

        List<string> parsedIds = new(ids.Count);
        HashSet<string> seen = new(StringComparer.Ordinal);

        foreach (object? idNode in ids)
        {
            string? id = idNode as string ?? idNode?.ToString();
            id = id?.Trim();
            if (string.IsNullOrEmpty(id))
            {
                Logger.Error("ParticlePaletteHelper", $"{sourceMod} profile #{profileNumber}: character_id contains an empty value. Skipping profile.");
                valid = false;
                return false;
            }

            if (seen.Add(id))
                parsedIds.Add(id);
        }

        characterIds = parsedIds;
        return true;
    }

    private static Palette5? ParseOptionalPalette(
        IDictionary<object, object> profileMap,
        string key,
        string sourceMod,
        int profileNumber
    )
    {
        // Missing role = vanilla for that role.
        if (!TryGet(profileMap, key, out object? node))
            return null;

        if (node is not IList<object> items || items.Count != 5)
        {
            Logger.Error("ParticlePaletteHelper", $"{sourceMod} profile #{profileNumber}: '{key}' must contain exactly 5 colors. This role will stay vanilla.");
            return null;
        }

        Color[] parsed = new Color[5];
        for (int i = 0; i < items.Count; i++)
        {
            string? text = items[i] as string ?? items[i]?.ToString();
            if (!TryParseColor(text, out parsed[i]))
            {
                Logger.Error("ParticlePaletteHelper", $"{sourceMod} profile #{profileNumber}: '{key}' color #{i + 1} must use #RRGGBB. This role will stay vanilla.");
                return null;
            }
        }

        return new Palette5(parsed[0], parsed[1], parsed[2], parsed[3], parsed[4]);
    }

    private static bool TryParseColor(string? text, out Color color)
    {
        color = default;
        if (text is null || text.Length != 7 || text[0] != '#')
            return false;

        if (!uint.TryParse(text.AsSpan(1), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out uint rgb))
            return false;

        color = new Color(
            (byte)((rgb >> 16) & 0xFF),
            (byte)((rgb >> 8) & 0xFF),
            (byte)(rgb & 0xFF)
        );
        return true;
    }

    private static bool TryGet(IDictionary<object, object> map, string key, out object? value)
    {
        foreach (KeyValuePair<object, object> pair in map)
        {
            if (pair.Key is string text && string.Equals(text, key, StringComparison.Ordinal))
            {
                value = pair.Value;
                return true;
            }
        }

        value = null;
        return false;
    }

    private static bool TryMapping(object? node, [NotNullWhen(true)] out IDictionary<object, object>? map)
    {
        map = node as IDictionary<object, object>;
        return map is not null;
    }
}
