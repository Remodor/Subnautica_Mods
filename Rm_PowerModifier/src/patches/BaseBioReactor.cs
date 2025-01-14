using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace Rm_PowerModifier;

[HarmonyPatch(typeof(BaseBioReactor))]
class BaseBioReactorPatches
{
    [HarmonyPatch(nameof(BaseBioReactor.Start))]
    [HarmonyPostfix]
    static void Start_Postfix(BaseBioReactor __instance)
    {
        __instance._powerSource.maxPower = Config.BioReactorMaxPower_;
    }

    [HarmonyPatch(nameof(BaseBioReactor.Update))]
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Update_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var found = false;
        var codeInstructions = new List<CodeInstruction>(instructions);
        for (int i = 0; i < codeInstructions.Count; i++)
        {
            if (i >= 1 && codeInstructions[i - 1].Is(OpCodes.Ldc_R4, 0.8333333f))
            {
                found = true;
#if DEBUG
                Dbg.Print("Transpiler Found: 0.8333333f");
#endif
                yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(Config), nameof(Config.BioReactorPowerModifier_)));
                yield return new CodeInstruction(OpCodes.Mul);
            }
            yield return codeInstructions[i];
        }
        if (!found) Plugin.Logger.LogError("Transpiler: Could not patch BaseBioReactor.Update");
    }
}