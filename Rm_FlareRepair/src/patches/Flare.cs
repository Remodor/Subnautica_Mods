
using HarmonyLib;
using UnityEngine;

namespace Rm_FlareRepair;

[HarmonyPatch(typeof(Flare))]
class FlarePatches
{
    [HarmonyPatch(nameof(Flare.Awake))]
    [HarmonyPrefix]
    static bool Awake_Prefix(Flare __instance)
    {
#if DEBUG
        Dbg.Print("Awake");
        Dbg.BoolChanged("fxIsPlaying", __instance.fxIsPlaying);
#endif
        __instance.originalIntensity = __instance.light.intensity;
        __instance.originalrange = __instance.light.range;
        __instance.light.intensity = 0f;
        __instance.light.range = 0f;
        __instance.sequence = new Sequence();
        if (!__instance.hasBeenThrown)
        {
            return false; //Full override!
        }
        __instance.capRenderer.enabled = false;
        __instance.throwDuration = 0f;
        __instance.flareActiveState = true;
        if (__instance.energyLeft != 0)
        {
            __instance.light.enabled = true;
            __instance.loopingSound.Play();
            if (__instance.fxControl)
            {
                __instance.fxControl.Play(1);
                __instance.fxIsPlaying = true;
            }
        }
        return false; //Full override!
    }
    static bool RightPressed = false;
    [HarmonyPatch(nameof(Flare.Update))]
    [HarmonyPrefix]
    static bool Update_Prefix(Flare __instance)
    {
        if (__instance.isThrowing)
        {
            __instance.sequence.Update();
        }
        if (!__instance.flareActiveState)
        {
#if DEBUG
            Dbg.FloatChanged("NotActive: energyLeft", __instance.energyLeft);
            Dbg.BoolChanged("NotActive: fxIsPlaying", __instance.fxIsPlaying);
            if (__instance.energyLeft != 0)
            {
                Dbg.FloatChanged("NotActive: flareActivateTime", __instance.flareActivateTime);
            }
#endif
            __instance.light.intensity = 0.0f;
            return false; //Full override!
        }
        __instance.throwDuration = 0f;
        if (__instance.energyLeft == 0)
        {
#if DEBUG
            Dbg.FloatChanged("NoEnergy: energyLeft", __instance.energyLeft);
            Dbg.BoolChanged("NoEnergy: fxIsPlaying", __instance.fxIsPlaying);
            if (__instance.energyLeft != 0)
            {
                Dbg.FloatChanged("NoEnergy: flareActivateTime", __instance.flareActivateTime);
            }
#endif
            if (!Config.KeepDepletedFlare_())
            {
                UnityEngine.Object.Destroy(__instance.gameObject, 2f);
                return false; //Full override!
            }
            __instance.light.intensity = 0.0f;
            __instance.light.enabled = false;
            __instance.loopingSound.Stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            if (__instance.fxIsPlaying)
            {
                __instance.fxControl.StopAndDestroy(1, 0f);
                __instance.fxIsPlaying = false;
            }
            return false; //Full override!
        }
#if DEBUG
        Dbg.FloatChanged("Burning: energyLeft", __instance.energyLeft);
        Dbg.BoolChanged("Burning: fxIsPlaying", __instance.fxIsPlaying);
        if (__instance.energyLeft != 0)
        {
            Dbg.FloatChanged("Burning: flareActivateTime", __instance.flareActivateTime);
        }
#endif
        float timePassed = DayNightCycle.main.timePassedAsFloat;
        float dayNightSpeed = DayNightCycle.main.dayNightSpeed;
        if (__instance.energyLeft == 1800)
        {
            __instance.energyLeft = timePassed;
        }
        float deltaActiveTime = timePassed - __instance.flareActivateTime;
        float cleanedDeltaActiveTime = deltaActiveTime / dayNightSpeed;
        float previousFlareActiveTime = __instance.flareActivateTime;
        __instance.flareActivateTime = timePassed - cleanedDeltaActiveTime;
        __instance.UpdateLight();
        __instance.flareActivateTime = previousFlareActiveTime;
        __instance.light.intensity *= Config.IntensityModifier_();
        __instance.loopingSound.Play();
        float totalDuration = Config.TotalDuration_();
        float timeLeft = Mathf.Max(__instance.energyLeft + totalDuration - timePassed, 0f);
        float destructionDuration = 3f * dayNightSpeed;
#if DEBUG
        Dbg.FloatChanged("timeLeft", timeLeft);
        Dbg.FloatChanged("totalDuration", totalDuration);
        Dbg.FloatChanged("destructionDuration", destructionDuration);
#endif
        float dimmingThreshold = totalDuration * Config.ProgressiveDimmingThreshold_();
        if (timeLeft < dimmingThreshold)
        {
            float relativeProgression = timeLeft / dimmingThreshold;
            float minimum = Config.MinimumRelativeFactor_();
            float remainder = 1 - minimum;
            float factor = minimum + remainder * relativeProgression;
#if DEBUG
            Dbg.FloatChanged("dimmingThreshold", dimmingThreshold);
            Dbg.FloatChanged("relativeProgression", relativeProgression, 2);
            Dbg.FloatChanged("factor", factor, 2);
#endif
            __instance.light.intensity *= factor;
        }
        if (timeLeft == 0)
        {
            __instance.energyLeft = 0;
        }
        else if (__instance.fxIsPlaying && timeLeft < destructionDuration)
        {
            __instance.fxControl.StopAndDestroy(1, 2);
            __instance.fxControl.Play(2);
            __instance.fxIsPlaying = false;
        }
        return false; //Full override!
    }
    [HarmonyPatch(nameof(Flare.OnRightHandDown))]
    [HarmonyPostfix]
    static void OnRightHandDown_Postfix(Flare __instance)
    {
        RightPressed = true;
    }
    [HarmonyPatch(typeof(PlayerTool))]
    [HarmonyPatch(nameof(PlayerTool.OnRightHandUp))]
    [HarmonyPostfix]
    static void OnRightHandUp_Postfix()
    {
        RightPressed = false;
    }
    [HarmonyPatch(nameof(Flare.Throw))]
    [HarmonyPrefix]
    static bool Throw_Prefix(Flare __instance)
    {
#if DEBUG
        Dbg.Print("Throw");
        Dbg.Bool("RightPressed", RightPressed);
#endif
        if (RightPressed)
        {
            if (__instance.fxControl && !__instance.fxIsPlaying)
            {
                __instance.fxControl.Play(1);
                __instance.fxIsPlaying = true;
            }
            __instance._isInUse = false;
            __instance.isThrowing = false;
            return false; //Full override!
        }
        return true;
    }
    private static bool previousHasBeenThrown = false;
    [HarmonyPatch(nameof(Flare.OnDraw))]
    [HarmonyPrefix]
    static void OnDraw_Prefix(Flare __instance)
    {
        if (__instance.energyLeft == 0)
        {
#if DEBUG
            Dbg.Print("OnDraw");
            Dbg.Bool("hasBeenThrown", __instance.hasBeenThrown);
#endif
            previousHasBeenThrown = __instance.hasBeenThrown;
            __instance.hasBeenThrown = false;
        }
    }
    [HarmonyPatch(nameof(Flare.OnDraw))]
    [HarmonyPostfix]
    static void OnDraw_Postfix(Flare __instance)
    {
        if (__instance.energyLeft == 0)
        {
            __instance.hasBeenThrown = previousHasBeenThrown;
        }
    }
    private static bool previousIsThrowing = false;
    [HarmonyPatch(nameof(Flare.OnHolster))]
    [HarmonyPrefix]
    static void OnHolster_Prefix(Flare __instance)
    {
        if (__instance.energyLeft == 0)
        {
#if DEBUG
            Dbg.Print("OnHolster");
            Dbg.Bool("isThrowing", __instance.isThrowing);
#endif
            previousIsThrowing = __instance.isThrowing;
            __instance.isThrowing = true;
        }
    }
    [HarmonyPatch(nameof(Flare.OnHolster))]
    [HarmonyPostfix]
    static void OnHolster_Postfix(Flare __instance)
    {
        if (__instance.energyLeft == 0)
        {
            __instance.isThrowing = previousIsThrowing;
        }
        else if (__instance.fxControl)
        {
            __instance.fxControl.StopAndDestroy(2, 0f);
        }
    }
}