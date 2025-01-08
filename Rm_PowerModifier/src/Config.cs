using Nautilus.Handlers;
using Nautilus.Json;
using Nautilus.Options.Attributes;

namespace Rm_PowerModifier;
[Menu("Power Modifier Settings")]
public class Config : ConfigFile
{
    //REMOVE 
    [Slider("Example Slider", 0.0f, 1.0f, DefaultValue = 0.5f, Format = "{0:F2}", Step = 0.01f, Tooltip = "This is an example slider. It does.."), OnChange("OnConfigChange")]
    public float ExampleSlider = 0.5f;

    //REMOVE 
    [Toggle("Example Bool", Tooltip = "This is an example slider."), OnChange("OnConfigChange")]
    public bool ExampleBool = true;

    private static Config Instance;
    public Config() : base() { }
    public static void RegisterConfig()
    {
        Instance = OptionsPanelHandler.RegisterModOptions<Config>();
        OnConfigChange();
    }
    public static void OnConfigChange()
    {
        ExampleSlider_ = Instance.ExampleSlider * 1200;
        ExampleBool_ = Instance.ExampleBool;
    }
    //REMOVE 
    internal static float ExampleSlider_;
    //REMOVE 
    internal static bool ExampleBool;
}