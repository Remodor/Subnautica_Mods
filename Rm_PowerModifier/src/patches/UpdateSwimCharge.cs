using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace Rm_PowerModifier;

[HarmonyPatch(typeof(UpdateSwimCharge))]
class UpdateSwimChargePatches
{
    [HarmonyPatch(nameof(UpdateSwimCharge.FixedUpdate))]
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> FixedUpdate_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var found = false;
        var codeInstructions = new List<CodeInstruction>(instructions);
        for (int i = 0; i < codeInstructions.Count; i++)
        {
            if (i >= 1 && codeInstructions[i - 1].Is(OpCodes.Ldfld, AccessTools.Field(typeof(UpdateSwimCharge), "chargePerSecond")))
            {
                found = true;
#if DEBUG
                Dbg.Print("Transpiler Found: UpdateSwimCharge::chargePerSecond");
#endif
                yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(Config), nameof(Config.FinChargeRateModifier_)));
                yield return new CodeInstruction(OpCodes.Mul);
            }
            yield return codeInstructions[i];
        }
        if (!found) Plugin.Logger.LogError("Transpiler: Could not patch UpdateSwimCharge.FixedUpdate");
    }
}