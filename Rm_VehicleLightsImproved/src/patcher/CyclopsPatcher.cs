using FMOD.Studio;
using Harmony;
using System;
using UnityEngine;

namespace Rm_VehicleLightsImproved
{
    internal static class CyclopsSettings
    {
        internal static float internalLightEnergyConsumption = 0f;
        internal static float emergencyLightEnergyConsumption = 0f;
        internal static float externalLightEnergyConsumption = 0f;
        internal static float engineIdlingEnergyConsumption = 0f;
        internal static float cyclopsCameraLightEnergyConsumption = 0f;
        internal static float cameraRotationDamper = 3f;
        internal static bool alternativeCameraControls = true;
        internal static float cyclopsCameraLightIntensity = 1f;
        internal static float cyclopsCameraLightRange = 55f;
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
                float energyCost = DayNightCycle.main.deltaTime * CyclopsSettings.externalLightEnergyConsumption;
                float consumedEnergy;
                __instance.cyclopsRoot.powerRelay.ConsumeEnergy(energyCost, out consumedEnergy);
            }
        }
    }
    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch(nameof(SubRoot.UpdateLighting))]
    internal class SubRoot_UpdateLighting_Patch
    {
        static void Prefix(SubRoot __instance, out bool __state)
        {
            __state = __instance.silentRunning;
            if (__instance.powerRelay.GetPowerStatus() == PowerSystem.Status.Emergency)
            {
                __instance.silentRunning = true;
            }
        }
        static void Postfix(SubRoot __instance, bool __state)
        {
            if (__instance.lightingState == 0)
            {
                float energyCost = DayNightCycle.main.deltaTime * CyclopsSettings.internalLightEnergyConsumption;
                float consumedEnergy;
                __instance.powerRelay.ConsumeEnergy(energyCost, out consumedEnergy);
            }
            else if (__instance.lightingState == 1)
            {
                float energyCost = DayNightCycle.main.deltaTime * CyclopsSettings.emergencyLightEnergyConsumption;
                float consumedEnergy;
                __instance.powerRelay.ConsumeEnergy(energyCost, out consumedEnergy);
            }
            __instance.silentRunning = __state;
        }
    }
    [HarmonyPatch(typeof(SubControl))]
    [HarmonyPatch(nameof(SubControl.Update))]
    internal class SubControl_Update_Patch
    {
        static void Postfix(SubControl __instance)
        {

            if (__instance.cyclopsMotorMode.engineOn && !__instance.appliedThrottle)
            {
                float energyCost = DayNightCycle.main.deltaTime * CyclopsSettings.engineIdlingEnergyConsumption;
                float consumedEnergy;
                __instance.powerRelay.ConsumeEnergy(energyCost, out consumedEnergy);
                if (consumedEnergy == 0 && energyCost != 0)
                {
                    __instance.sub.voiceNotificationManager.PlayVoiceNotification(__instance.sub.enginePowerDownNotification, true, false);
                    __instance.sub.BroadcastMessage("InvokeChangeEngineState", false, SendMessageOptions.RequireReceiver);
                }
            }
        }
    }
    [HarmonyPatch(typeof(CyclopsEngineChangeState))]
    [HarmonyPatch(nameof(CyclopsEngineChangeState.OnClick))]
    internal class CyclopsEngineChangeState_OnClick_Patch
    {
        static bool Prefix(CyclopsEngineChangeState __instance)
        {
            if (!__instance.subRoot.powerRelay.IsPowered())
            {
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(CyclopsCameraInput))]
    [HarmonyPatch(nameof(CyclopsCameraInput.Update))]
    internal class CyclopsCameraInput_Update_Patch
    {
        static void Prefix(CyclopsCameraInput __instance)
        {
            __instance.rotationSpeedDamper = CyclopsSettings.cameraRotationDamper;
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
    [HarmonyPatch(nameof(CyclopsExternalCams.HandleInput))]
    internal class CyclopsExternalCams_HandleInput_Patch
    {
        private static void CycleLightUp()
        {
            CyclopsExternalCams.lightState = Mathf.Min(CyclopsExternalCams.lightState + 1, 2);
        }
        private static void CycleLightDown()
        {
            CyclopsExternalCams.lightState = Mathf.Max(CyclopsExternalCams.lightState - 1, 0);
        }
        static bool Prefix(CyclopsExternalCams __instance, ref bool __result)
        {
            if (CyclopsSettings.alternativeCameraControls)
            {
                if (!__instance.usingCamera)
                {
                    __result = false;
                    return false;
                }
                if (!__instance.liveMixin.IsAlive())
                {
                    __instance.ExitCamera();
                    __result = false;
                    return false;
                }
                if (GameInput.GetButtonUp(GameInput.Button.Exit) || Input.GetKeyUp(KeyCode.Escape))
                {
                    __instance.ExitCamera();
                    __result = false;
                    return false;
                }
                if (GameInput.GetButtonDown(GameInput.Button.CycleNext))
                {
                    __instance.ChangeCamera(1);
                }
                else if (GameInput.GetButtonDown(GameInput.Button.CyclePrev))
                {
                    __instance.ChangeCamera(-1);
                }
                if (GameInput.GetButtonUp(GameInput.Button.RightHand))
                {
                    CycleLightUp();
                    __instance.SetLight();
                }
                if (GameInput.GetButtonUp(GameInput.Button.LeftHand))
                {
                    CycleLightDown();
                    __instance.SetLight();
                }
                if (GameInput.GetButtonUp(GameInput.Button.Slot1))
                {
                    __instance.cameraIndex = 1;
                    __instance.ChangeCamera(0);
                }
                if (GameInput.GetButtonUp(GameInput.Button.Slot2))
                {
                    __instance.cameraIndex = 2;
                    __instance.ChangeCamera(0);
                }
                if (GameInput.GetButtonUp(GameInput.Button.Slot3))
                {
                    __instance.cameraIndex = 0;
                    __instance.ChangeCamera(0);
                }
                __result = true;
                return false;
            }
            return true;
        }
    }  
    [HarmonyPatch(typeof(CyclopsExternalCams))]
    [HarmonyPatch(nameof(CyclopsExternalCams.ChangeCamera))]
    internal class CyclopsExternalCams_ChangeCamera_Patch
    {
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
                    __instance.cameraLight.range = CyclopsSettings.cyclopsCameraLightRange / 2;
                    __instance.cameraLight.intensity = CyclopsSettings.cyclopsCameraLightIntensity / 2;
                    __instance.cameraLight.enabled = true;
                    return false;
                case 2:
                    __instance.cameraLight.range = CyclopsSettings.cyclopsCameraLightRange;
                    __instance.cameraLight.intensity = CyclopsSettings.cyclopsCameraLightIntensity;
                    __instance.cameraLight.enabled = true;
                    return false;
                default:
                    return false;
            }
        }
    }
    [HarmonyPatch(typeof(CyclopsExternalCams))]
    [HarmonyPatch(nameof(CyclopsExternalCams.LateUpdate))]
    internal class CyclopsExternalCams_LateUpdate_Patch
    {
        static void Postfix(CyclopsExternalCams __instance)
        {
            if (__instance.cameraLight.isActiveAndEnabled)
            {
                var factor = __instance.cameraLight.intensity / CyclopsSettings.cyclopsCameraLightIntensity;
                float energyCost = DayNightCycle.main.deltaTime * CyclopsSettings.cyclopsCameraLightEnergyConsumption * factor;
                float consumedEnergy;
                __instance.lightingPanel.cyclopsRoot.powerRelay.ConsumeEnergy(energyCost, out consumedEnergy);
                if (consumedEnergy == 0 && energyCost != 0)
                {
                    __instance.cameraLight.enabled = false;
                    CyclopsExternalCams.lightState = 0;
                }
            }
        }
    }
 }
