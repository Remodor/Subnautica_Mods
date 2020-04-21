using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            Console.WriteLine("#Start");
            var toggleLights = __instance.GetComponent<ToggleLights>();
            if (toggleLights == null)
            {
                toggleLights = __instance.gameObject.AddComponent<ToggleLights>();
            }
            Console.WriteLine("#toggleLights1 : {0}", toggleLights);

            toggleLights.lightsParent = __instance.lightsParent;
            toggleLights.energyMixin = __instance.energyMixin;
            toggleLights.SetLightsActive(false);
            Console.WriteLine("#toggleLights.lightsActive: {0}", toggleLights.lightsActive);
            Console.WriteLine("#__instance.lightsParent.activeSelf: {0}", __instance.lightsParent.activeSelf);
            //Console.WriteLine("#toggleLights2 : {0}", __instance.GetComponentInChildren<ToggleLights>());
            //Console.WriteLine("#toggleLights3 : {0}", __instance.gameObject.GetComponent<ToggleLights>());
            //Console.WriteLine("#toggleLights4 : {0}", __instance.gameObject.GetComponentInChildren<ToggleLights>());
            //var toggleLights = __instance.gameObject.AddComponent<ToggleLights>();


            var seaMothTemplate = SeaMothTemplate.Get();
            var seaMothToggleLights = seaMothTemplate.toggleLights;
            toggleLights.lightsOnSound = seaMothToggleLights.lightsOnSound;
            toggleLights.lightsOffSound = seaMothToggleLights.lightsOffSound;
            toggleLights.onSound = seaMothToggleLights.onSound;
            toggleLights.offSound = seaMothToggleLights.offSound;



            toggleLights.energyPerSecond = MapRoomSettings.mapRoomCameraLightEnergyConsumption;
            //Console.WriteLine("#toggleLights : {0}", toggleLights);
            foreach (var light in toggleLights.lightsParent.GetComponentsInChildren<Light>())
            {
                light.gameObject.SetActive(true);
                Console.WriteLine("#light.range: {0}", light.range);
                Console.WriteLine("#light.intensity: {0}", light.intensity);
                Console.WriteLine("#light.spotAngle: {0}", light.spotAngle);
                CustomVolumetricLight.CreateVolumetricLight(light, seaMothTemplate);
            }
            Console.WriteLine("#EndStart");

        }
    }
    [HarmonyPatch(typeof(MapRoomCamera))]
    [HarmonyPatch(nameof(MapRoomCamera.Update))]
    internal class MapRoomCamera_Update_Patch
    {
        static void Postfix(MapRoomCamera __instance)
        {
            //Console.WriteLine("#toggleLights1u : {0}", __instance.GetComponent<ToggleLights>());
            //Console.WriteLine("#toggleLights2u : {0}", __instance.GetComponentInChildren<ToggleLights>());
            //Console.WriteLine("#toggleLights3u : {0}", __instance.gameObject.GetComponent<ToggleLights>());
            //Console.WriteLine("#toggleLights4u : {0}", __instance.gameObject.GetComponentInChildren<ToggleLights>());

            if (__instance.IsControlled() && GameInput.GetButtonUp(GameInput.Button.RightHand))
            {
                Console.WriteLine("#U");
                var toggleLights = __instance.GetComponent<ToggleLights>();

                Console.WriteLine("#U toggleLights: {0}", toggleLights);
                toggleLights.SetLightsActive(!toggleLights.lightsActive);
            }

        }
    }
    //[HarmonyPatch(typeof(MapRoomCamera))]
    //[HarmonyPatch(nameof(MapRoomCamera.ControlCamera))]
    //internal class MapRoomCamera_ControlCamera_Patch
    //{
    //    static void Prefix(MapRoomCamera __instance, out bool __state)
    //    {
    //        __state = __instance.lightsParent.activeSelf;
    //    }
    //    static void Postfix(MapRoomCamera __instance, bool __state)
    //    {
    //        __instance.lightsParent.SetActive(__state);
    //        __instance.GetAllComponentsInChildren<VFXVolumetricLight>().ForEach(x => x.DisableVolume());
    //    }
    //}
    //[HarmonyPatch(typeof(MapRoomCamera))]
    //[HarmonyPatch(nameof(MapRoomCamera.FreeCamera))]
    //internal class MapRoomCamera_FreeCamera_Patch
    //{
    //    static void Prefix(MapRoomCamera __instance, out bool __state)
    //    {
    //        __state = __instance.lightsParent.activeSelf;
    //    }
    //    static void Postfix(MapRoomCamera __instance, bool __state)
    //    {
    //        __instance.lightsParent.SetActive(__state);
    //        __instance.GetAllComponentsInChildren<VFXVolumetricLight>().ForEach(x => x.RestoreVolume());
    //    }
    //}
    //[HarmonyPatch(typeof(MapRoomCamera))]
    //[HarmonyPatch(nameof(MapRoomCamera.SetDocked))]
    //internal class MapRoomCamera_SetDocked_Patch
    //{
    //    static void Postfix(MapRoomCamera __instance)
    //    {
    //        var toggleLights = __instance.GetComponent<ToggleLights>();
    //        toggleLights.SetLightsActive(false);
    //    }
    //}
    //[HarmonyPatch(typeof(MapRoomCamera))]
    //[HarmonyPatch(nameof(MapRoomCamera.OnPickedUp))]
    //internal class MapRoomCamera_OnPickedUp_Patch
    //{
    //    static void Postfix(MapRoomCamera __instance)
    //    {
    //        var toggleLights = __instance.GetComponent<ToggleLights>();
    //        toggleLights.SetLightsActive(false);
    //    }
    //}
}
