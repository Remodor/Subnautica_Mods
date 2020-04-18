using Harmony;


namespace Rm_VehicleLightsImproved
{
    [HarmonyPatch(typeof(VFXVolumetricLight))]
    [HarmonyPatch(nameof(VFXVolumetricLight.Awake))]
    internal class VFXVolumetricLight_Awake_Patch
    {
        static void Postfix(VFXVolumetricLight __instance)
        {
            __instance.DisableVolume();
        }

    }
}
