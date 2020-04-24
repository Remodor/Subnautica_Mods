using Harmony;
using System;
using System.Text;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;

namespace Rm_VehicleLightsImproved
{
    [HarmonyPatch(typeof(Exosuit))]
    [HarmonyPatch(nameof(Exosuit.Start))]
    internal class Exosuit_Start_Patch
    {
        internal static ExosuitCustomLight currentExosuitCustomLight;
        static void Postfix(Exosuit __instance)
        {
            var exoToggleLights = __instance.GetComponent<ExosuitCustomLight>();
            if (exoToggleLights == null)
            {
                exoToggleLights = __instance.gameObject.AddComponent<ExosuitCustomLight>();
            }
            exoToggleLights.SetLightsActive(false);
            currentExosuitCustomLight = exoToggleLights;
            Exosuit_UpdateUIText_Patch.BuildToggleLightText();
        }
    }
    [HarmonyPatch(typeof(Exosuit))]
    [HarmonyPatch(nameof(Exosuit.OnPilotModeBegin))]
    internal static class Exosuit_OnPilotModeBegin_Patch
    {
        [HarmonyPostfix]
        static void Postfix(Exosuit __instance)
        {
            __instance.GetComponentsInChildren<VFXVolumetricLight>().ForEach(x => x.DisableVolume());
            var customLights = __instance.GetComponent<ExosuitCustomLight>();
            customLights.isPilotMode = true;
            Exosuit_Start_Patch.currentExosuitCustomLight = customLights;
            Exosuit_UpdateUIText_Patch.BuildToggleLightText();
        }
    }
    [HarmonyPatch(typeof(Exosuit))]
    [HarmonyPatch(nameof(Exosuit.OnPilotModeEnd))]
    internal static class Exosuit_OnPilotModeEnd_Patch
    {
        [HarmonyPostfix]
        static void Postfix(Exosuit __instance)
        {
            __instance.GetComponentsInChildren<VFXVolumetricLight>().ForEach(x => x.RestoreVolume());
            var customLights = __instance.GetComponent<ExosuitCustomLight>();
            customLights.isPilotMode = false;
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
    [HarmonyPatch(typeof(Exosuit))]
    [HarmonyPatch(nameof(Exosuit.UpdateUIText))]
    internal class Exosuit_UpdateUIText_Patch
    {
        private static string toggleLightTextOn;
        private static string toggleLightTextOff;
        internal static void BuildToggleLightText()
        {
            var keyText = uGUI.GetDisplayTextForBinding(GameInput.GetKeyCodeAsInputName(ExosuitSettings.exosuitToggleLightKey));
            StringBuilder sb = new StringBuilder();
            sb.Append("Lights Off ");
            sb.AppendFormat("(<color=#ADF8FFFF>{0}</color>)", keyText);
            toggleLightTextOff = sb.ToString();
            sb.Clear();
            sb.Append("Lights On ");
            sb.AppendFormat("(<color=#ADF8FFFF>{0}</color>)", keyText);
            toggleLightTextOn = sb.ToString();
        }
        static void Postfix(Exosuit __instance)
        {
            if (Exosuit_Start_Patch.currentExosuitCustomLight.IsLightActive())
            {
                HandReticle.main.useText1 = "\n" + HandReticle.main.useText1 + toggleLightTextOff;
            } 
            else
            {
                HandReticle.main.useText1 = "\n" + HandReticle.main.useText1 + toggleLightTextOn;
            }
        }
        // Removes the annoying "Exit" overlay.
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var methodAppendLine = typeof(StringBuilder).GetMethod(nameof(StringBuilder.AppendLine), new Type[] { typeof(string) });
            int state = 0;
            foreach (CodeInstruction instruction in instructions)
            {
                switch (state)
                {
                    case 0:
                        if (instruction.operand as String == "PressToExit")
                        {
                            state++;
                            continue;
                        }
                        break;
                    case 1:
                        if (instruction.opcode == OpCodes.Callvirt && instruction.operand as MethodInfo == methodAppendLine)
                        {
                            state++;
                        }
                        continue;
                }
                yield return instruction;
            }
        }
    }
}
