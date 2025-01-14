using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace Rm_PowerModifier;

[HarmonyPatch(typeof(SolarPanel))]
class SolarPanelPatches
{
    [HarmonyPatch(nameof(SolarPanel.Start))]
    [HarmonyPostfix]
    static void Start_Postfix(SolarPanel __instance)
    {
        __instance.powerSource.maxPower = Config.SolarPanelMaxPower_;
    }

    [HarmonyPatch(nameof(SolarPanel.Update))]
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Update_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var found = false;
        var codeInstructions = new List<CodeInstruction>(instructions);
        for (int i = 0; i < codeInstructions.Count; i++)
        {
            if (i >= 4 && codeInstructions[i - 4].Is(OpCodes.Ldc_R4, 0.25f) && codeInstructions[i - 3].opcode == OpCodes.Mul && codeInstructions[i - 2].Is(OpCodes.Ldc_R4, 5.0f) && codeInstructions[i - 1].opcode == OpCodes.Mul)
            {
                found = true;
#if DEBUG
                Dbg.Print("Transpiler Found: * 0.25f * 5f");
#endif
                yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(Config), nameof(Config.SolarPanelPowerModifier_)));
                yield return new CodeInstruction(OpCodes.Mul);
            }
            yield return codeInstructions[i];
        }
        if (!found) Plugin.Logger.LogError("Transpiler: Could not patch SolarPanel.Update");

    }
}