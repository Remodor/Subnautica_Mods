using Harmony;
using UnityEngine;


namespace Rm_PowerModifier
{
    [HarmonyPatch(typeof(SolarPanel))]
    [HarmonyPatch(nameof(SolarPanel.Update))]
    internal class SolarPanel_PowerModifier_Patch
    {
        private static float powerLevel;
        private static float powerModifier = 1.0f;
        internal static void SetPowerModifier(float modifier)
        {
            powerModifier = modifier;
        }

        [HarmonyPrefix]
        internal static void Prefix(SolarPanel __instance)
        {

            powerLevel = __instance.powerSource.power;
        }
        [HarmonyPostfix]
        internal static void Postfix(SolarPanel __instance)
        {
            float powerDelta = __instance.powerSource.power - powerLevel;
            __instance.powerSource.SetPower(powerLevel + powerDelta * powerModifier);
        }
    }
    [HarmonyPatch(typeof(SolarPanel))]
    [HarmonyPatch(nameof(SolarPanel.Awake))]
    internal class SolarPanel_MaxPower_Patch
    {
        private static float maxPower = 75f;
        internal static void SetMaxPower(float power)
        {
            maxPower = power;
        }

        [HarmonyPostfix]
        internal static void Postfix(SolarPanel __instance)
        {
            __instance.powerSource.maxPower = Mathf.Max(maxPower, 0);
        }
    }
}
