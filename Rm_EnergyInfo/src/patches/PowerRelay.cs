using HarmonyLib;

namespace Rm_EnergyInfo;

[HarmonyPatch(typeof(PowerRelay))]
class PowerRelayPatches
{
    [HarmonyPatch(nameof(PowerRelay.Start))]
    [HarmonyPostfix]
    static void Start_Postfix()
    {
#if DEBUG
        Dbg.Print("PowerRelay.Start");
#endif
        if (Config.EnableSub_ && Player.main._currentSub is not null)
        {
            PlayerPatches.CurrentSubPowerRelay = Player.main._currentSub.powerRelay;
            PlayerPatches.Reset();
        }
    }
}