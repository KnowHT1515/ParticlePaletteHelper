# Fixed particle palette mapping

Palette indices are 1..5 from darkest/design-low to brightest/design-high as supplied by the skin config.
When an effect uses fewer than five distinct colors, the design priority is `4 -> 3 -> 2 -> 5 -> 1`, while assignment preserves vanilla relative luminance.

- 1 color: `{4}`
- 2 colors: `{3,4}`
- 3 colors: `{2,3,4}`
- 4 colors: `{2,3,4,5}`
- 5 colors: `{1,2,3,4,5}`

## Refill -> Self

Vanilla luminance order: `#6DE081 < #85FC87 < #A5FFF7 < #D3FFD4`.

| ParticleType | Color | Color2 |
|---|---:|---:|
| `Refill.P_Shatter` | 5 | 3 |
| `Refill.P_Glow` | 4 | 2 |
| `Refill.P_Regen` | 4 | 2 |

## RefillTwo -> LevelUp

Vanilla luminance order: `#DD6CCA < #EF94E3 < #FFA5AA < #FFD3F9`.

| ParticleType | Color | Color2 |
|---|---:|---:|
| `Refill.P_ShatterTwo` | 5 | 3 |
| `Refill.P_GlowTwo` | 4 | 2 |
| `Refill.P_RegenTwo` | 4 | 2 |

## BadelineBoost + Summit spin -> Another

Five vanilla visual colors, darkest to brightest:
`trail #FF6DEF < ambience #F78AE7 < move #E0A8D8 < ambience #FFCCF7 < move #FFFFFF`.

| Source | Index |
|---|---:|
| moving `TrailManager` afterimage | 1 |
| `BadelineBoost.P_Ambience.Color` | 2 |
| `BadelineBoost.P_Move.Color2` | 3 |
| `BadelineBoost.P_Ambience.Color2` | 4 |
| `BadelineBoost.P_Move.Color` | 5 |

## launch <-> spin split/merge -> Another

| ParticleType | Color | Color2 |
|---|---:|---:|
| `Player.P_Split` | 4 | 3 |

## launchRecover -> Self

| ParticleType | Color | Color2 |
|---|---:|---:|
| `Player.P_SummitLandA` | 4 | 4 |
| `Player.P_SummitLandB` | 4 | 4 |
| `Player.P_SummitLandC` | 2 | 3 |
