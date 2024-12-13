using Nautilus.Handlers;
using Nautilus.Json;
using Nautilus.Options.Attributes;

namespace Rm_FlareRepair;
[Menu("Flare Repair Settings")]
public class Config : ConfigFile
{
    [Slider("Total Duration (Game Days)", 0.01f, 2.0f, DefaultValue = 0.5f, Format = "{0:F2}", Step = 0.01f)]
    public float TotalDuration = 0.5f;

    [Slider("Intensity Modifier", 0.01f, 2.0f, DefaultValue = 1.0f, Format = "{0:F2}", Step = 0.01f)]
    public float IntensityModifier = 1.0f;

    [Slider("Progressive Dimming Threshold", 0.0f, 1.0f, DefaultValue = 0.5f, Format = "{0:F2}", Step = 0.01f, Tooltip = "Setting this value >0 will progressively dim the flare intensity starting at the set threshold and used 'Intensity Modifier'. For example, a value of 0.5 will start dimming the flare when 50% of the time is left.")]
    public float ProgressiveDimmingThreshold = 0.5f;

    [Slider("Minimum Relative Factor", 0.0f, 0.99f, DefaultValue = 0.2f, Format = "{0:F2}", Step = 0.01f, Tooltip = "Only enabled if 'Progressive Dimming Threshold' is >0. This value sets the reached minimum intensity when the flare is depleted and progressive dimming is used. For example, a value of 0.25 will result in a flare intensity of 25% of the 'Intensity Modifier' at the end of the flare's lifetime.")]
    public float MinimumRelativeFactor = 0.2f;

    [Toggle("Keep Depleted Flares", Tooltip = "Enabling this option keeps depleted flares. They will persist when saving. Disabling this option will remove all kept flares as long as they are currently loaded.")]
    public bool KeepDepletedFlare = true;

    private static Config Instance;
    public Config() : base() { }
    public static void RegisterConfig()
    {
        Instance = OptionsPanelHandler.RegisterModOptions<Config>();
    }
    public static float TotalDuration_ => Instance.TotalDuration * 1200;
    public static float IntensityModifier_ => Instance.IntensityModifier;
    public static bool KeepDepletedFlare_ => Instance.KeepDepletedFlare;
    public static float ProgressiveDimmingThreshold_ => Instance.ProgressiveDimmingThreshold;
    public static float MinimumRelativeFactor_ => Instance.MinimumRelativeFactor;
}