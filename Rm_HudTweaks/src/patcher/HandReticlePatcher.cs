using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Rm_Config;
using UnityEngine;

namespace Rm_HudTweaks
{
    internal class HandReticlePatcher
    {
        private static HudTweaksConfig config = Config<HudTweaksConfig>.Get();
        //Set interact text, like names and the input icon.
        [HarmonyPatch(typeof(HandReticle), nameof(HandReticle.SetInteractText), new Type[] { typeof(string), typeof(string), typeof(bool), typeof(bool), typeof(HandReticle.Hand) })]
        internal class HandReticle_SetInteractText
        {
            [HarmonyPrefix]
            private static bool Prefix(HandReticle __instance, ref string text1, ref string text2, ref HandReticle.Hand hand)
            {
                if (config.HideNames)
                {
                    text1 = String.Empty;
                }
                if (config.DisableText2)
                {
                    text2 = String.Empty;
                }
                if (config.DisableInputIcon)
                {
                    hand = HandReticle.Hand.None;
                }
                return true;
            }
        }
        //Set use text.
        [HarmonyPatch(typeof(HandReticle), nameof(HandReticle.SetUseTextRaw))]
        internal class HandReticle_SetUseTextRaw
        {
            [HarmonyPrefix]
            private static bool Prefix(HandReticle __instance, ref string text1, ref string text2)
            {
                if (config.HideToolControlsText)
                {
                    text1 = String.Empty;
                }
                if (config.HideToolInfo)
                {
                    text2 = String.Empty;
                }
                return true;
            }
        }
        //Crosshair
        [HarmonyPatch(typeof(HandReticle), nameof(HandReticle.LateUpdate))]
        internal class HandReticle_LateUpdate
        {
            internal static bool activateCyclopsCrosshair = false;
            internal static float activateCyclopsCrosshairLastHover = Time.realtimeSinceStartup;
            [HarmonyPrefix]
            private static bool Prefix(HandReticle __instance)
            {
                __instance.desiredIconScale *= config.CrosshairScale;
                if (config.DisableCrosshair)
                {
                    if (config.AllowCrosshairCyclops && IsPilotingCyclops())
                    {
                        if (config.AllowCrosshairCyclopsOnlyOnHover &! activateCyclopsCrosshair &! WasRecentlyHovered())
                        {
                            __instance.desiredIconType = HandReticle.IconType.None;
                            return true;
                        }
                        __instance.desiredIconType = (HandReticle.IconType)config.CyclopsCrosshairIcon;
                        return true;
                    }
                    if (config.ShowCrosshairInteractIcons && __instance.desiredIconType != HandReticle.IconType.Default)
                    {
                        return true;
                    }
                    __instance.desiredIconType = HandReticle.IconType.None;
                }
                return true;
            }
            private static bool IsPilotingCyclops()
            {
                return Player.main && Player.main.IsPiloting() && Player.main.IsInSubmarine();
            }
            private static bool WasRecentlyHovered()
            {
                return Time.realtimeSinceStartup - activateCyclopsCrosshairLastHover < config.CyclopsSpeedCrosshairLingerTime;
            }
        }

