using Harmony;

namespace Rm_PowerModifier
{
    [HarmonyPatch(typeof(BaseBioReactor))]
    [HarmonyPatch("Update")]
    internal class BioReactor_Update_Patch
    {
        private static float powerLevel;
        private static float powerModifier;

        public static void SetPowerModifier(float modifier)
        {
            powerModifier = modifier;
        }

        [HarmonyPrefix]
        public static bool Prefix(BaseBioReactor __instance)
        {
            powerLevel = __instance._powerSource.power;
            return true;
        }
        [HarmonyPostfix]
        public static void Postfix(BaseBioReactor __instance)
        {
            float powerDelta = __instance._powerSource.power - powerLevel;
            __instance._powerSource.power = powerLevel + powerDelta * powerModifier;
        }
    }
}