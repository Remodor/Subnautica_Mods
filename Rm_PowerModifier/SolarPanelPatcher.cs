using Harmony;

namespace Rm_PowerModifier
{
    [HarmonyPatch(typeof(SolarPanel))]
    [HarmonyPatch("Update")]
    internal class SolarPanel_Update_Patch
    {
        private static float powerLevel;
        private static float powerModifier;

        public static void SetPowerModifier(float modifier)
        {
            powerModifier = modifier;
        }

        [HarmonyPrefix]
        public static bool Prefix(SolarPanel __instance)
        {
            powerLevel = __instance.powerSource.power;
            return true;
        }
        [HarmonyPostfix]
        public static void Postfix(SolarPanel __instance)
        {
            float powerDelta = __instance.powerSource.power - powerLevel;
            if (powerDelta < 0)
            {
                return;
            }
            __instance.powerSource.power = powerLevel + powerDelta * powerModifier;
        }
    }
}
