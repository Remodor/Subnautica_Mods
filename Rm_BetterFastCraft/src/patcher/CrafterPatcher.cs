using Harmony;
using UnityEngine;

namespace Rm_BetterFastCraft
{
    internal static class CraftingSettings
    {
        internal static float craftingSpeed = 1f;
        internal static float minimumDuration = 2.7f;
        internal static CraftConfig.ApplyMode mode = CraftConfig.ApplyMode.Relative;

    }
    [HarmonyPatch(typeof(Crafter))]
    [HarmonyPatch(nameof(Crafter.Craft))]
    internal class Crafter_Craft_Patch
    {
        static void Prefix(ref float duration)
        {
            if (CraftingSettings.mode == CraftConfig.ApplyMode.Relative)
            {
                duration *= CraftingSettings.craftingSpeed;
            }
            else
            {
                duration = CraftingSettings.craftingSpeed;
            }
            duration = Mathf.Max(duration, CraftingSettings.minimumDuration, 0.1f);
        }
    }
}
