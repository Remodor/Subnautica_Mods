using Harmony;
using UnityEngine;

namespace Rm_VehicleLightsImproved
{
    internal static class SubRootSettings
    {
        internal static float defaultLightEnergyConsumption = 0f;
        internal static float emergencyLightEnergyConsumption = 0f;

        internal static bool autoLightDim = true;
    }
    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch(nameof(SubRoot.Start))]
    internal class SubRoot_Start_Patch
    {
        static void Postfix(SubRoot __instance)
        {
            __instance.silentRunningPowerCost = 0;
            __instance.lightControl.fadeDuration = 1.5f;
        }
    }
    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch(nameof(SubRoot.UpdateLighting))]
    internal class SubRoot_UpdateLighting_Patch
    {
        static void Prefix(SubRoot __instance, out bool __state)
        {
            __state = __instance.silentRunning;
            if ((SubRootSettings.autoLightDim && Player.main.currentSub != __instance) || __instance.powerRelay.GetPowerStatus() == PowerSystem.Status.Emergency)
            {
                __instance.silentRunning = true;
            }
        }
        static void Postfix(SubRoot __instance, bool __state)
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
