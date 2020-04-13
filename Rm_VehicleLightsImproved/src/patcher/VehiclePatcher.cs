using Harmony;
using System;
using UnityEngine;

namespace Rm_VehicleLightsImproved.src.patcher
{
    [HarmonyPatch(typeof(Vehicle))]
    [HarmonyPatch(nameof(Vehicle.ReplenishOxygen))]
    internal class Vehicle_ReplenishOxygen_Patch
    {
        [HarmonyPrefix]
        internal static bool Prefix(Vehicle __instance)
        {
            if (__instance.turnedOn && __instance.replenishesOxygen && __instance.GetPilotingMode() && __instance.CanPilot())
            {
                OxygenManager oxygenMgr = Player.main.oxygenMgr;
                float available;
                float capacity;
                oxygenMgr.GetTotal(out available, out capacity);
                float amount = Mathf.Min(capacity - available, __instance.oxygenPerSecond * Time.deltaTime) * __instance.oxygenEnergyCost;
                float secondsToAdd = __instance.energyInterface.ConsumeEnergy(amount) / __instance.oxygenEnergyCost;
                oxygenMgr.AddOxygen(secondsToAdd);
                Console.WriteLine("#50 availableOX: {0}, capacityOX: {1}, energyAmount: {2}, secondsToAdd: {3}, energyCost: {4}", available, capacity, amount, secondsToAdd, __instance.oxygenEnergyCost);
            }
            return false;
        }
    }
}
