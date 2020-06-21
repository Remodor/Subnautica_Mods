using Harmony;

namespace Rm_FastBuild
{
    internal static class BuildSettings
    {
        internal static float buildingSpeed = 1f;
    }
    [HarmonyPatch(typeof(Constructable))]
    [HarmonyPatch(nameof(Constructable.GetConstructInterval))]
    internal class Constructable_GetConstructInterval_Patch
    {
        static void Postfix(ref float __result)
        {
            if (__result == 1)
            {
                __result = BuildSettings.buildingSpeed;
            }
        }
    }
}
