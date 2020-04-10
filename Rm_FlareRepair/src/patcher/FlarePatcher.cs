using Harmony;
using UnityEngine;

namespace Rm_FlareRepair
{
    [HarmonyPatch(typeof(Flare))]
    [HarmonyPatch(nameof(Flare.OnRightHandDown))]
    internal class Flare_RightDown_Patch
    {
        [HarmonyPostfix]
        internal static void Postfix(Flare __instance)
        {
            Flare_ToolUse_Patch.rightPressed = true;
            if (!__instance.hasBeenThrown)
            {
                __instance.energyLeft = DayNightCycle.main.timePassedAsFloat;
            }
        }
    }
    [HarmonyPatch(typeof(Flare))]
    [HarmonyPatch(nameof(Flare.OnRightHandUp))]
    internal class Flare_RightUp_Patch
    {
        [HarmonyPostfix]
        internal static void Postfix()
        {
            Flare_ToolUse_Patch.rightPressed = false;
        }
    }
    [HarmonyPatch(typeof(Flare))]
    [HarmonyPatch(nameof(Flare.Throw))]
    internal class Flare_ToolUse_Patch
    {
        internal static bool rightPressed = false;
        [HarmonyPrefix]
        internal static bool Prefix(Flare __instance)
        {
            if (rightPressed)
            {
                if (__instance.fxControl && !__instance.fxIsPlaying)
                {
                    __instance.fxControl.Play(1);
                    __instance.fxIsPlaying = true;
                }
                __instance._isInUse = false;
                __instance.isThrowing = false;
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(Flare))]
    [HarmonyPatch(nameof(Flare.OnDrop))]
    internal class Flare_DropFix_Patch
    {
        [HarmonyPrefix]
        internal static bool Prefix(Flare __instance)
        {
            if (__instance.energyLeft == 0)
            {
                __instance.isThrowing = false;
                __instance.GetComponent<Rigidbody>().AddForce(MainCamera.camera.transform.forward * __instance.dropForceAmount);
                __instance.GetComponent<Rigidbody>().AddTorque(__instance.transform.right * __instance.dropTorqueAmount);
                return false;
            }
            if (!__instance.hasBeenThrown)
            {
                __instance.isThrowing = false;
                return false;
            }
            __instance.SetFlareActiveState(true);
            return true;
        }
    }
    [HarmonyPatch(typeof(Flare))]
    [HarmonyPatch(nameof(Flare.OnDraw))]
    internal class Flare_Draw_Patch
    {
        [HarmonyPrefix]
        internal static bool Prefix(Flare __instance)
        {
            if (__instance.energyLeft == 0)
            {
                __instance.flareActiveState = true;
                return false;
            }
            __instance.flareActiveState = false;
            return true;
        }
    }

    [HarmonyPatch(typeof(Flare))]
    [HarmonyPatch(nameof(Flare.Update))]
    internal class Flare_EnergyDraw_Patch
    {
        private static float totalEnergy = 600f;
        private static float intensityModifier = 1f;
        internal static void SetTotalEnergy(float totalEnergy)
        {
            Flare_EnergyDraw_Patch.totalEnergy = totalEnergy;
        }
        internal static void SetIntensityModifier(float intensityModifier)
        {
            Flare_EnergyDraw_Patch.intensityModifier = intensityModifier;
        }
        [HarmonyPrefix]
        internal static bool Prefix(Flare __instance)
        {
            if (__instance.energyLeft == 0 || !__instance.hasBeenThrown)
            {
                __instance.sequence.Update();
                return false; //Full override!
            }
            if (__instance.flareActiveState)
            {
                __instance.flareActivateTime = __instance.energyLeft;
                __instance.sequence.Update();
                __instance.UpdateLight();
                __instance.light.intensity *= intensityModifier;
            } else
            {
                __instance.light.intensity = 0f;
            }
            float timeLeft = Mathf.Max(__instance.energyLeft + totalEnergy - DayNightCycle.main.timePassedAsFloat, 0f);
            float destructionDuration = 3f * DayNightCycle.main.dayNightSpeed;
            if (timeLeft < destructionDuration)
            {
                if (__instance.fxIsPlaying)
                {
                    __instance.fxControl.StopAndDestroy(1, 2f);
                    __instance.fxControl.Play(2);
                }
                if (timeLeft != 0)
                {
                    float perlin = Mathf.PerlinNoise(timeLeft * 2, 0);
                    float factor = timeLeft / destructionDuration;
                    float modifier = (0.8f * factor) + (0.3f * perlin);
                    __instance.light.intensity *= modifier;
                    __instance.light.range *= modifier;
                } else
                {
                    __instance.loopingSound.Stop();
                    __instance.light.enabled = false;
                    __instance.energyLeft = 0;
                }
            }
            return false; //Full override!
        }
    }
    [HarmonyPatch(typeof(Flare))]
    [HarmonyPatch(nameof(Flare.Awake))]
    internal class Flare_Load_Patch
    {
        [HarmonyPrefix]
        internal static bool Prefix(Flare __instance)
        {
            
            __instance.originalIntensity = __instance.light.intensity;
            __instance.originalrange = __instance.light.range;
            __instance.light.intensity = 0f;
            __instance.light.range = 0f;
            __instance.sequence = new Sequence();
            if (__instance.hasBeenThrown)
            {
                __instance.capRenderer.enabled = false;
                __instance.throwDuration = 0f;
                __instance.flareActiveState = false;
                if (__instance.fxControl)
                {
                    __instance.fxControl.Stop();
                    if (__instance.energyLeft != 0)
                    {
                        __instance.fxControl.Play(1);
                        __instance.fxIsPlaying = true;
                        __instance.light.enabled = true;
                        __instance.flareActiveState = true;
                    }
                }
            }
            return false; //Full override!
        }
    }
}
