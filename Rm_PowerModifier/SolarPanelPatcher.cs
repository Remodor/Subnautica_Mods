using Harmony;
using Rm_Config;

namespace Rm_PowerModifier
{
    [HarmonyPatch(typeof(SolarPanel))]
    [HarmonyPatch("Update")]
    internal class SolarPanel_Update_Patch
    {
        private static float powerLevel;
        private static float powerModifier;

        public static void SetPowerModider(float modifier)
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
            if (__instance.gameObject.GetComponent<Constructable>().constructed)
            {
                float powerDelta = __instance.powerSource.power - powerLevel;
                __instance.powerSource.power = powerLevel + powerDelta * powerModifier;
            }
        }
    }
}
