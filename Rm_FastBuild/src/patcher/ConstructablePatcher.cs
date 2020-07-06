using Harmony;
using UnityEngine;

namespace Rm_FastBuild
{
    internal static class BuildSettings
    {
        internal static float buildingSpeed = 1f;
        internal static float maximumBuildTime = 6f;
        internal static float minimumBuildTime = 2f;
        internal static bool progressive = true;
    }
    [HarmonyPatch(typeof(Constructable))]
    [HarmonyPatch(nameof(Constructable.GetConstructInterval))]
    internal class Constructable_GetConstructInterval_Patch
    {
        internal static int numberOfResources = 1;
        private static float calculateLogisticBuildingSpeed()
        {
            float buildDelta = BuildSettings.maximumBuildTime - BuildSettings.minimumBuildTime;
            float offset = buildDelta - BuildSettings.minimumBuildTime;
            return ((buildDelta * 2) / (1 + Mathf.Exp(-1 * BuildSettings.buildingSpeed * (numberOfResources - 1))) - offset);
        }
        static void Postfix(ref float __result)
        {
            if (__result == 1)
            {
                if (!BuildSettings.progressive)
                {
                    float minimumBuildSpeed = BuildSettings.minimumBuildTime / numberOfResources;
                    float maximumBuildSpeed = BuildSettings.maximumBuildTime / numberOfResources;
                    __result = Mathf.Clamp(BuildSettings.buildingSpeed, minimumBuildSpeed, maximumBuildSpeed);
                }
                else
                {
                    __result = Mathf.Max(calculateLogisticBuildingSpeed(), 0.1f) / numberOfResources;
                }
            }
        }
    }
    [HarmonyPatch(typeof(Constructable))]
    [HarmonyPatch(nameof(Constructable.Construct))]
    internal class Constructable_Construct_Patch
    {
        static void Prefix(Constructable __instance)
        {
            Constructable_GetConstructInterval_Patch.numberOfResources = __instance.resourceMap.Count;
        }
    }
    [HarmonyPatch(typeof(Constructable))]
    [HarmonyPatch(nameof(Constructable.Deconstruct))]
    internal class Constructable_Deconstruct_Patch
    {
        static void Prefix(Constructable __instance)
        {
            Constructable_GetConstructInterval_Patch.numberOfResources = __instance.resourceMap.Count;
        }
    }
}
