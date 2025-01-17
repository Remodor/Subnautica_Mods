using Nautilus.Handlers;
using Nautilus.Json;
using Nautilus.Options.Attributes;
using UnityEngine;

namespace Rm_FastBuild;

public enum ModifierMode { Linear, Progressive }
[Menu("Fast Build Settings")]
public class Config : ConfigFile
{
    [Choice("Construction Modifier Mode", Tooltip = "'Linear': Uses a linear function to calculate the construction time. 'Progressive': Uses a logistic function to calculate the construction time (every additional resource adds less time than the previous). More details on the mod page."), OnChange("OnConfigChange")]
    public ModifierMode ModifierMode = ModifierMode.Linear;

    [Slider("Construction Modifier", 0.0f, 5.0f, DefaultValue = 1.0f, Format = "{0:F2}", Step = 0.05f, Tooltip = "The construction time modifier. 'Linear': the resource amount is multiplied by the construction modifier (e.g. 3 resources and 1.5 modifier -> 4.5s crafting time). 'Progressive': the construction modifier is the logistic growth rate, the steepness of the curve (a good value is around 0.4)."), OnChange("OnConfigChange")]
    public float ConstructionModifier = 1.0f;

    [Slider("Maximum Build Time", 0.1f, 20.0f, DefaultValue = 6.0f, Format = "{0:F1}", Step = 0.1f, Tooltip = "The upper bound for both calculation modes. The total construction time will never surpass this value."), OnChange("OnConfigChange")]
    public float MaximumBuildTime = 6.0f;

    [Slider("Minimum Build Time", 0.1f, 20.0f, DefaultValue = 2.0f, Format = "{0:F1}", Step = 0.1f, Tooltip = "The lower bound for both calculation modes. The construction time will never fall below this value. Is internally restricted by 'Maximum Build Time'."), OnChange("OnConfigChange")]
    public float MinimumBuildTime = 2.0f;

    [Toggle("Creative Instant Build", Tooltip = "Enabling this option overrides the building time to be nearly instant when using the creative mode."), OnChange("OnConfigChange")]
    public bool CreativeInstantBuild = true;

    private static Config Instance;
    public Config() : base() { }
    public static void RegisterConfig()
    {
        Instance = OptionsPanelHandler.RegisterModOptions<Config>();
        OnConfigChange();
    }
    public static void OnConfigChange()
    {
        ModifierMode_ = Instance.ModifierMode;
        ConstructionModifier_ = Instance.ConstructionModifier;
        MaximumBuildTime_ = Instance.MaximumBuildTime;
        MinimumBuildTime_ = Mathf.Min(Instance.MinimumBuildTime, Instance.MaximumBuildTime);
#if DEBUG
        Dbg.FloatChanged("MaximumBuildTime", MaximumBuildTime_, 1);
        Dbg.FloatChanged("MinimumBuildTime", MinimumBuildTime_, 1);
#endif
        CreativeInstantBuild_ = Instance.CreativeInstantBuild;
    }
    internal static ModifierMode ModifierMode_;
    internal static float ConstructionModifier_;
    internal static float MaximumBuildTime_;
    internal static float MinimumBuildTime_;
    internal static bool CreativeInstantBuild_;

}