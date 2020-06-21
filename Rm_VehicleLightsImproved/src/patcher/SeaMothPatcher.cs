using Harmony;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using System;

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
    [HarmonyPatch(typeof(SeaMoth))]
    [HarmonyPatch(nameof(SeaMoth.SubConstructionComplete))]
    internal class SeaMoth_SubConstructionComplete_Patch
    {
        static void Postfix(SeaMoth __instance)
        {
            __instance.lightsParent.SetActive(false);
        }
    }
}
