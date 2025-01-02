using HarmonyLib;
using UnityEngine;

namespace Rm_HudTweaks;

[HarmonyPatch(typeof(CyclopsMotorModeButton))]
class CyclopsLastSpeedHoverPatches
{
    [HarmonyPatch(nameof(CyclopsMotorModeButton.OnMouseOver))]
    [HarmonyPostfix]
    static void OnMouseOver_Postfix()
    {
        if (Config.CyclopsCrosshairOnlyOnHover_ && Player.main.mode == Player.Mode.Piloting)
        {
            HandReticlePatches.CyclopsLastSpeedHover = Time.realtimeSinceStartup;
            HandReticlePatches.ShowCyclopsCrosshair = true;
        }
    }
}

[HarmonyPatch(typeof(CyclopsEngineChangeState))]
class CyclopsEngineChangeStatePatches
{
    [HarmonyPatch(nameof(CyclopsEngineChangeState.OnMouseEnter))]
    [HarmonyPostfix]
    static void OnMouseEnter_Postfix()
    {
        if (Config.CyclopsCrosshairOnlyOnHover_ && Player.main.mode == Player.Mode.Piloting)
        {
            HandReticlePatches.ShowCyclopsCrosshair = true;
            HandReticlePatches.CyclopsLastSpeedHover = 0;
        }
    }
    [HarmonyPatch(nameof(CyclopsEngineChangeState.OnMouseExit))]
    [HarmonyPostfix]
    static void OnMouseExit_Postfix()
    {
        if (Config.CyclopsCrosshairOnlyOnHover_)
        {
            HandReticlePatches.ShowCyclopsCrosshair = false;
        }
    }
}

[HarmonyPatch(typeof(CyclopsShieldButton))]
class CyclopsShieldButtonPatches
{
    [HarmonyPatch(nameof(CyclopsShieldButton.OnMouseEnter))]
    [HarmonyPostfix]
    static void OnMouseEnter_Postfix()
    {
        if (Config.CyclopsCrosshairOnlyOnHover_ && Player.main.mode == Player.Mode.Piloting)
        {
            HandReticlePatches.ShowCyclopsCrosshair = true;
            HandReticlePatches.CyclopsLastSpeedHover = 0;
        }
    }
    [HarmonyPatch(nameof(CyclopsShieldButton.OnMouseExit))]
    [HarmonyPostfix]
    static void OnMouseExit_Postfix()
    {
        if (Config.CyclopsCrosshairOnlyOnHover_)
        {
            HandReticlePatches.ShowCyclopsCrosshair = false;
        }
    }
}

[HarmonyPatch(typeof(CyclopsFireSuppressionSystemButton))]
class CyclopsFireSuppressionSystemButtonPatches
{
    [HarmonyPatch(nameof(CyclopsFireSuppressionSystemButton.OnMouseEnter))]
    [HarmonyPostfix]
    static void OnMouseEnter_Postfix()
    {
        if (Config.CyclopsCrosshairOnlyOnHover_ && Player.main.mode == Player.Mode.Piloting)
        {
            HandReticlePatches.ShowCyclopsCrosshair = true;
            HandReticlePatches.CyclopsLastSpeedHover = 0;
        }
    }
    [HarmonyPatch(nameof(CyclopsFireSuppressionSystemButton.OnMouseExit))]
    [HarmonyPostfix]
    static void OnMouseExit_Postfix()
    {
        if (Config.CyclopsCrosshairOnlyOnHover_)
        {
            HandReticlePatches.ShowCyclopsCrosshair = false;
        }
    }
}

[HarmonyPatch(typeof(CyclopsDecoyLaunchButton))]
class CyclopsDecoyLaunchButtonPatches
{
    [HarmonyPatch(nameof(CyclopsDecoyLaunchButton.OnMouseEnter))]
    [HarmonyPostfix]
    static void OnMouseEnter_Postfix()
    {
        if (Config.CyclopsCrosshairOnlyOnHover_ && Player.main.mode == Player.Mode.Piloting)
        {
            HandReticlePatches.ShowCyclopsCrosshair = true;
            HandReticlePatches.CyclopsLastSpeedHover = 0;
        }
    }
    [HarmonyPatch(nameof(CyclopsDecoyLaunchButton.OnMouseExit))]
    [HarmonyPostfix]
    static void OnMouseExit_Postfix()
    {
        if (Config.CyclopsCrosshairOnlyOnHover_)
        {
            HandReticlePatches.ShowCyclopsCrosshair = false;
        }
    }
}

[HarmonyPatch(typeof(CyclopsExternalCamsButton))]
class CyclopsExternalCamsButtonPatches
{
    [HarmonyPatch(nameof(CyclopsExternalCamsButton.OnMouseEnter))]
    [HarmonyPostfix]
    static void OnMouseEnter_Postfix()
    {
        if (Config.CyclopsCrosshairOnlyOnHover_ && Player.main.mode == Player.Mode.Piloting)
        {
            HandReticlePatches.ShowCyclopsCrosshair = true;
            HandReticlePatches.CyclopsLastSpeedHover = 0;
        }
    }
    [HarmonyPatch(nameof(CyclopsExternalCamsButton.OnMouseExit))]
    [HarmonyPostfix]
    static void OnMouseExit_Postfix()
    {
        if (Config.CyclopsCrosshairOnlyOnHover_)
        {
            HandReticlePatches.ShowCyclopsCrosshair = false;
        }
    }
}

[HarmonyPatch(typeof(CyclopsSonarButton))]
class CyclopsSonarButtonPatches
{
    [HarmonyPatch(nameof(CyclopsSonarButton.OnMouseEnter))]
    [HarmonyPostfix]
    static void OnMouseEnter_Postfix()
    {
        if (Config.CyclopsCrosshairOnlyOnHover_ && Player.main.mode == Player.Mode.Piloting)
        {
            HandReticlePatches.ShowCyclopsCrosshair = true;
            HandReticlePatches.CyclopsLastSpeedHover = 0;
        }
    }
    [HarmonyPatch(nameof(CyclopsSonarButton.OnMouseExit))]
    [HarmonyPostfix]
    static void OnMouseExit_Postfix()
    {
        if (Config.CyclopsCrosshairOnlyOnHover_)
        {
            HandReticlePatches.ShowCyclopsCrosshair = false;
        }
    }
}

[HarmonyPatch(typeof(CyclopsSilentRunningAbilityButton))]
class CyclopsSilentRunningAbilityButtonPatches
{
    [HarmonyPatch(nameof(CyclopsSilentRunningAbilityButton.OnMouseEnter))]
    [HarmonyPostfix]
    static void OnMouseEnter_Postfix()
    {
        if (Config.CyclopsCrosshairOnlyOnHover_ && Player.main.mode == Player.Mode.Piloting)
        {
            HandReticlePatches.CyclopsLastSpeedHover = 0;
            HandReticlePatches.ShowCyclopsCrosshair = true;
        }
    }
    [HarmonyPatch(nameof(CyclopsSilentRunningAbilityButton.OnMouseExit))]
    [HarmonyPostfix]
    static void OnMouseExit_Postfix()
    {
        if (Config.CyclopsCrosshairOnlyOnHover_)
        {
            HandReticlePatches.ShowCyclopsCrosshair = false;
        }
    }
}