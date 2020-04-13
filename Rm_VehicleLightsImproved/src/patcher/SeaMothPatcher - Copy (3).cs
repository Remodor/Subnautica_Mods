using Harmony;
using UnityEngine;

namespace Rm_VehicleLightRepair
{
    [HarmonyPatch(typeof(SeaMoth))]
    [HarmonyPatch(nameof(SeaMoth.Start))]
    internal class SeaMoth_Start_Patch
    {
        internal static void Set(float yyyy)
        {
            powerModifier = modifier;
        }

        [HarmonyPrefix]
        internal static bool Prefix(XXXX __instance)
        {

        }
        [HarmonyPostfix]
        internal static void Postfix(XXXX __instance)
        {

        }
    }
}
