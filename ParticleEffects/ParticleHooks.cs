using Celeste;
using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.ParticlePaletteHelper.Config;

namespace Celeste.Mod.ParticlePaletteHelper.ParticleEffects;

internal static class ParticleHooks
{
    private static bool loaded;

    public static void Load()
    {
        if (loaded)
            return;
        loaded = true;

        On.Monocle.ParticleSystem.Emit_ParticleType_Vector2 += Emit;
        On.Monocle.ParticleSystem.Emit_ParticleType_Vector2_float += Emit;
        On.Monocle.ParticleSystem.Emit_ParticleType_Vector2_Color += Emit;
        On.Monocle.ParticleSystem.Emit_ParticleType_Vector2_Color_float += Emit;
        On.Monocle.ParticleSystem.Emit_ParticleType_Entity_int_Vector2_Vector2_float += Emit;

        // BadelineBoost's moving afterimage is a TrailManager trail, not a ParticleType.
        On.Celeste.TrailManager.Add_Entity_Color_float_bool_bool += AddBadelineBoostTrail;
    }

    public static void Unload()
    {
        if (!loaded)
            return;
        loaded = false;

        On.Monocle.ParticleSystem.Emit_ParticleType_Vector2 -= Emit;
        On.Monocle.ParticleSystem.Emit_ParticleType_Vector2_float -= Emit;
        On.Monocle.ParticleSystem.Emit_ParticleType_Vector2_Color -= Emit;
        On.Monocle.ParticleSystem.Emit_ParticleType_Vector2_Color_float -= Emit;
        On.Monocle.ParticleSystem.Emit_ParticleType_Entity_int_Vector2_Vector2_float -= Emit;

        On.Celeste.TrailManager.Add_Entity_Color_float_bool_bool -= AddBadelineBoostTrail;
        ParticlePaletteResolver.ClearCache();
    }

    private static void Emit(
        On.Monocle.ParticleSystem.orig_Emit_ParticleType_Vector2 orig,
        ParticleSystem self,
        ParticleType type,
        Vector2 position
    ) => orig(self, ParticlePaletteResolver.Resolve(type), position);

    private static void Emit(
        On.Monocle.ParticleSystem.orig_Emit_ParticleType_Vector2_float orig,
        ParticleSystem self,
        ParticleType type,
        Vector2 position,
        float direction
    ) => orig(self, ParticlePaletteResolver.Resolve(type), position, direction);

    private static void Emit(
        On.Monocle.ParticleSystem.orig_Emit_ParticleType_Vector2_Color orig,
        ParticleSystem self,
        ParticleType type,
        Vector2 position,
        Color color
    ) => orig(self, ParticlePaletteResolver.Resolve(type), position, color);

    private static void Emit(
        On.Monocle.ParticleSystem.orig_Emit_ParticleType_Vector2_Color_float orig,
        ParticleSystem self,
        ParticleType type,
        Vector2 position,
        Color color,
        float direction
    ) => orig(self, ParticlePaletteResolver.Resolve(type), position, color, direction);

    private static void Emit(
        On.Monocle.ParticleSystem.orig_Emit_ParticleType_Entity_int_Vector2_Vector2_float orig,
        ParticleSystem self,
        ParticleType type,
        Entity track,
        int amount,
        Vector2 position,
        Vector2 positionRange,
        float direction
    ) => orig(self, ParticlePaletteResolver.Resolve(type), track, amount, position, positionRange, direction);

    private static void AddBadelineBoostTrail(
        On.Celeste.TrailManager.orig_Add_Entity_Color_float_bool_bool orig,
        Entity entity,
        Color color,
        float duration,
        bool frozenUpdate,
        bool useRawDeltaTime
    )
    {
        if (entity is BadelineBoost && ParticlePaletteResolver.TryResolveTrailColor(PaletteRole.Another, 1, out Color replacement))
            color = replacement;

        orig(entity, color, duration, frozenUpdate, useRawDeltaTime);
    }
}
