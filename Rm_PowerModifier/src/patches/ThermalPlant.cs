using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace Rm_PowerModifier;

[HarmonyPatch(typeof(ThermalPlant))]
class ThermalPlantPatches
{
    [HarmonyPatch(nameof(ThermalPlant.Start))]
    [HarmonyPostfix]
    static void Start_Postfix(ThermalPlant __instance)
    {
        __instance.powerSource.maxPower = Config.ThermalPlantMaxPower_;
    }

    [HarmonyPatch(nameof(ThermalPlant.AddPower))]
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> AddPower_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var found = false;
        var codeInstructions = new List<CodeInstruction>(instructions);
        for (int i = 0; i < codeInstructions.Count; i++)
        {
            if (i >= 3 && codeInstructions[i - 3].Is(OpCodes.Ldc_R4, 1.6500001f) && codeInstructions[i - 2].opcode == OpCodes.Ldloc_0 && codeInstructions[i - 1].opcode == OpCodes.Mul)
            {
                found = true;
#if DEBUG
                Dbg.Print("Transpiler Found: 1.6500001f * num");
#endif
                yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(Config), nameof(Config.ThermalPlantPowerModifier_)));
                yield return new CodeInstruction(OpCodes.Mul);
            }
            yield return codeInstructions[i];
        }
        if (!found) Plugin.Logger.LogError("Transpiler: Could not patch ThermalPlant.AddPower");
    }
}