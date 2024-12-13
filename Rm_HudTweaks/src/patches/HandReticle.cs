using HarmonyLib;
using UnityEngine;

namespace Rm_HudTweaks;
[HarmonyPatch(typeof(HandReticle))]
class HandReticlePatches
{
    // Set interact text, like names and locker content.
    [HarmonyPatch(nameof(HandReticle.SetText))]
    [HarmonyPrefix]
    static bool SetText_Prefix(HandReticle __instance, HandReticle.TextType type, ref string text, ref GameInput.Button button)
    {
        switch (type)
        {
            case HandReticle.TextType.Hand:
                bool hideNames = Config.HideNames_;
                bool hideInputIcon = Config.HideInputIcon_;
                if (hideNames && hideInputIcon)
                {
                    return false; // Full override!
                }
                if (hideNames)
                {
                    text = string.Empty;
                }
                if (hideInputIcon)
                {
                    button = GameInput.Button.None;
                }
                return true;
            case HandReticle.TextType.HandSubscript:
                return !Config.HideText2_; // Full override!
            default:
#if DEBUG
                Dbg.Print($"SetText: (TextType) {type} (text) {text}");
#endif
                return true;
        }
    }
    internal static float CyclopsLastSpeedHover = 0;
    internal static bool ShowCyclopsCrosshair = false;
    // Hide tool controls and tool info. Set icon scale. Show cyclops crosshair. Hide crosshair.
    [HarmonyPatch(nameof(HandReticle.LateUpdate))]
    [HarmonyPrefix]
    static void LateUpdate_Prefix(HandReticle __instance)
    {
        if (Config.HideToolControlsText_)
        {
            __instance.textUse = string.Empty;
        }
        if (Config.HideToolInfo_)
        {
            __instance.textUseSubscript = string.Empty;
        }
        __instance.desiredIconScale *= Config.CrosshairScale_;
        if (ShowCyclopsCrosshair)
        {
            if (CyclopsLastSpeedHover == 0 || Time.realtimeSinceStartup - CyclopsLastSpeedHover < Config.CyclopsSpeedLingerTime_)
            {
                __instance.desiredIconType = (HandReticle.IconType)Config.CyclopsCrosshairIcon_;

            }
            else
            {
                CyclopsLastSpeedHover = 0;
                ShowCyclopsCrosshair = false;
            }
        }
        else if (Config.HideCrosshair_ && __instance.desiredIconType == HandReticle.IconType.Default)
        {
            __instance.desiredIconType = HandReticle.IconType.None;
        }
    }
    // Hide interact icons.
    [HarmonyPatch(nameof(HandReticle.SetIcon))]
    [HarmonyPrefix]
    static void SetIcon_Prefix(ref HandReticle.IconType type)
    {

        if (Config.HideInteractIcons_)
            type = HandReticle.IconType.Default;
    }
}