using Harmony;
using UnityEngine;

namespace Rm_VehicleLightsImproved
{
    internal static class MapRoomSettings
    {
        internal static float mapRoomCameraLightEnergyConsumption = 0f;
    }

    [HarmonyPatch(typeof(MapRoomCamera))]
    [HarmonyPatch(nameof(MapRoomCamera.Start))]
    internal class MapRoomCamera_Start_Patch
    {
        static void Postfix(MapRoomCamera __instance)
        {
            var toggleLights = __instance.GetComponent<ToggleLights>();
            if (toggleLights == null)
            {
                toggleLights = __instance.gameObject.AddComponent<ToggleLights>();
            }

            toggleLights.lightsParent = __instance.lightsParent;
            toggleLights.energyMixin = __instance.energyMixin;

            toggleLights.SetLightsActive(false);

            var seaMothTemplate = SeaMothTemplate.Get();
            var seaMothToggleLights = seaMothTemplate.toggleLights;
            MapRoomCamera_ControlCamera_Patch.lightsOnSound = seaMothToggleLights.lightsOnSound;
            MapRoomCamera_ControlCamera_Patch.lightsOffSound = seaMothToggleLights.lightsOffSound;
            toggleLights.onSound = seaMothToggleLights.onSound;
            toggleLights.offSound = seaMothToggleLights.offSound;

            var lights = toggleLights.lightsParent.GetComponentsInChildren<Light>();
            foreach (var light in lights)
            {
                light.gameObject.SetActive(true);
                light.intensity = 0.8f;
                light.range = 40f;
                light.transform.localRotation = new Quaternion(0, -0.075f, 0, 1);
            }
            CustomVolumetricLight.CreateVolumetricLight(lights[0], seaMothTemplate, new Vector3(0.032f, -0.027f, -0.05f));
            CustomVolumetricLight.CreateVolumetricLight(lights[1], seaMothTemplate, new Vector3(-0.058f, 0.049f, -0.05f));
        }
    }
    [HarmonyPatch(typeof(MapRoomCamera))]
    [HarmonyPatch(nameof(MapRoomCamera.Update))]
    internal class MapRoomCamera_Update_Patch
    {
        static void Postfix(MapRoomCamera __instance)
        {
            if (__instance.IsControlled() && GameInput.GetButtonUp(GameInput.Button.RightHand))
            {
                var toggleLights = __instance.GetComponent<ToggleLights>();
                toggleLights.SetLightsActive(!toggleLights.lightsActive);
                toggleLights.energyPerSecond = MapRoomSettings.mapRoomCameraLightEnergyConsumption;
            }

        }
    }
    [HarmonyPatch(typeof(MapRoomCamera))]
    [HarmonyPatch(nameof(MapRoomCamera.ControlCamera))]
    internal class MapRoomCamera_ControlCamera_Patch
    {
        internal static FMOD_StudioEventEmitter lightsOnSound;
        internal static FMOD_StudioEventEmitter lightsOffSound;
        static void Prefix(MapRoomCamera __instance, out bool __state)
        {
            __state = __instance.lightsParent.activeSelf;
        }
        static void Postfix(MapRoomCamera __instance, bool __state)
        {
            __instance.lightsParent.SetActive(__state);
            __instance.GetAllComponentsInChildren<VFXVolumetricLight>().ForEach(x => x.DisableVolume());
            var toggleLights = __instance.GetComponent<ToggleLights>();
            toggleLights.lightsOnSound = lightsOnSound;
            toggleLights.lightsOffSound = lightsOffSound;
            var lights = toggleLights.lightsParent.GetComponentsInChildren<Light>();
            foreach (var light in lights)
            {
                light.transform.localRotation = new Quaternion(0, 0, 0, 1);
            }
        }
    }
    [HarmonyPatch(typeof(MapRoomCamera))]
    [HarmonyPatch(nameof(MapRoomCamera.FreeCamera))]
    internal class MapRoomCamera_FreeCamera_Patch
    {
        static void Prefix(MapRoomCamera __instance, out bool __state)
        {
            __state = __instance.lightsParent.activeSelf;
        }
        static void Postfix(MapRoomCamera __instance, bool __state)
        {
            __instance.lightsParent.SetActive(__state);
            __instance.GetAllComponentsInChildren<VFXVolumetricLight>().ForEach(x => x.RestoreVolume());
            var toggleLights = __instance.GetComponent<ToggleLights>();
            toggleLights.lightsOnSound = null;
            toggleLights.lightsOffSound = null;
            var lights = toggleLights.lightsParent.GetComponentsInChildren<Light>();
            foreach (var light in lights)
            {
                light.transform.localRotation = new Quaternion(0, -0.075f, 0, 1);
            }
        }
    }
    [HarmonyPatch(typeof(MapRoomCamera))]
    [HarmonyPatch(nameof(MapRoomCamera.SetDocked))]
    internal class MapRoomCamera_SetDocked_Patch
    {
        static void Postfix(MapRoomCamera __instance)
        {
            var toggleLights = __instance.GetComponent<ToggleLights>();
            if (toggleLights != null)
            {
                toggleLights.SetLightsActive(false);
            }
        }
    }
    [HarmonyPatch(typeof(MapRoomCamera))]
    [HarmonyPatch(nameof(MapRoomCamera.OnPickedUp))]
    internal class MapRoomCamera_OnPickedUp_Patch
    {
        static void Postfix(MapRoomCamera __instance)
        {
            var toggleLights = __instance.GetComponent<ToggleLights>();
            toggleLights.SetLightsActive(false);
        }
    }
}
