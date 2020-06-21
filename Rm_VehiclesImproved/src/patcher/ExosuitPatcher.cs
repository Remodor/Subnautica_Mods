using Harmony;
using System;
using System.Text;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;

namespace Rm_VehiclesImproved
{
    [HarmonyPatch(typeof(Exosuit))]
    [HarmonyPatch(nameof(Exosuit.UpdateUIText))]
    internal class Exosuit_UpdateUIText_Patch
    {
        // Removes the annoying "Exit" overlay.
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var methodAppendLine = typeof(StringBuilder).GetMethod(nameof(StringBuilder.AppendLine), new Type[] { typeof(string) });
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
                        if (instruction.opcode == OpCodes.Callvirt && instruction.operand as MethodInfo == methodAppendLine)
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
