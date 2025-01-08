using HarmonyLib;

namespace Rm_EnergyInfo;

[HarmonyPatch(typeof(Vehicle))]
class VehiclePatches
{
    [HarmonyPatch(nameof(Vehicle.EnterVehicle))]
    [HarmonyPostfix]
    static void EnterVehicle_Postfix(Vehicle __instance)
    {
#if DEBUG
        Dbg.Print("Vehicle.EnterVehicle");
#endif
        if (Config.EnableVehicle_)
        {
            PlayerPatches.Reset();
            PlayerPatches.CurrentVehicleEnergyInterface = __instance.energyInterface;
        }
    }
    [HarmonyPatch(nameof(Vehicle.SetPlayerInside))]
    [HarmonyPostfix]
    static void SetPlayerInside_Postfix(Vehicle __instance)
    {
#if DEBUG
        Dbg.Print("Vehicle.SetPlayerInside");
#endif
        if (Config.EnableVehicle_)
        {
            PlayerPatches.Reset();
            PlayerPatches.CurrentVehicleEnergyInterface = __instance.energyInterface;
        }
    }
}
[HarmonyPatch(typeof(VehicleDockingBay))]
class VehicleDockingBayPatches
{
    [HarmonyPatch(nameof(VehicleDockingBay.DockVehicle))]
    [HarmonyPostfix]
    static void DockVehicle_Postfix()
    {
#if DEBUG
        Dbg.Print("VehicleDockingBay.DockVehicle");
#endif
        PlayerPatches.CurrentVehicleEnergyInterface = null;
    }
}