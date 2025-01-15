using Nautilus.Handlers;
using Nautilus.Json;
using Nautilus.Options.Attributes;
using UnityEngine;

namespace Rm_BetterFastCraft;

public enum CraftingModifierMode { Relative, Total }
[Menu("Better Fast Craft Settings")]
public class Config : ConfigFile
{
    [Choice("Crafting Modifier Mode", Tooltip = "'Relative': Multiplies the crafting time by this value. 'Total': Replaces the crafting time by this value."), OnChange("OnConfigChange")]
    public CraftingModifierMode CraftingModifierMode = Rm_BetterFastCraft.CraftingModifierMode.Relative;

    [Slider("Crafting Modifier", 0.0f, 5.0f, DefaultValue = 1f, Format = "{0:F2}", Step = 0.05f, Tooltip = "The crafting time modifier. When using 'Relative', the crafting time is multiplied by this value (e.g. 2s crafting time and 0.5 modifier -> 1s). 'Total' uses the crafting modifier as the crafting time (e.g. 2s crafting time and 0.5 modifier -> 0.5s)."), OnChange("OnConfigChange")]
    public float CraftingModifier = 1f;

    [Slider("Minimum Duration", 0.05f, 5.0f, DefaultValue = 2.7f, Format = "{0:F2}", Step = 0.05f, Tooltip = "When using 'Relative', this is the minimum crafting duration (e.g. 2s crafting time, 0.5 modifier and 1.5 minimum duration -> 1.5s)."), OnChange("OnConfigChange")]
    public float MinimumDuration = 2.7f;

    [Toggle("Creative Mode Instant Craft", Tooltip = "Enabling this option overrides the crafting time to be nearly instant when using the creative mode."), OnChange("OnConfigChange")]
    public bool CreativeInstantCraft = true;

    private static Config Instance;
    public Config() : base() { }
    public static void RegisterConfig()
    {
        Instance = OptionsPanelHandler.RegisterModOptions<Config>();
        OnConfigChange();
    }
    public static void OnConfigChange()
    {
        CraftingModifierMode_ = Instance.CraftingModifierMode;
        if (CraftingModifierMode_ == CraftingModifierMode.Total)
        {
            CraftingModifier_ = Mathf.Max(Instance.CraftingModifier, 0.05f);
        }
        else
        {
            CraftingModifier_ = Instance.CraftingModifier;
        }
        MinimumDuration_ = Mathf.Max(Instance.MinimumDuration, 0.05f);
        CreativeInstantCraft_ = Instance.CreativeInstantCraft;
    }
    internal static CraftingModifierMode CraftingModifierMode_;
    internal static float CraftingModifier_;
    internal static float MinimumDuration_;
    internal static bool CreativeInstantCraft_;
}