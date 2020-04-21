using Harmony;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using System;

namespace Rm_VehicleLightsImproved
{
    internal static class SeaMothSettings
    {
        internal static float lightEnergyConsumption = 0f;
    }
    [HarmonyPatch(typeof(SeaMoth))]
    [HarmonyPatch(nameof(SeaMoth.Start))]
    internal class SeaMoth_Start_Patch
    {
        [HarmonyPostfix]
        static void Postfix(SeaMoth __instance)
        {
            __instance.toggleLights.energyPerSecond = SeaMothSettings.lightEnergyConsumption;
            __instance.toggleLights.SetLightsActive(false);
        }
    }
    [HarmonyPatch(typeof(SeaMoth))]
    [HarmonyPatch(nameof(SeaMoth.OnDockedChanged))]
    internal class SeaMoth_OnDockedChanged_Patch
    {
        [HarmonyPostfix]
        static void Postfix(SeaMoth __instance, bool docked)
        {
            if (docked)
            {
                __instance.toggleLights.SetLightsActive(false);
            }
        }
    }
    [HarmonyPatch(typeof(SeaMoth))]
    [HarmonyPatch(nameof(SeaMoth.SubConstructionComplete))]
    internal class SeaMoth_SubConstructionComplete_Patch
    {
        static void Postfix(SeaMoth __instance)
        {
            __instance.lightsParent.SetActive(false);
        }
    }
    [HarmonyPatch(typeof(SeaMoth))]
    [HarmonyPatch(nameof(SeaMoth.Update))]
    internal class SeaMoth_Update_Patch
    {
        // Removes the annoying "Exit" overlay.
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var methodSetUseTextRaw = typeof(HandReticle).GetMethod(nameof(HandReticle.SetUseTextRaw), new Type[] { typeof(string), typeof(string) });
            int state = 0;
            foreach (CodeInstruction instruction in instructions)
            {
                switch (state)
                {
                    case 0:
                        if (instruction.operand as String == "PressToExit")
                        {
                            state++;
                            continue;
                        }
                        break;
                    case 1:
                        if (instruction.opcode == OpCodes.Callvirt && instruction.operand as MethodInfo == methodSetUseTextRaw)
                        {
                            state++;
                        }
                        continue;
                }
                yield return instruction;
            }
        }
    }
}
