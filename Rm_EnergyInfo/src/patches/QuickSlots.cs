using HarmonyLib;

namespace Rm_EnergyInfo;

[HarmonyPatch(typeof(QuickSlots))]
class QuickSlotsPatches
{
    [HarmonyPatch(nameof(QuickSlots.SelectInternal))]
    [HarmonyPostfix]
    static void SelectInternal_Postfix(QuickSlots __instance)
    {
#if DEBUG
        Dbg.Print("QuickSlots.SelectInternal");
#endif
        if (Config.EnableTools_ && PlayerPatches.CurrentSubPowerRelay is null && __instance._heldItem is not null)
        {
            PlayerTool playerTool = __instance._heldItem.item.GetComponent<PlayerTool>();
            if (playerTool is not null)
            {
                PlayerPatches.Reset();
                PlayerPatches.CurrentToolEnergyMixin = playerTool.energyMixin;
            }
        }
    }
    [HarmonyPatch(nameof(QuickSlots.DeselectInternal))]
    [HarmonyPostfix]
    static void DeselectInternal_Postfix()
    {
#if DEBUG
        Dbg.Print("QuickSlots.DeselectInternal");
#endif
        PlayerPatches.CurrentToolEnergyMixin = null;
    }
}