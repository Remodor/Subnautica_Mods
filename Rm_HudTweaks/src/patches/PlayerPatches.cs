using HarmonyLib;

namespace Rm_HudTweaks;
[HarmonyPatch(typeof(Player))]
class PlayerPatches
{
    [HarmonyPatch(nameof(Player.EnterPilotingMode))]
    [HarmonyPostfix]
    static void EnterPilotingMode_Postfix()
    {
        if (Config.CyclopsCrosshairAlways_)
        {
            HandReticlePatches.ShowCyclopsCrosshair = true;
        }
    }
    [HarmonyPatch(nameof(Player.ExitPilotingMode))]
    [HarmonyPostfix]
    static void ExitPilotingMode_Postfix()
    {
        HandReticlePatches.ShowCyclopsCrosshair = false;
    }
    [HarmonyPatch(nameof(Player.SetScubaMaskActive))]
    [HarmonyPrefix]
    static void SetScubaMaskActive_Prefix(ref bool state)
    {
        if (Config.HideScubaMask_)
            state = false;
    }
}