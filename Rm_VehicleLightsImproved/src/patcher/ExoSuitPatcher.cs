using Harmony;
using UnityEngine;

namespace Rm_VehicleLightsImproved
{

    [HarmonyPatch(typeof(Exosuit))]
    [HarmonyPatch(nameof(Exosuit.Start))]
    internal class Exosuit_Start_Patch
    {
        static void Postfix(Exosuit __instance)
        {
            var exoToggleLights = __instance.gameObject.AddComponent<ExosuitCustomLight>();
            exoToggleLights.SetLightsActive(false);
        }
    }
    [HarmonyPatch(typeof(Exosuit))]
    [HarmonyPatch(nameof(Exosuit.OnPilotModeBegin))]
    internal static class Exosuit_OnPilotModeBegin_Patch
    {
        [HarmonyPostfix]
        static void Postfix(Exosuit __instance)
        {
            __instance.GetComponentsInChildren<VFXVolumetricLight>()
                        .ForEach(x => x.DisableVolume());
            __instance.GetComponent<ExosuitCustomLight>().isPilotMode = true;
        }
    }
    [HarmonyPatch(typeof(Exosuit))]
    [HarmonyPatch(nameof(Exosuit.OnPilotModeEnd))]
    internal static class Exosuit_OnPilotModeEnd_Patch
    {
        [HarmonyPostfix]
        static void Postfix(Exosuit __instance)
        {
            __instance.GetComponentsInChildren<VFXVolumetricLight>()
                        .ForEach(x => x.RestoreVolume());
            __instance.GetComponent<ExosuitCustomLight>().isPilotMode = false;
        }
    }
    [HarmonyPatch(typeof(Exosuit))]
    [HarmonyPatch(nameof(Exosuit.OnDockedChanged))]
    internal class Exosuit_OnDockedChanged_Patch
    {
        [HarmonyPostfix]
        static void Postfix(Exosuit __instance, bool docked)
        {
            if (docked)
            {
                __instance.GetComponent<ExosuitCustomLight>().SetLightsActive(false);
            }
        }
    }
}
