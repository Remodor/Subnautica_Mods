using Nautilus.Handlers;
using Nautilus.Json;
using Nautilus.Options.Attributes;

namespace Rm_EnergyInfo;
[Menu("Energy Info Settings")]
public class Config : ConfigFile
{
    //REMOVE 
    [Slider("Example Slider", 0.0f, 1.0f, DefaultValue = 0.5f, Format = "{0:F2}", Step = 0.01f, Tooltip = "This is an example slider. It does..")]
    public float ExampleSlider = 0.5f;

    //REMOVE 
    [Toggle("Example Bool", Tooltip = "This is an example slider.")]
    public bool ExampleBool = true;

    private static Config Instance;
    public Config() : base() { }
    public static void RegisterConfig()
    {
        Instance = OptionsPanelHandler.RegisterModOptions<Config>();
    }
    //REMOVE 
    public static float ExampleSlider_() => Instance.ExampleSlider * 1200;
    //REMOVE 
    public static bool ExampleBool_() => Instance.ExampleBool;
}