using Microsoft.Xna.Framework;

namespace Celeste.Mod.ParticlePaletteHelper.Config;

internal sealed class Palette5
{
    // Index 0 is unused so config numbering remains 1..5.
    private readonly Color[] colors = new Color[6];

    public Palette5(Color c1, Color c2, Color c3, Color c4, Color c5)
    {
        colors[1] = c1;
        colors[2] = c2;
        colors[3] = c3;
        colors[4] = c4;
        colors[5] = c5;
    }

    public Color this[int index] => colors[index];
}
