using Harmony;

namespace Rm_PowerModifier
{
    [HarmonyPatch(typeof(BaseNuclearReactor))]
    [HarmonyPatch("Update")]
    internal class NuclearReactor_Update_Patch
    {
        private static float powerLevel;
        private static float powerModifier;

        public static void SetPowerModifier(float modifier)
        {
            powerModifier = modifier;
        }

        [HarmonyPrefix]
        public static bool Prefix(BaseNuclearReactor __instance)
        {
            powerLevel = __instance._powerSource.power;
            return true;
        }
        [HarmonyPostfix]
        public static void Postfix(BaseNuclearReactor __instance)
        {
            float powerDelta = __instance._powerSource.power - powerLevel;
            __instance._powerSource.power = powerLevel + powerDelta * powerModifier;
        }
    }
}