using System.Collections.Generic;

namespace Celeste.Mod.ParticlePaletteHelper.Config;

internal sealed class PaletteProfile
{
    public string SourceMod { get; }
    public int SourceProfileIndex { get; }

    // null = this profile has no character section and currently does not
    // participate in character particle replacement.
    // empty list = global character profile.
    public IReadOnlyList<string>? CharacterIds { get; }

    public Palette5? Self { get; }
    public Palette5? Another { get; }
    public Palette5? LevelUp { get; }

    public PaletteProfile(
        string sourceMod,
        int sourceProfileIndex,
        IReadOnlyList<string>? characterIds,
        Palette5? self,
        Palette5? another,
        Palette5? levelUp
    )
    {
        SourceMod = sourceMod;
        SourceProfileIndex = sourceProfileIndex;
        CharacterIds = characterIds;
        Self = self;
        Another = another;
        LevelUp = levelUp;
    }

    public Palette5? GetPalette(PaletteRole role) => role switch
    {
        PaletteRole.Self => Self,
        PaletteRole.Another => Another,
        PaletteRole.LevelUp => LevelUp,
        _ => null,
    };

    public string Describe() => $"{SourceMod} profile #{SourceProfileIndex}";
}
