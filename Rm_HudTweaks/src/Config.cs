using Nautilus.Handlers;
using Nautilus.Json;
using Nautilus.Options.Attributes;

namespace Rm_HudTweaks;
public enum CyclopsCrosshair { Hidden, Always, OnlyOnHover }
[Menu("Hud Tweak Settings")]
public class Config : ConfigFile
{
    [Toggle("Hide Names", Tooltip = "Hide the entity name text. Like 'Peeper', when you aim at a peeper."), OnChange("OnConfigChange")]
    public bool HideNames = true;

    [Toggle("Hide Text 2", Tooltip = "Hide additional texts, like the 'Deconstruct'-text when using the builder or the 'empty'-text when aiming at a locker."), OnChange("OnConfigChange")]
    public bool HideText2 = true;

    [Toggle("Hide Input Icon", Tooltip = "Hide the input icon, e.g. (<color=#ADF8FFFF></color>) when opening a locker."), OnChange("OnConfigChange")]
    public bool HideInputIcon = true;

    [Toggle("Hide Tool Controls", Tooltip = "Hide tool controls. Also affects Seamoth and Prawn Suit texts."), OnChange("OnConfigChange")]
    public bool HideToolControlsText = true;

    [Toggle("Hide Tool Info", Tooltip = "Hide tool infos, like the battery status. Also affects Seamoth and Prawn Suit texts."), OnChange("OnConfigChange")]
    public bool HideToolInfo = true;

    [Slider("Crosshair Scale", 0.01f, 2.0f, DefaultValue = 1.0f, Format = "{0:F2}", Step = 0.01f, Tooltip = "Change the crosshair scale."), OnChange("OnConfigChange")]
    public float CrosshairScale = 1.0f;

    [Toggle("Hide Crosshair", Tooltip = "Hide the default crosshair. Other interact icons are not affected."), OnChange("OnConfigChange")]
    public bool HideCrosshair = true;

    [Toggle("Hide Scuba Mask", Tooltip = "Hide the scuba mask when diving. Applies when entering the water."), OnChange("OnConfigChange")]
    public bool HideScubaMask = false;

    [Toggle("Hide Interact Icons", Tooltip = "Hide the interact icons, e.g. when entering a sub. Depending on the 'Hide Crosshair'-setting, either the default crosshair is shown or nothing if it is hidden."), OnChange("OnConfigChange")]
    public bool HideInteractIcons = true;

    [Choice("Cyclops Crosshair", Tooltip = "Show a crosshair when piloting the cyclops. Overrides a hidden crosshair. 'OnlyOnHover': Only shows the crosshair when hovering cyclops control elements.\n(Has additional options: 'Crosshair Icon', 'Speed Linger Time')"), OnChange("OnConfigChange")]
    public CyclopsCrosshair CyclopsCrosshair = CyclopsCrosshair.OnlyOnHover;

    [Slider("    Crosshair Icon", 1, 10, DefaultValue = 1, Tooltip = "Change the cyclops crosshair icon."), OnChange("OnConfigChange")]
    public int CyclopsCrosshairIcon = 1;

    [Slider("    Speed Linger Time", 0.1f, 3.0f, DefaultValue = 0.5f, Format = "{0:F1}", Step = 0.1f, Tooltip = "The cyclops speed buttons required a workaround - the crosshair lingers instead of the on-and-of behavior of the other buttons. This setting controls the linger time in seconds."), OnChange("OnConfigChange")]
    public float CyclopsSpeedLingerTime = 0.5f;

    private static Config Instance;
    public Config() : base() { }
    public static void RegisterConfig()
    {
        Instance = OptionsPanelHandler.RegisterModOptions<Config>();
        OnConfigChange();
    }
    public static void OnConfigChange()
    {
        HideNames_ = Instance.HideNames;
        HideText2_ = Instance.HideText2;
        HideToolControlsText_ = Instance.HideToolControlsText;
        HideToolInfo_ = Instance.HideToolInfo;
        HideInputIcon_ = Instance.HideInputIcon;
        CrosshairScale_ = Instance.CrosshairScale;
        HideCrosshair_ = Instance.HideCrosshair;
        HideInteractIcons_ = Instance.HideInteractIcons;
        CyclopsCrosshairAlways_ = Instance.CyclopsCrosshair == CyclopsCrosshair.Always;
        CyclopsCrosshairOnlyOnHover_ = Instance.CyclopsCrosshair == CyclopsCrosshair.OnlyOnHover;
        CyclopsCrosshairIcon_ = Instance.CyclopsCrosshairIcon;
        CyclopsSpeedLingerTime_ = Instance.CyclopsSpeedLingerTime;
        HideScubaMask_ = Instance.HideScubaMask;
    }

    internal static bool HideNames_;
    internal static bool HideText2_;
    internal static bool HideToolControlsText_;
    internal static bool HideToolInfo_;
    internal static bool HideInputIcon_;
    internal static float CrosshairScale_;
    internal static bool HideCrosshair_;
    internal static bool HideInteractIcons_;
    internal static bool CyclopsCrosshairAlways_;
    internal static bool CyclopsCrosshairOnlyOnHover_;
    internal static int CyclopsCrosshairIcon_;
    internal static float CyclopsSpeedLingerTime_;
    internal static bool HideScubaMask_;
}