        //Crosshair.CyclopsHovering
        //  SilentRunningAbilityButton
        [HarmonyPatch(typeof(CyclopsSilentRunningAbilityButton), nameof(CyclopsSilentRunningAbilityButton.OnMouseEnter))] 
        internal class CyclopsSilentRunningAbilityButton_OnMouseEnter 
        {
            [HarmonyPostfix]
            private static void Postfix()
            {
                HandReticle_LateUpdate.activateCyclopsCrosshair = true;
            }
        }
        [HarmonyPatch(typeof(CyclopsSilentRunningAbilityButton), nameof(CyclopsSilentRunningAbilityButton.OnMouseExit))]
        internal class CyclopsSilentRunningAbilityButton_OnMouseExit
        {
            [HarmonyPostfix]
            private static void Postfix()
            {
                HandReticle_LateUpdate.activateCyclopsCrosshair = false;
            }
        }
        //  SonarButton
        [HarmonyPatch(typeof(CyclopsSonarButton), nameof(CyclopsSonarButton.OnMouseEnter))]
        internal class CyclopsSonarButton_OnMouseEnter
        {
            [HarmonyPostfix]
            private static void Postfix(CyclopsSonarButton __instance)
            {
                if (__instance.active) HandReticle_LateUpdate.activateCyclopsCrosshair = true;
            }
        }
        [HarmonyPatch(typeof(CyclopsSonarButton), nameof(CyclopsSonarButton.OnMouseExit))]
        internal class CyclopsSonarButton_OnMouseExit
        {
            [HarmonyPostfix]
            private static void Postfix()
            {
                HandReticle_LateUpdate.activateCyclopsCrosshair = false;
            }
        }
        //  ExternalCamsButton
        [HarmonyPatch(typeof(CyclopsExternalCamsButton), nameof(CyclopsExternalCamsButton.OnMouseEnter))]
        internal class CyclopsExternalCamsButton_OnMouseEnter
        {
            [HarmonyPostfix]
            private static void Postfix()
            {
                HandReticle_LateUpdate.activateCyclopsCrosshair = true;
            }
        }
        [HarmonyPatch(typeof(CyclopsExternalCamsButton), nameof(CyclopsExternalCamsButton.OnMouseExit))]
        internal class CyclopsExternalCamsButton_OnMouseExit
        {
            [HarmonyPostfix]
            private static void Postfix()
            {
                HandReticle_LateUpdate.activateCyclopsCrosshair = false;
            }
        }
        //  DecoyLaunchButton
        [HarmonyPatch(typeof(CyclopsDecoyLaunchButton), nameof(CyclopsDecoyLaunchButton.OnMouseEnter))]
        internal class CyclopsDecoyLaunchButton_OnMouseEnter
        {
            [HarmonyPostfix]
            private static void Postfix(CyclopsDecoyLaunchButton __instance)
            {
                HandReticle_LateUpdate.activateCyclopsCrosshair = true;
            }
        }
        [HarmonyPatch(typeof(CyclopsDecoyLaunchButton), nameof(CyclopsDecoyLaunchButton.OnMouseExit))]
        internal class CyclopsDecoyLaunchButton_OnMouseExit
        {
            [HarmonyPostfix]
            private static void Postfix()
            {
                HandReticle_LateUpdate.activateCyclopsCrosshair = false;
            }
        }
        //  FireSuppressionSystemButton
        [HarmonyPatch(typeof(CyclopsFireSuppressionSystemButton), nameof(CyclopsFireSuppressionSystemButton.OnMouseEnter))]
        internal class CyclopsFireSuppressionSystemButton_OnMouseEnter
        {
            [HarmonyPostfix]
            private static void Postfix(CyclopsFireSuppressionSystemButton __instance)
            {
                if (__instance.active) HandReticle_LateUpdate.activateCyclopsCrosshair = true;
            }
        }
        [HarmonyPatch(typeof(CyclopsFireSuppressionSystemButton), nameof(CyclopsFireSuppressionSystemButton.OnMouseExit))]
        internal class CyclopsFireSuppressionSystemButton_OnMouseExit
        {
            [HarmonyPostfix]
            private static void Postfix()
            {
                HandReticle_LateUpdate.activateCyclopsCrosshair = false;
            }
        }
        //  ShieldButton
        [HarmonyPatch(typeof(CyclopsShieldButton), nameof(CyclopsShieldButton.OnMouseEnter))]
        internal class CyclopsShieldButton_OnMouseEnter
        {
            [HarmonyPostfix]
            private static void Postfix(CyclopsShieldButton __instance)
            {
                if (__instance.active) HandReticle_LateUpdate.activateCyclopsCrosshair = true;
            }
        }
        [HarmonyPatch(typeof(CyclopsShieldButton), nameof(CyclopsShieldButton.OnMouseExit))]
        internal class CyclopsShieldButton_OnMouseExit
        {
            [HarmonyPostfix]
            private static void Postfix()
            {
                HandReticle_LateUpdate.activateCyclopsCrosshair = false;
            }
        }
        //  EngineChangeState
        [HarmonyPatch(typeof(CyclopsEngineChangeState), nameof(CyclopsEngineChangeState.OnMouseEnter))]
        internal class CyclopsEngineChangeState_OnMouseEnter
        {
            [HarmonyPostfix]
            private static void Postfix()
            {
                HandReticle_LateUpdate.activateCyclopsCrosshair = true;
            }
        }
        [HarmonyPatch(typeof(CyclopsEngineChangeState), nameof(CyclopsEngineChangeState.OnMouseExit))]
        internal class CyclopsEngineChangeState_OnMouseExit
        {
            [HarmonyPostfix]
            private static void Postfix()
            {
                HandReticle_LateUpdate.activateCyclopsCrosshair = false;
            }
        }
        //  CyclopsMotorModeButton
        [HarmonyPatch(typeof(CyclopsMotorModeButton), nameof(CyclopsMotorModeButton.OnMouseOver))]
        internal class CyclopsMotorModeButton_OnMouseOver
        {
            [HarmonyPostfix]
            private static void Postfix(CyclopsMotorModeButton __instance)
            {
                HandReticle_LateUpdate.activateCyclopsCrosshairLastHover = Time.realtimeSinceStartup;
            }
        }
    }
}
