using Harmony;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace Rm_VehicleLightsImproved
{
    internal static class CyclopsSettings
    {
        internal static float floodLightEnergyConsumption = 0f;
        internal static float cameraLightEnergyConsumption = 0f;

        internal static bool swapLightButtons = true;
        
        internal static float cameraLightIntensity = 1f;
        internal static float cameraLightRange = 55f;
    }
    [HarmonyPatch(typeof(CyclopsLightingPanel))]
    [HarmonyPatch(nameof(CyclopsLightingPanel.Start))]
    internal class CyclopsLightingPanel_Start_Patch
    {
        static void Prefix(CyclopsLightingPanel __instance)
        {
            __instance.lightingOn = false;
            __instance.floodlightsOn = false;
        }
    }
    [HarmonyPatch(typeof(CyclopsLightingPanel))]
    [HarmonyPatch(nameof(CyclopsLightingPanel.SubConstructionComplete))]
    internal class CyclopsLightingPanel_SubConstructionComplete_Patch
    {
        static void Postfix(CyclopsLightingPanel __instance)
        {
            __instance.lightingOn = false;
            __instance.floodlightsOn = false;
            __instance.cyclopsRoot.ForceLightingState(false);
            __instance.SetExternalLighting(false);
            __instance.UpdateLightingButtons();
        }
    }
    [HarmonyPatch(typeof(CyclopsLightingPanel))]
    [HarmonyPatch(nameof(CyclopsLightingPanel.Update))]
    internal class CyclopsLightingPanel_Update_Patch
    {
        static void Postfix(CyclopsLightingPanel __instance)
        {
            if (__instance.prevPowerRelayState && __instance.floodlightsOn)
            {
                float energyCost = DayNightCycle.main.deltaTime * CyclopsSettings.floodLightEnergyConsumption;
                __instance.cyclopsRoot.powerRelay.ConsumeEnergy(energyCost, out _);
            }
        }
    }
    [HarmonyPatch(typeof(CyclopsLightingPanel))]
    [HarmonyPatch(nameof(CyclopsLightingPanel.ButtonsOn))]
    internal class CyclopsLightingPanel_ButtonsOn_Patch
    {
        //Swaps the lighting buttons to make more sense.
        static void Postfix(CyclopsLightingPanel __instance)
        {
            if (CyclopsSettings.swapLightButtons)
            {
                __instance.uiPanel.transform.localRotation = new Quaternion(0, -1.0f, 0, 0);
            }
            else
            {
                __instance.uiPanel.transform.localRotation = new Quaternion(0, 0, 0, 0);
            }
        }
    }
    [HarmonyPatch(typeof(CyclopsLightingPanel))]
    [HarmonyPatch(nameof(CyclopsLightingPanel.ToggleFloodlights))]
    internal class CyclopsLightingPanel_ToggleFloodlights_Patch
    {
        //Fixed toggle lights bug with multiple Cyclops.
        static bool Prefix(CyclopsLightingPanel __instance)
        {
            if (Player.main.currentSub != __instance.cyclopsRoot)
            {
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(CyclopsLightingPanel))]
    [HarmonyPatch(nameof(CyclopsLightingPanel.TempTurnOffFloodlights))]
    internal class CyclopsLightingPanel_TempTurnOffFloodlights_Patch
    {
        internal static bool previous = false;
        //Really turns off floodlight to avoid energy consumption
        static bool Prefix(CyclopsLightingPanel __instance)
        {
            previous = __instance.floodlightsOn;
            if (__instance.floodlightsOn)
            {
                __instance.ToggleFloodlights();
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(CyclopsLightingPanel))]
    [HarmonyPatch(nameof(CyclopsLightingPanel.RestoreFloodlightsFromTempState))]
    internal class CyclopsLightingPanel_RestoreFloodlightsFromTempState_Patch
    {
        //Restores previous floodlight
        static bool Prefix(CyclopsLightingPanel __instance)
        {
            if (CyclopsLightingPanel_TempTurnOffFloodlights_Patch.previous)
            {
                __instance.ToggleFloodlights();
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(CyclopsExternalCams))]
    [HarmonyPatch(nameof(CyclopsExternalCams.Start))]
    internal class CyclopsExternalCams_Start_Patch
    {
        static void Postfix(CyclopsExternalCams __instance)
        {
            CyclopsExternalCams.lightState = 0;
            __instance.cameraLight.color = Color.white;
            __instance.cameraIndex = 1;
        }
    }
    [HarmonyPatch(typeof(CyclopsExternalCams))]
    [HarmonyPatch(nameof(CyclopsExternalCams.ChangeCamera))]
    internal class CyclopsExternalCams_ChangeCamera_Patch
    {
        //No more light reset on camera change.
        static void Prefix(out int __state)
        {
            __state = CyclopsExternalCams.lightState;
        }
        static void Postfix(CyclopsExternalCams __instance, int __state)
        {
            CyclopsExternalCams.lightState = __state;
            __instance.SetLight();
        }
    }
    [HarmonyPatch(typeof(CyclopsExternalCams))]
    [HarmonyPatch(nameof(CyclopsExternalCams.SetLight))]
    internal class CyclopsExternalCams_SetLight_Patch
    {
        static bool Prefix(CyclopsExternalCams __instance)
        {
            if (!__instance.lightingPanel.cyclopsRoot.powerRelay.IsPowered())
            {
                return false;
            }
            switch (CyclopsExternalCams.lightState)
            {
                case 0:
                    __instance.cameraLight.enabled = false;
                    return false;
                case 1:
                    __instance.cameraLight.range = CyclopsSettings.cameraLightRange;
                    __instance.cameraLight.intensity = CyclopsSettings.cameraLightIntensity;
                    __instance.cameraLight.enabled = true;
                    return false;
                case 2:
                    __instance.cameraLight.range = CyclopsSettings.cameraLightRange * 1.35f;
                    __instance.cameraLight.intensity = CyclopsSettings.cameraLightIntensity * 1.35f;
                    __instance.cameraLight.enabled = true;
                    return false;
                default:
                    return false;
            }
        }
    }
    [HarmonyPatch(typeof(CyclopsExternalCamsButton))]
    [HarmonyPatch(nameof(CyclopsExternalCamsButton.Update))]
    internal class CyclopsExternalCamsButton_Update_Patch
    {
        static void Postfix(CyclopsExternalCamsButton __instance)
        {
            if (__instance.cyclopsExternalCams.cameraLight.isActiveAndEnabled)
            {
                float energyCost = DayNightCycle.main.deltaTime * CyclopsSettings.cameraLightEnergyConsumption * CyclopsExternalCams.lightState;
                if (!__instance.subRoot.powerRelay.ConsumeEnergy(energyCost, out _))
                {
                    __instance.cyclopsExternalCams.cameraLight.enabled = false;
                    CyclopsExternalCams.lightState = 0;
                }
            }
        }
    }
}
