using Harmony;
using UnityEngine;

namespace Rm_VehicleLightsImproved
{
    [HarmonyPatch(typeof(ToggleLights))]
    [HarmonyPatch(nameof(ToggleLights.OnPoweredChanged))]
    internal class ToggleLights_OnPoweredChanged_Patch
    {
        [HarmonyPrefix]
        static bool Prefix(ToggleLights __instance, bool powered)
        {
            return false;
        }
    }
}
