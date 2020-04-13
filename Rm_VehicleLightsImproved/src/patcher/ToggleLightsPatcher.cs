using Harmony;
using UnityEngine;
using System;

namespace Rm_VehicleLightsImproved
{
    [HarmonyPatch(typeof(ToggleLights))]
    [HarmonyPatch(nameof(ToggleLights.OnPoweredChanged))]
    internal class ToggleLights_OnPoweredChanged_Patch
    {
        [HarmonyPrefix]
        internal static bool Prefix(ToggleLights __instance, bool powered)
        {
            return false; //Complete override!
        }
    }
}
