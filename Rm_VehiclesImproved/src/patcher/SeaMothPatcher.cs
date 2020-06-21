using Harmony;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using System;

namespace Rm_VehiclesImproved.src.patcher
{
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
