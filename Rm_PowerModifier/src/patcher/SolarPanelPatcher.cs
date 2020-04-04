using Harmony;

namespace Rm_PowerModifier
{
    [HarmonyPatch(typeof(SolarPanel))]
    [HarmonyPatch(nameof(SolarPanel.Update))]
    internal class SolarPanel_Update_Patch
    {
        private static float powerLevel;
        private static float powerModifier = 1.0f;

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
            __instance.powerSource.SetPower(powerLevel + powerDelta * powerModifier);
        }
    }
}
