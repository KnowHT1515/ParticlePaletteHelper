# ParticlePaletteHelper 1.0.0

A small Everest CodeMod that recolors selected vanilla Celeste particle effects from per-skin YAML profiles.
It does not mutate vanilla static `ParticleType` objects; recolored clones are cached and substituted at emission time.

## Supported character particle roles

- `Self`
  - `Refill.P_Shatter`, `P_Glow`, `P_Regen`
  - `Player.P_SummitLandA/B/C` (`launchRecover` landing particles)
- `Another`
  - `BadelineBoost.P_Ambience`
  - `BadelineBoost.P_Move` (also used by Summit spin)
  - BadelineBoost moving `TrailManager` afterimages
  - `Player.P_Split` (launch <-> spin split/merge transition particles)
- `LevelUp`
  - `Refill.P_ShatterTwo`, `P_GlowTwo`, `P_RegenTwo`

The fixed luminance-to-palette mapping is documented in `MAPPING.md`.

## Skin-side configuration

A mod that wants to use ParticlePaletteHelper creates:

`config/ParticlePaletteHelper/palette.yaml`

Example:

```yaml
Palette:
  - profile:
      characters:
        character_id:
          - player_name
          - player_name_no_backpack
          - player_name_playback

      Self:
        - "#112233"
        - "#223344"
        - "#334455"
        - "#445566"
        - "#556677"

      Another:
        - "#112233"
        - "#223344"
        - "#334455"
        - "#445566"
        - "#556677"

      LevelUp:
        - "#112233"
        - "#223344"
        - "#334455"
        - "#445566"
        - "#556677"
```

### Optional fields

- Missing `characters`: this profile does not modify character particle effects. This reserves the profile for future non-character extensions.
- `characters` present with missing `character_id`: global character profile. Prefer the explicit form `characters: {}`.
- Missing `Self`, `Another`, or `LevelUp`: only that role stays vanilla.
- A present palette role must contain exactly five `#RRGGBB` colors.

### Profile selection and conflicts

1. Exact `character_id` match.
2. Global character profile.
3. Vanilla.

Once a profile is selected, missing palette roles do **not** inherit from the global profile; they stay vanilla.

If the same exact character ID is registered more than once, the later-loaded definition wins and a warning is logged.
If more than one global character profile is registered, the later-loaded one wins and a warning is logged.

`character_id: []` is invalid and the profile is skipped.

## Multiple mods

Every mod may ship the same virtual config path. ParticlePaletteHelper walks each loaded `ModContent.Map` separately, so configs from different mods are read independently rather than being lost to the global content-path override.

## Building

Requires the .NET 8 SDK and an active modern Everest Celeste installation.

```powershell
.\build.ps1 -CelesteDir "D:\SteamLibrary\steamapps\common\Celeste"
```

Copy the produced `ParticlePaletteHelper.dll` (and during testing, `.pdb`) into this mod's `Code/` directory.

## Depending on the helper

Add to the skin mod's `everest.yaml`:

```yaml
Dependencies:
  - Name: ParticlePaletteHelper
    Version: 1.0.0
```

The skin mod itself does not need to ship a code DLL just for these particle palettes.
