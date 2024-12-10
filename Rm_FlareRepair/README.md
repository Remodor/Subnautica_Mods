# Flare Repair

## Changes

### Fixes

- Energy consumption: In the base game flares had a total amount of energy, which only depleted when they were visible (holding in hand or within 60m distance to them). Therefore, many people still believe flares last forever, but they are only super buggy.
  - > Flares now have an ignition Time (savegame consistent). Based on the time they were lit, they will deplete after a set duration no matter where they are. As you would expect it: when you light a flare and put it in a locker or your pocket, it is still depleting!
- Fixed visual bugs: There is a number of visual bugs related to flares which are now fixed, e.g.:
  - Flare flickering is not affected by game speed anymore (very annoying if you set a low game speed, like 0.2).
  - Flare cap fix: After reloading, used flares magically had there flare cap reappearing to the flare and just burning through the cap. This is now fixed, the cap will not reappear on used flares after reloading.
  - Flare state is now properly restored when loading, holstering, and throwing.
- Fixed Sound bug: The flare was missing a sound when thrown, dropped from the inventory or loaded into the game. This is fixed - the flare is now always emitting a sound when lit.

### Additions

- Added an option to `Keep Depleted Flares`: In the vanilla game, flares were destroyed when depleted. With this option they will persist, even after saving and loading. You can either leave a flare permanently somewhere, e.g. to mark a location, or you need to destroy it with the trash can. Disabling this option will remove all kept flares as long as they are currently loaded.
- Added an option to set the total duration of flares in relation to ingame days. The default duration is set to 0.5, which corresponds to half a day.
- Added an option to modify the flare intensity. The default intensity is 1.
- Added the ability to keep holding the flare during the ignition. When holding the "use-button" (right mouse button) during ignition, the player will not instantly throw the flare away but keep it in his hand.
- Added progressive dimming: Flares can now change their intensity depending on the remaining time. Two settings were added:
  - `Progressive Dimming Threshold`: Setting this value >0 will progressively dim the flare intensity starting at the set threshold and used 'Intensity Modifier'. For example, a value of 0.5 will start dimming the flare when 50% of the time is left.
  - `Minimum Relative Factor`: This value sets the reached minimum intensity when the flare is depleted and progressive dimming is used. For example, a value of 0.25 will result in a flare intensity of 25% of the 'Intensity Modifier' at the end of the flare's lifetime.
