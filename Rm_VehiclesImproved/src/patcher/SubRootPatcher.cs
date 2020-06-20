using Harmony;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Rm_VehiclesImproved
{
    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch(nameof(SubRoot.Start))]
    internal class SubRoot_Start_Patch
    {
        static void Postfix(SubRoot __instance)
        {
            __instance.silentRunningPowerCost = 0;
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
