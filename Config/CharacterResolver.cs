using System.Reflection;
using Celeste;
using Monocle;

namespace Celeste.Mod.ParticlePaletteHelper.Config;

internal static class CharacterResolver
{
    private static readonly FieldInfo? SpriteNameField = typeof(PlayerSprite).GetField(
        "spriteName",
        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
    );

    // Returns false only when there is no active Player to associate with character effects.
    // A null ID still permits a global character profile.
    public static bool TryGetCurrentCharacterId(out string? characterId)
    {
        characterId = null;

        if (Engine.Scene?.Tracker?.GetEntity<Player>() is not Player player || player.Sprite is null)
            return false;

        characterId = SpriteNameField?.GetValue(player.Sprite) as string;
        return true;
    }
}
