using System.Collections.Generic;
using Celeste;
using Monocle;
using Celeste.Mod.ParticlePaletteHelper.Config;

namespace Celeste.Mod.ParticlePaletteHelper.ParticleEffects;

internal static class ParticlePaletteResolver
{
    private readonly record struct CacheKey(ParticleType Source, PaletteProfile Profile);

    private static readonly Dictionary<CacheKey, ParticleType> Cache = new();

    public static void ClearCache() => Cache.Clear();

    public static ParticleType Resolve(ParticleType source)
    {
        // Reject non-target particle types before doing any Player lookup.
        if (!TryGetMapping(source, out PaletteRole role, out int colorIndex, out int color2Index))
            return source;

        if (!PaletteProfileRegistry.TryResolveCurrentCharacterProfile(out PaletteProfile? profile) || profile is null)
            return source;

        // A missing role means "use default game config" for exactly that role.
        Palette5? palette = profile.GetPalette(role);
        if (palette is null)
            return source;

        CacheKey key = new(source, profile);
        if (Cache.TryGetValue(key, out ParticleType? cached))
            return cached;

        ParticleType clone = new(source)
        {
            Color = palette[colorIndex],
            Color2 = palette[color2Index],
        };

        Cache[key] = clone;
        return clone;
    }

    public static bool TryResolveTrailColor(PaletteRole role, int colorIndex, out Microsoft.Xna.Framework.Color color)
    {
        color = default;

        if (!PaletteProfileRegistry.TryResolveCurrentCharacterProfile(out PaletteProfile? profile) || profile is null)
            return false;

        Palette5? palette = profile.GetPalette(role);
        if (palette is null)
            return false;

        color = palette[colorIndex];
        return true;
    }

    private static bool TryGetMapping(
        ParticleType source,
        out PaletteRole role,
        out int colorIndex,
        out int color2Index
    )
    {
        // refill -> Self
        // Vanilla luminance: 6DE081 < 85FC87 < A5FFF7 < D3FFD4
        if (object.ReferenceEquals(source, Refill.P_Shatter))
            return Map(PaletteRole.Self, 5, 3, out role, out colorIndex, out color2Index);
        if (object.ReferenceEquals(source, Refill.P_Glow) || object.ReferenceEquals(source, Refill.P_Regen))
            return Map(PaletteRole.Self, 4, 2, out role, out colorIndex, out color2Index);

        // refillTwo -> LevelUp
        // Vanilla luminance: DD6CCA < EF94E3 < FFA5AA < FFD3F9
        if (object.ReferenceEquals(source, Refill.P_ShatterTwo))
            return Map(PaletteRole.LevelUp, 5, 3, out role, out colorIndex, out color2Index);
        if (object.ReferenceEquals(source, Refill.P_GlowTwo) || object.ReferenceEquals(source, Refill.P_RegenTwo))
            return Map(PaletteRole.LevelUp, 4, 2, out role, out colorIndex, out color2Index);

        // launch <-> spin split transition -> Another
        if (object.ReferenceEquals(source, Player.P_Split))
            return Map(PaletteRole.Another, 4, 3, out role, out colorIndex, out color2Index);

        // badelineBoost + Summit spin -> Another
        if (object.ReferenceEquals(source, BadelineBoost.P_Move))
            return Map(PaletteRole.Another, 5, 3, out role, out colorIndex, out color2Index);
        if (object.ReferenceEquals(source, BadelineBoost.P_Ambience))
            return Map(PaletteRole.Another, 2, 4, out role, out colorIndex, out color2Index);

        // launchRecover -> Self
        if (object.ReferenceEquals(source, Player.P_SummitLandA) || object.ReferenceEquals(source, Player.P_SummitLandB))
            return Map(PaletteRole.Self, 4, 4, out role, out colorIndex, out color2Index);
        if (object.ReferenceEquals(source, Player.P_SummitLandC))
            return Map(PaletteRole.Self, 2, 3, out role, out colorIndex, out color2Index);

        role = default;
        colorIndex = default;
        color2Index = default;
        return false;
    }

    private static bool Map(
        PaletteRole r,
        int c1,
        int c2,
        out PaletteRole role,
        out int colorIndex,
        out int color2Index
    )
    {
        role = r;
        colorIndex = c1;
        color2Index = c2;
        return true;
    }
}
