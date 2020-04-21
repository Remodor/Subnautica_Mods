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
        internal static float internalLightEnergyConsumption = 0f;
        internal static float emergencyLightEnergyConsumption = 0f;
        internal static float externalLightEnergyConsumption = 0f;
        internal static float engineIdlingEnergyConsumption = 0f;
        internal static float cyclopsCameraLightEnergyConsumption = 0f;
        internal static float cyclopsSilentRunningEnergyConsumption = 1f;

        internal static float cameraRotationDamper = 3f;

        internal static bool cyclopsAutoLightDim = true;
        internal static bool alternativeCameraControls = true;
        internal static bool cyclopsSwapLightButtons = true;
        
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
            if (CyclopsSettings.cyclopsSwapLightButtons)
            {
                __instance.uiPanel.transform.localRotation = new Quaternion(0, -1.0f, 0, 0);
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
    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch(nameof(SubRoot.Start))]
    internal class SubRoot_Start_Patch
    {
        static void Postfix(SubRoot __instance)
        {
            __instance.silentRunningPowerCost = 0;
            __instance.lightControl.fadeDuration = 1.5f;
        }
    }
    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch(nameof(SubRoot.UpdateLighting))]
    internal class SubRoot_UpdateLighting_Patch
    {
        static void Prefix(SubRoot __instance, out bool __state)
        {
            __state = __instance.silentRunning;
            if ((CyclopsSettings.cyclopsAutoLightDim && Player.main.currentSub != __instance) || __instance.powerRelay.GetPowerStatus() == PowerSystem.Status.Emergency)
            {
                __instance.silentRunning = true;
            }
        }
        static void Postfix(SubRoot __instance, bool __state)
        {
            __instance.silentRunning = __state;
            if (__instance.lightingState == 1)
            {
                float energyCost = DayNightCycle.main.deltaTime * CyclopsSettings.emergencyLightEnergyConsumption;
                __instance.powerRelay.ConsumeEnergy(energyCost, out _);
            }
            else if (__instance.lightingState == 0)
            {
                float energyCost = DayNightCycle.main.deltaTime * CyclopsSettings.internalLightEnergyConsumption;
                __instance.powerRelay.ConsumeEnergy(energyCost, out _);
            }
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
                if (!__instance.powerRelay.ConsumeEnergy(energyCost, out _))
                {
                    __instance.sub.voiceNotificationManager.PlayVoiceNotification(__instance.sub.enginePowerDownNotification, true, false);
                    __instance.sub.BroadcastMessage("InvokeChangeEngineState", false, SendMessageOptions.RequireReceiver);
                }
            }
        }
    }
    [HarmonyPatch(typeof(VehicleDockingBay))]
    [HarmonyPatch(nameof(VehicleDockingBay.DockVehicle))]
    internal class VehicleDockingBay_DockVehicle_Patch
    {
        static void Postfix(VehicleDockingBay __instance)
        {
            var CinematicHatchControl = __instance.subRoot.GetComponentInChildren<CinematicHatchControl>();
            if (CinematicHatchControl != null)
            {
                CinematicHatchControl.hatch.Invoke(nameof(CinematicHatchControl.hatch.Close), 5.5f);
            }
        }
    }
    [HarmonyPatch(typeof(CyclopsEngineChangeState))]
    [HarmonyPatch(nameof(CyclopsEngineChangeState.OnClick))]
    internal class CyclopsEngineChangeState_OnClick_Patch
    {
        // Disallows turning the engine on without power.
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
        private static bool CycleLightUp()
        {
            bool changed = false;
            if (CyclopsExternalCams.lightState != 2)
            {
                changed = true;
            }
            CyclopsExternalCams.lightState = Mathf.Min(CyclopsExternalCams.lightState + 1, 2);
            return changed;
        }
        private static bool CycleLightDown()
        {
            bool changed = false;
            if (CyclopsExternalCams.lightState != 0)
            {
                changed = true;
            }
            CyclopsExternalCams.lightState = Mathf.Max(CyclopsExternalCams.lightState - 1, 0);
            return changed;
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
                    if (CycleLightUp())
                    {
                        FMODUWE.PlayOneShot(__instance.lightingPanel.vn_lightsOn, __instance.cameraLight.transform.position, 1f);
                    }
                    __instance.SetLight();
                }
                if (GameInput.GetButtonUp(GameInput.Button.LeftHand))
                {
                    if (CycleLightDown())
                    {
                        FMODUWE.PlayOneShot(__instance.lightingPanel.vn_lightsOff, __instance.cameraLight.transform.position, 1f);
                    }
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
                if (!__instance.lightingPanel.cyclopsRoot.powerRelay.ConsumeEnergy(energyCost, out _))
                {
                    __instance.cameraLight.enabled = false;
                    CyclopsExternalCams.lightState = 0;
                }
            }
        }
    }
    [HarmonyPatch(typeof(CyclopsSilentRunningAbilityButton))]
    [HarmonyPatch(nameof(CyclopsSilentRunningAbilityButton.Update))]
    internal class CyclopsSilentRunningAbilityButton_Update_Patch
    {
        static void Postfix(CyclopsSilentRunningAbilityButton __instance)
        {
            if (__instance.active)
            {
                float rawEnergyCost = Mathf.Max(CyclopsSettings.cyclopsSilentRunningEnergyConsumption * __instance.subRoot.noiseManager.GetNoisePercent(), CyclopsSettings.internalLightEnergyConsumption) * 2;
                float energyCost = DayNightCycle.main.deltaTime * rawEnergyCost;
                if (!__instance.subRoot.powerRelay.ConsumeEnergy(energyCost, out _))
                {
                    __instance.TurnOffSilentRunning();
                }
            }
        }
    }
    [HarmonyPatch(typeof(CyclopsSilentRunningAbilityButton))]
    [HarmonyPatch(nameof(CyclopsSilentRunningAbilityButton.TurnOnSilentRunning))]
    internal class CyclopsSilentRunningAbilityButton_TurnOnSilentRunning_Patch
    {
        //Skips the InvokeRepeating of "SilentRunningIteration".
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var methodInvokeRepeating = typeof(MonoBehaviour).GetMethod(nameof(MonoBehaviour.InvokeRepeating), new Type[] { typeof(string), typeof(float), typeof(float) });
            int state = 0;
            foreach (CodeInstruction instruction in instructions)
            {
                switch (state)
                {
                    case 0:
                        if (instruction.operand as String == "SilentRunningIteration")
                        {
                            state++;
                            continue;
                        }
                        break;
                    case 1:
                        if (instruction.opcode == OpCodes.Call && instruction.operand as MethodInfo == methodInvokeRepeating)
                        {
                            state++;
                        }
                        continue;
                    case 2:
                        state++;
                        continue;
                }
                yield return instruction;
            }
        }
    }
    [HarmonyPatch(typeof(CyclopsSilentRunningAbilityButton))]
    [HarmonyPatch(nameof(CyclopsSilentRunningAbilityButton.TurnOffSilentRunning))]
    internal class CyclopsSilentRunningAbilityButton_TurnOffSilentRunning_Patch
    {
        //Skips the CancelInvoke of "SilentRunningIteration".
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var methodCancelInvoke = typeof(MonoBehaviour).GetMethod(nameof(MonoBehaviour.CancelInvoke), new Type[] { typeof(string) });
            int state = 0;
            foreach (CodeInstruction instruction in instructions)
            {
                switch (state)
                {
                    case 0:
                        if (instruction.operand as String == "SilentRunningIteration")
                        {
                            state++;
                            continue;
                        }
                        break;
                    case 1:
                        if (instruction.opcode == OpCodes.Call && instruction.operand as MethodInfo == methodCancelInvoke)
                        {
                            state++;
                        }
                        continue;
                    case 2:
                        state++;
                        continue;
                }
                yield return instruction;
            }
        }
    }
    [HarmonyPatch(typeof(uGUI_CameraCyclops))]
    [HarmonyPatch(nameof(uGUI_CameraCyclops.UpdateBindings))]
    internal class uGUI_CameraCyclops_UpdateBindings_Patch
    {
        static bool Prefix(uGUI_CameraCyclops __instance)
        {
            string rightHand = uGUI.FormatButton(GameInput.Button.RightHand, false, " / ", false);
            string leftHand = uGUI.FormatButton(GameInput.Button.LeftHand, false, " / ", false);
            string slot1 = uGUI.FormatButton(GameInput.Button.Slot1, false, " / ", false);
            string slot2 = uGUI.FormatButton(GameInput.Button.Slot2, false, " / ", false);
            string slot3 = uGUI.FormatButton(GameInput.Button.Slot3, false, " / ", false);

            var exitText = LanguageCache.GetButtonFormat("PressToExit", GameInput.Button.Exit);
            var onLightText = "Lights On (" + rightHand + ")";
            var offLightText = "Lights Off (" + leftHand + ")";
            var selectCamera = "Select Camera (" + slot1 + "/" + slot2 + "/" + slot3 + ")";

            __instance.stringControls = "\n" + exitText + "\n" + onLightText + " / " + offLightText + "\n" + selectCamera;
            return false;
        }
    }
}
