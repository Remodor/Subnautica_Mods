using Harmony;
using System;
using UnityEngine;

namespace Rm_VehicleLightsImproved
{
    internal static class CyclopsSettings
    {
        internal static float internalLightEnergyConsumption = 0f;
        internal static float emergencyLightEnergyConsumption = 0f;
        internal static float externalLightEnergyConsumption = 0f;
    }
    [HarmonyPatch(typeof(CyclopsLightingPanel))]
    [HarmonyPatch(nameof(CyclopsLightingPanel.Start))]
    internal class CyclopsLightingPanel_Start_Patch
    {
        static bool Prefix(CyclopsLightingPanel __instance)
        {
            __instance.lightingOn = false;
            return true;
        }
    }
    [HarmonyPatch(typeof(CyclopsLightingPanel))]
    [HarmonyPatch(nameof(CyclopsLightingPanel.Update))]
    internal class CyclopsLightingPanel_Update_Patch
    {
        static void Postfix(CyclopsLightingPanel __instance)
        {
            if (__instance.prevPowerRelayState && __instance.floodlightsOn)
            {
                float energyCost = DayNightCycle.main.deltaTime * CyclopsSettings.externalLightEnergyConsumption;
                float consumedEnergy; 
                __instance.cyclopsRoot.powerRelay.ConsumeEnergy(energyCost, out consumedEnergy);
            }
        }
    }
    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch(nameof(SubRoot.UpdateLighting))]
    internal class SubRoot_UpdateLighting_Patch
    {
        private static bool actualSilentRunning;
        static void Prefix(SubRoot __instance)
        {
            actualSilentRunning = __instance.silentRunning;
            if (__instance.powerRelay.GetPowerStatus() == PowerSystem.Status.Emergency)
            {
                __instance.silentRunning = true;
            }
        }
        static void Postfix(SubRoot __instance)
        {
            if (__instance.lightingState == 0)
            {
                float energyCost = DayNightCycle.main.deltaTime * CyclopsSettings.internalLightEnergyConsumption;
                float consumedEnergy;
                __instance.powerRelay.ConsumeEnergy(energyCost, out consumedEnergy);
            }
            if (__instance.lightingState == 1)
            {
                float energyCost = DayNightCycle.main.deltaTime * CyclopsSettings.emergencyLightEnergyConsumption;
                float consumedEnergy;
                __instance.powerRelay.ConsumeEnergy(energyCost, out consumedEnergy);
            }
            __instance.silentRunning = actualSilentRunning;
        }
    }
}
