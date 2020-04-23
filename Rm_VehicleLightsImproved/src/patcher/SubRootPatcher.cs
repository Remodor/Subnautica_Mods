using Harmony;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Rm_VehicleLightsImproved
{
    internal static class SubRootSettings
    {
        internal static float defaultLightEnergyConsumption = 0f;
        internal static float emergencyLightEnergyConsumption = 0f;

        internal static bool autoLightDim = true;
        internal static bool includeBaseLights = true;
        internal static bool debugDailyEnergyExtrapolation = true;   
    }
    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch(nameof(SubRoot.Start))]
    internal class SubRoot_Start_Patch
    {
        static void Postfix(SubRoot __instance)
        {
            __instance.silentRunningPowerCost = 0;
            __instance.lightControl.fadeDuration = 1.5f;
        }
    }
    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch(nameof(SubRoot.UpdateLighting))]
    internal class SubRoot_UpdateLighting_Patch
    {
        static void Prefix(SubRoot __instance, out bool __state)
        {
            __state = __instance.silentRunning;
            if (__instance.isCyclops || SubRootSettings.includeBaseLights)
            {
                if ((SubRootSettings.autoLightDim && Player.main.currentSub != __instance) || __instance.powerRelay.GetPowerStatus() == PowerSystem.Status.Emergency)
                {
                    __instance.silentRunning = true;
                }
            }
        }
        static void Postfix(SubRoot __instance, bool __state)
        {
            if (__instance.isCyclops || SubRootSettings.includeBaseLights)
            {
                __instance.silentRunning = __state;
                if (__instance.lightingState == 1)
                {
                    float energyCost = DayNightCycle.main.deltaTime * SubRootSettings.emergencyLightEnergyConsumption;
                    __instance.powerRelay.ConsumeEnergy(energyCost, out _);
                }
                else if (__instance.lightingState == 0)
                {
                    float energyCost = DayNightCycle.main.deltaTime * SubRootSettings.defaultLightEnergyConsumption;
                    __instance.powerRelay.ConsumeEnergy(energyCost, out _);
                }
            }
        }
    }
    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch(nameof(SubRoot.Update))]
    internal class SubRoot_Update_Patch
    {
        //Fixes methodUpdateLighting and methodUpdateThermalReactorCharge are not called if SubRoot further away than ~60m
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var methodUpdateLighting = typeof(SubRoot).GetMethod(nameof(SubRoot.UpdateLighting), BindingFlags.NonPublic | BindingFlags.Instance);
            var methodUpdateThermalReactorCharge = typeof(SubRoot).GetMethod(nameof(SubRoot.UpdateThermalReactorCharge), BindingFlags.NonPublic | BindingFlags.Instance);

            int state = 0;
            var newInstruction = new CodeInstruction(OpCodes.Ldarg_0);
            yield return newInstruction; 
            newInstruction = new CodeInstruction(OpCodes.Call, methodUpdateLighting);
            yield return newInstruction;
            newInstruction = new CodeInstruction(OpCodes.Ldarg_0);
            yield return newInstruction;
            newInstruction = new CodeInstruction(OpCodes.Call, methodUpdateThermalReactorCharge);
            yield return newInstruction;
            foreach (CodeInstruction instruction in instructions)
            {
                switch (state)
                {
                    case 0:
                        if (instruction.opcode == OpCodes.Call && instruction.operand as MethodInfo == methodUpdateLighting)
                        {
                            state++;
                            continue;
                        }
                        break;
                    case 1:
                        state++;
                        continue;
                    case 2:
                        if (instruction.opcode == OpCodes.Call && instruction.operand as MethodInfo == methodUpdateThermalReactorCharge)
                        {
                            state++;
                            continue;
                        }
                        break;
                    case 3:
                        state++;
                        continue;
                }
                yield return instruction;
            }
        }
    }
}
