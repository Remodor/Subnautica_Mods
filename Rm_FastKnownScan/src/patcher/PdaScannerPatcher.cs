using Harmony;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;

namespace Rm_FastKnownScan
{
    [HarmonyPatch(typeof(PDAScanner))]
    [HarmonyPatch(nameof(PDAScanner.Scan))]
    internal class FastRedundantScan
    {
        private static float knownScanTime = 2f;

        public static void SetKnownScanTime(float scanTime) {
            knownScanTime = scanTime;
        }
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var methodContains = SymbolExtensions.GetMethodInfo(() => PDAScanner.complete.Contains(TechType.None));
            object operandContains = null;
            var instructionList = new List<CodeInstruction>(instructions);
            for (int i = 0; i < instructionList.Count - 2; i++)
            {
                if (operandContains == null
                    && instructionList[i].opcode == OpCodes.Callvirt
                    && instructionList[i].operand as MethodInfo == methodContains
                    && instructionList[i + 1].opcode == OpCodes.Stloc_S)
                {
                    operandContains = instructionList[i + 1].operand;
                }
                else if (instructionList[i].opcode == OpCodes.Ldloc_S
                    && instructionList[i].operand == operandContains 
                    && instructionList[i + 1].opcode == OpCodes.Brfalse
                    && instructionList[i + 2].opcode == OpCodes.Ldc_R4)
                {
                    instructionList[i + 2].operand = knownScanTime;
                    Console.WriteLine("[Rm_FastKnownScan] Changed scan speed to: {0}", knownScanTime);
                    break;
                }
            }
            return instructionList;
        }
    }
}
