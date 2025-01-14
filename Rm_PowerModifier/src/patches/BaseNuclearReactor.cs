using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace Rm_PowerModifier;

[HarmonyPatch(typeof(BaseNuclearReactor))]
class BaseNuclearReactorPatches
{
    [HarmonyPatch(nameof(BaseNuclearReactor.Start))]
    [HarmonyPostfix]
    static void Start_Postfix(BaseNuclearReactor __instance)
    {
        __instance._powerSource.maxPower = Config.NuclearReactorMaxPower_;
    }
    [HarmonyPatch(nameof(BaseNuclearReactor.Update))]
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Update_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var found = false;
        var codeInstructions = new List<CodeInstruction>(instructions);
        for (int i = 0; i < codeInstructions.Count; i++)
        {
            if (i >= 1 && codeInstructions[i - 1].Is(OpCodes.Ldc_R4, 4.1666665f))
            {
                found = true;
#if DEBUG
                Dbg.Print("Transpiler Found: 4.1666665f");
#endif
                yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(Config), nameof(Config.NuclearReactorPowerModifier_)));
                yield return new CodeInstruction(OpCodes.Mul);
            }
            yield return codeInstructions[i];
        }
        if (!found) Plugin.Logger.LogError("Transpiler: Could not patch BaseNuclearReactor.Update");
    }
}