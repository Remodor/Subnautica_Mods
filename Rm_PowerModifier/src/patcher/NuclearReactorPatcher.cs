using Harmony;
using UnityEngine;


namespace Rm_PowerModifier
{
    [HarmonyPatch(typeof(BaseNuclearReactor))]
    [HarmonyPatch(nameof(BaseNuclearReactor.Update))]
    internal class NuclearReactor_PowerModifier_Patch
    {
        private static float powerLevel;
        private static float powerModifier = 1.0f;

        internal static void SetPowerModifier(float modifier)
        {
            powerModifier = modifier;
        }

        [HarmonyPrefix]
        internal static void Prefix(BaseNuclearReactor __instance)
        {
            powerLevel = __instance._powerSource.power;
        }
        [HarmonyPostfix]
        internal static void Postfix(BaseNuclearReactor __instance)
        {
            float powerDelta = __instance._powerSource.power - powerLevel;
            __instance._powerSource.SetPower(powerLevel + powerDelta * powerModifier);
        }
    }
    [HarmonyPatch(typeof(BaseNuclearReactor))]
    [HarmonyPatch(nameof(BaseNuclearReactor.Start))]
    internal class NuclearReactor_MaxPower_Patch
    {
        private static float maxPower = 2500f;
        internal static void SetMaxPower(float power)
        {
            maxPower = power;
        }

        [HarmonyPostfix]
        internal static void Postfix(BaseNuclearReactor __instance)
        {
            __instance._powerSource.maxPower = Mathf.Max(maxPower, 0);
        }
    }
}