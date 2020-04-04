using Harmony;

namespace Rm_PowerModifier
{
    [HarmonyPatch(typeof(ThermalPlant))]
    [HarmonyPatch("AddPower")]
    internal class ThermalPlant_Update_Patch
    {
        private static float powerLevel;
        private static float powerModifier = 1.0f;

        public static void SetPowerModifier(float modifier)
        {
            powerModifier = modifier;
        }

        [HarmonyPrefix]
        public static bool Prefix(ThermalPlant __instance)
        {
            powerLevel = __instance.powerSource.power;
            return true;
        }
        [HarmonyPostfix]
        public static void Postfix(ThermalPlant __instance)
        {
            float powerDelta = __instance.powerSource.power - powerLevel;
            __instance.powerSource.power = powerLevel + powerDelta * powerModifier;
        }
    }
}
