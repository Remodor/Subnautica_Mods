using Harmony;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace Rm_VehicleLightsImproved
{
    internal static class EnergyInfo
    {
        internal static Text energyDisplayText;
        internal static float energyConsumption;
        internal static float energyProduction;
        internal static float energyState;
        internal static float timeStamp;
    }

    [HarmonyPatch(typeof(PowerRelay))]
    [HarmonyPatch(nameof(PowerRelay.ModifyPower))]
    internal class PowerRelay_AddEnergy_Patch
    {
        static void Postfix(PowerRelay __instance, float amount)
        {
            if (Player.main.currentSub.powerRelay == __instance && Player.main.currentMountedVehicle == null)
            {
                if (amount < 0)
                {
                    EnergyInfo.energyConsumption += amount;
                }
                else
                {
                    EnergyInfo.energyProduction += amount;
                }
            }
        }
    }
    [HarmonyPatch(typeof(EnergyInterface))]
    [HarmonyPatch(nameof(EnergyInterface.ModifyCharge))]
    internal class EnergyInterface_ModifyCharge_Patch
    {
        static void Postfix(EnergyInterface __instance, float amount)
        {
            if (Player.main.currentMountedVehicle.energyInterface == __instance && Player.main.currentSub == null)
            {
                if (amount < 0)
                {
                    EnergyInfo.energyConsumption += amount;
                }
                else
                {
                    EnergyInfo.energyProduction += amount;
                }
            }
        }
    }
}
