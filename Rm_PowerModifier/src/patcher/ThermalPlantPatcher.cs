using Harmony;
using UnityEngine;

namespace Rm_PowerModifier
{
    [HarmonyPatch(typeof(ThermalPlant))]
    [HarmonyPatch(nameof(ThermalPlant.AddPower))]
    internal class ThermalPlant_PowerModifier_Patch
    {
        private static float powerLevel;
        private static float powerModifier = 1.0f;

        internal static void SetPowerModifier(float modifier)
        {
            powerModifier = modifier;
        }

        [HarmonyPrefix]
        internal static void Prefix(ThermalPlant __instance)
        {
            powerLevel = __instance.powerSource.power;
        }
        [HarmonyPostfix]
        internal static void Postfix(ThermalPlant __instance)
        {
            float powerDelta = __instance.powerSource.power - powerLevel;
            __instance.powerSource.SetPower(powerLevel + powerDelta * powerModifier);
        }
    }
    [HarmonyPatch(typeof(ThermalPlant))]
    [HarmonyPatch(nameof(ThermalPlant.Start))]
    internal class ThermalPlant_MaxPower_Patch
    {
        private static float maxPower = 250f;
        internal static void SetMaxPower(float power)
        {
            maxPower = power;
        }

        [HarmonyPostfix]
        internal static void Postfix(ThermalPlant __instance)
        {
            __instance.powerSource.maxPower = Mathf.Max(maxPower, 0);
        }
    }
}
