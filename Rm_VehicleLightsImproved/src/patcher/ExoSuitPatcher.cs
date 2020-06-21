using Harmony;
using System.Text;

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
            if (ExosuitSettings.toggleLightHud)
            {
                Exosuit_UpdateUIText_Patch.BuildToggleLightText();
            }
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
                var exosuitCustomLight = __instance.GetComponent<ExosuitCustomLight>();
                if (exosuitCustomLight != null)
                    exosuitCustomLight.SetLightsActive(false);
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
            var keyText = uGUI.GetDisplayTextForBinding(GameInput.GetKeyCodeAsInputName(ExosuitSettings.toggleLightKey));
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
            if (ExosuitSettings.toggleLightHud)
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
        }        
    }
}
