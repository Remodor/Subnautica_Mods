using HarmonyLib;
using UnityEngine;

namespace Rm_FastBuild;

[HarmonyPatch(typeof(Constructable))]
class ConstructablePatches
{
    private static int NumberOfResources = 1;
    private static float LogisticFunction(float numberOfResources)
    {
        float numerator = (Config.MaximumBuildTime_ - Config.MinimumBuildTime_) * 2;
        float denominator = 1 + Mathf.Exp(-1 * Config.ConstructionModifier_ * (numberOfResources - 1));
        float offset = Config.MaximumBuildTime_ - (2 * Config.MinimumBuildTime_);
        return numerator / denominator - offset;
    }

    [HarmonyPatch(nameof(Constructable.GetConstructInterval))]
    [HarmonyPostfix]
    static void GetConstructInterval_Postfix(ref float __result)
    {
#if DEBUG 
        Dbg.FloatChanged("Duration per resource, before:", __result, 2);
#endif
        if (Config.CreativeInstantBuild_ && GameModeUtils.currentGameMode == GameModeOption.Creative)
        {
            __result = 0.01f;
        }
        else if (Config.ModifierMode_ == ModifierMode.Linear)
        {
            float minimumResourceTime = Config.MinimumBuildTime_ / NumberOfResources;
            float maximumResourceTime = Config.MaximumBuildTime_ / NumberOfResources;
            __result = Mathf.Clamp(Config.ConstructionModifier_, minimumResourceTime, maximumResourceTime);
        }
        else
        {
            float buildTime = LogisticFunction(NumberOfResources);
            __result = Mathf.Max(buildTime / NumberOfResources, 0.01f);
        }
#if DEBUG
        Dbg.FloatChanged("Duration per resource, after:", __result, 2);
        Dbg.FloatChanged("Total Build Time:", __result * NumberOfResources, 2);
#endif
    }
    [HarmonyPatch(nameof(Constructable.Construct))]
    [HarmonyPrefix]
    static void Construct_Prefix(Constructable __instance)
    {
        NumberOfResources = Mathf.Max(__instance.resourceMap.Count, 1);
    }

    [HarmonyPatch(nameof(Constructable.DeconstructAsync))]
    [HarmonyPrefix]
    static void DeconstructAsync_Prefix(Constructable __instance)
    {
        NumberOfResources = Mathf.Max(__instance.resourceMap.Count, 1);
    }
}