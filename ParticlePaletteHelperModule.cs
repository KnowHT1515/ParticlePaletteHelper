using Celeste.Mod;
using Celeste.Mod.ParticlePaletteHelper.Config;
using Celeste.Mod.ParticlePaletteHelper.ParticleEffects;

namespace Celeste.Mod.ParticlePaletteHelper;

public sealed class ParticlePaletteHelperModule : EverestModule
{
    public static ParticlePaletteHelperModule Instance { get; private set; } = null!;

    public ParticlePaletteHelperModule()
    {
        Instance = this;
    }

    public override void Load()
    {
        // Install hooks as soon as the code module is loaded.
        // Do not scan palette configs here: ParticlePaletteHelper is commonly a
        // dependency of skin mods, so dependent mods may not have had their content
        // crawled yet when this Load() executes.
        ParticleHooks.Load();
    }

    public override void LoadContent(bool firstLoad)
    {
        // At this point Everest has finished loading/crawling mod content, so palette
        // configs provided by mods which depend on ParticlePaletteHelper are visible.
        // Reloading here also makes asset/content reloads refresh the registry.
        PaletteProfileRegistry.Reload();
    }

    public override void Unload()
    {
        ParticleHooks.Unload();
        PaletteProfileRegistry.Clear();
    }
}
