using Harmony;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace Rm_VehiclesImproved
{
    internal static class CyclopsSettings
    {
        internal static float engineIdlingEnergyConsumption = 0f;
        internal static float silentRunningEnergyConsumption = 1f;

        internal static float cameraRotationDamper = 3f;

        internal static bool alternativeCameraControls = true;

        internal static bool autoCloseHatch = true;
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
            if (CyclopsSettings.autoCloseHatch && __instance.subRoot.isCyclops)
            {
                var CinematicHatchControl = __instance.subRoot.GetComponentInChildren<CinematicHatchControl>();
                if (CinematicHatchControl != null)
                {
                    CinematicHatchControl.hatch.Invoke(nameof(CinematicHatchControl.hatch.Close), 5.5f);
                }
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
    [HarmonyPatch(typeof(CyclopsSilentRunningAbilityButton))]
    [HarmonyPatch(nameof(CyclopsSilentRunningAbilityButton.Update))]
    internal class CyclopsSilentRunningAbilityButton_Update_Patch
    {
        static void Postfix(CyclopsSilentRunningAbilityButton __instance)
        {
            if (__instance.active)
            {
                float rawEnergyCost = Mathf.Max(CyclopsSettings.silentRunningEnergyConsumption * __instance.subRoot.noiseManager.GetNoisePercent(), CyclopsSettings.silentRunningEnergyConsumption * 0.05f) * 2;
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
            if (CyclopsSettings.alternativeCameraControls)
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

                __instance.stringControls = "\n" + exitText + "\n" + offLightText + " / " + onLightText + "\n" + selectCamera;
                return false;
            }
            return true;
        }
    }
}
