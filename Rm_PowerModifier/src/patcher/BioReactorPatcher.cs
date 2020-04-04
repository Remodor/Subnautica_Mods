using Harmony;

namespace Rm_PowerModifier
{
    [HarmonyPatch(typeof(BaseBioReactor))]
    [HarmonyPatch(nameof(BaseBioReactor.Update))]
    internal class BioReactor_Update_Patch
    {
        private static float powerLevel;
        private static float powerModifier = 1.0f;

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
            if (powerDelta < 0)
            {
                return;
            }
            __instance._powerSource.SetPower(powerLevel + powerDelta * powerModifier);
        }
    }
}