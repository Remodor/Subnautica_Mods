using Harmony;
using UnityEngine;

namespace Rm_PowerModifier
{
    [HarmonyPatch(typeof(BaseBioReactor))]
    [HarmonyPatch(nameof(BaseBioReactor.Update))]
    internal class BioReactor_PowerModifier_Patch
    {
        private static float powerLevel;
        private static float powerModifier = 1.0f;

        internal static void SetPowerModifier(float modifier)
        {
            powerModifier = modifier;
        }

        [HarmonyPrefix]
        internal static void Prefix(BaseBioReactor __instance)
        {

            powerLevel = __instance._powerSource.power;
        }
        [HarmonyPostfix]
        internal static void Postfix(BaseBioReactor __instance)
        {
            float powerDelta = __instance._powerSource.power - powerLevel;
            __instance._powerSource.SetPower(powerLevel + powerDelta * powerModifier);
        }
    }
    [HarmonyPatch(typeof(BaseBioReactor))]
    [HarmonyPatch(nameof(BaseBioReactor.Start))]
    internal class BioReactor_MaxPower_Patch
    {
        private static float maxPower = 500f;
        internal static void SetMaxPower(float power)
        {
            maxPower = power;
        }

        [HarmonyPostfix]
        internal static void Postfix(BaseBioReactor __instance)
        {
            __instance._powerSource.maxPower = Mathf.Max(maxPower, 0);
        }
    }
}