using HarmonyLib;
using UnityEngine;

namespace Rm_BetterFastCraft;

[HarmonyPatch(typeof(Crafter))]
class CrafterPatches
{
    [HarmonyPatch(nameof(Crafter.Craft))]
    [HarmonyPrefix]
    static void Craft_Prefix(ref float duration)
    {
#if DEBUG
        Dbg.Float("Duration before: ", duration);
#endif
        if (Config.CreativeInstantCraft_ && GameModeUtils.currentGameMode == GameModeOption.Creative)
        {
#if DEBUG
            Dbg.Print("Creative");
#endif
            duration = 0.05f;
        }
        else if (Config.CraftingModifierMode_ == CraftingModifierMode.Relative)
        {
#if DEBUG
            Dbg.Print("Relative");
#endif
            duration *= Config.CraftingModifier_;
            duration = Mathf.Max(duration, Config.MinimumDuration_);
        }
        else
        {
#if DEBUG
            Dbg.Print("Absolute");
#endif
            duration = Config.CraftingModifier_;
        }
#if DEBUG
        Dbg.Float("Duration after: ", duration);
#endif
    }
}