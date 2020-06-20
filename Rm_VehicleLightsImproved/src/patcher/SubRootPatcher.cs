using Harmony;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Rm_VehicleLightsImproved
{
    internal static class SubRootSettings
    {
        internal static float defaultLightEnergyConsumption = 0f;
        internal static float emergencyLightEnergyConsumption = 0f;

        internal static bool autoLightDim = true;
        internal static bool includeBaseLights = true;

        internal static float lightFadeDuration = 1.5f;
    }
    [HarmonyPatch(typeof(LightingController))]
    [HarmonyPatch(nameof(LightingController.LerpToState))]
    [HarmonyPatch(new Type[] { typeof(int) })]
    internal class LightingController_LerpToState_Patch
    {
        static void Prefix(LightingController __instance)
        {
            __instance.fadeDuration = SubRootSettings.lightFadeDuration;
        }
    }
    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch(nameof(SubRoot.UpdateLighting))]
    internal class SubRoot_UpdateLighting_Patch
    {
        static void Prefix(SubRoot __instance, out bool __state)
        {
            __state = __instance.silentRunning;
            if (__instance.isCyclops || SubRootSettings.includeBaseLights)
            {
                if ((SubRootSettings.autoLightDim && Player.main.currentSub != __instance) || __instance.powerRelay.GetPowerStatus() == PowerSystem.Status.Emergency)
                {
                    __instance.silentRunning = true;
                }
            }
        }
        static void Postfix(SubRoot __instance, bool __state)
        {
            if (__instance.isCyclops || SubRootSettings.includeBaseLights)
            {
                __instance.silentRunning = __state;
                if (__instance.lightingState == 1)
                {
                    float energyCost = DayNightCycle.main.deltaTime * SubRootSettings.emergencyLightEnergyConsumption;
                    __instance.powerRelay.ConsumeEnergy(energyCost, out _);
                }
                else if (__instance.lightingState == 0)
                {
                    float energyCost = DayNightCycle.main.deltaTime * SubRootSettings.defaultLightEnergyConsumption;
                    __instance.powerRelay.ConsumeEnergy(energyCost, out _);
                }
            }
        }
    }
}
