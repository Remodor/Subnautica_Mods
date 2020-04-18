using Harmony;
using System;
using UnityEngine;

namespace Rm_VehicleLightsImproved
{
    internal static class SeaMothSettings
    {
        internal static float lightEnergyConsumption = 0f;
    }
    [HarmonyPatch(typeof(SeaMoth))]
    [HarmonyPatch(nameof(SeaMoth.Start))]
    internal class SeaMoth_Start_Patch
    {
        [HarmonyPostfix]
        static void Postfix(SeaMoth __instance)
        {
            __instance.toggleLights.energyPerSecond = SeaMothSettings.lightEnergyConsumption;
            __instance.toggleLights.SetLightsActive(false);
        }
    }
    [HarmonyPatch(typeof(SeaMoth))]
    [HarmonyPatch(nameof(SeaMoth.OnDockedChanged))]
    internal class SeaMoth_OnDockedChanged_Patch
    {
        [HarmonyPostfix]
        static void Postfix(SeaMoth __instance, bool docked)
        {
            if (docked)
            {
                __instance.toggleLights.SetLightsActive(false);
            }
        }
    }
}
