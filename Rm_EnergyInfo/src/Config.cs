using Nautilus.Handlers;
using Nautilus.Json;
using Nautilus.Options.Attributes;
using UnityEngine;


namespace Rm_EnergyInfo;
[Menu("Energy Info Settings")]
public class Config : ConfigFile
{
    [Toggle("Enable", Tooltip = "Enables the energy info. Overrides 'Base/ Cyclops', 'Vehicle', and 'Tools' when disabled."), OnChange("OnConfigChange")]
    public bool Enable = true;

    [Toggle("Base/ Cyclops", Tooltip = "Show the energy info for the current base or Cyclops. Has higher priority than held tools."), OnChange("OnConfigChange")]
    public bool EnableSub = true;

    [Toggle("Vehicle", Tooltip = "Show the energy info for the current vehicle, e.g. the Prawn Suit or the Seamoth."), OnChange("OnConfigChange")]
    public bool EnableVehicle = true;

    [Toggle("Tools", Tooltip = "Show the energy info for the currently held tool. Is overridden when inside a base/ Cyclops if 'Base/ Cyclops' is enabled."), OnChange("OnConfigChange")]
    public bool EnableTools = true;

    [Slider("Sample Interval", 0.05f, 5.5f, DefaultValue = 1.0f, Format = "{0:F2}", Step = 0.05f, Tooltip = "Defines the refresh rate of the energy info. Shorter intervals mean quicker updates but increase fluctuation. Some energy consumers/ producers, like the filtration machine, or the thermal plant, draw/ produce energy in intervals. Increasing the sample interval can cover these spikes and calculate an energy consumption per ingame hour (e/h). Longer intervals can be set inside the config file."), OnChange("OnConfigChange")]
    public float SampleInterval = 1.0f;

    [Slider("Spike Threshold", 0.05f, 5.0f, DefaultValue = 0.4f, Format = "{0:F2}", Step = 0.05f, Tooltip = "Some actions draw energy on a per-use basis, like the prawn suit's jump ability consuming 1.2 energy per jump. These spikes can be shown separately without affecting the consumption-per-hour rate. The spike threshold defines what is considered a spike. Greater thresholds can be set inside the config file."), OnChange("OnConfigChange")]
    public float SpikeThreshold = 0.4f;
    [Slider("Spike Linger Duration", 0, 40, DefaultValue = 1, Tooltip = "Defines how many sample intervals an energy spike is shown (i.e. how long a spike is shown). A value of 0 hides energy spikes. Keep in mind that a new spike with the same value can not be noticed if the linger duration is set too high."), OnChange("OnConfigChange")]
    public int SpikeLingerDuration = 1;

    [Toggle("Modify Hud", Tooltip = "Enables the modification mode. Using the arrow keys moves the overlay on the hud. Using 'left-shift' + 'Up' or 'Down' increases/ decreases the overlay size. It is advisable to turn it off when finished. Additionally, the overlay can be modified using the config file."), OnChange("OnConfigChange")]
    public bool ModifyHud = false;

    [Toggle("Draw Energy Unit", Tooltip = "Draws energy units -- 'e/h' for energy per ingame hour and 'e' for energy spikes."), OnChange("OnConfigChange")]
    public bool DrawEnergyUnit = true;

    [Toggle("Inverse Energy Display", Tooltip = "The default energy displays the energy production (positive values: energy produced, negative values: energy consumed). This setting inverts the displayed energy (positive value: energy consumed, negative value: energy produced)."), OnChange("OnConfigChange")]
    public bool InverseEnergyDisplay = false;


    public Vector3 HudPosition = new(-475, 225, 0);
    public Vector2 HudSize = new(250, 125);
    public int HudFontSize = 25;

    private static Config Instance;
    public Config() : base() { }
    public static void RegisterConfig()
    {
        Instance = OptionsPanelHandler.RegisterModOptions<Config>();
        OnConfigChange();
    }
    public static void OnConfigChange()
    {
        Enable_ = Instance.Enable;
        EnableSub_ = Instance.EnableSub && Instance.Enable;
        EnableVehicle_ = Instance.EnableVehicle && Instance.Enable;
        EnableTools_ = Instance.EnableTools && Instance.Enable;
        ModifyHud_ = Instance.ModifyHud;
        SampleInterval_ = Instance.SampleInterval;
        DrawEnergyUnit_ = Instance.DrawEnergyUnit;
        SpikeThreshold_ = Instance.SpikeThreshold;
        SpikeLingerDuration_ = Instance.SpikeLingerDuration;
        InverseEnergyDisplay_ = Instance.InverseEnergyDisplay ? -1.0f : 1.0f;
        PlayerPatches.Reset();
        if (Player.main is not null)
        {
            if (!Enable_)
            {
                PlayerPatches.EnergyUiObject.SetActive(false);
            }
            if (!EnableSub_)
            {
                PlayerPatches.CurrentSubPowerRelay = null;
            }
            else if (Player.main._currentSub is not null)
            {
                PlayerPatches.CurrentSubPowerRelay = Player.main._currentSub.powerRelay;
            }
            if (!EnableVehicle_)
            {
                PlayerPatches.CurrentVehicleEnergyInterface = null;
            }
            else if (Player.main.currentMountedVehicle is not null)
            {
                PlayerPatches.CurrentVehicleEnergyInterface = Player.main.currentMountedVehicle.energyInterface;
            }
            if (!EnableTools_)
            {
                PlayerPatches.CurrentToolEnergyMixin = null;
            }
            else
            {
                PlayerTool playerTool = Inventory.main.GetHeldTool();
                if (playerTool is not null)
                {
                    PlayerPatches.CurrentToolEnergyMixin = playerTool.energyMixin;
                }
            }
        }
    }
    internal static bool Enable_;
    internal static bool EnableSub_;
    internal static bool EnableVehicle_;
    internal static bool EnableTools_;
    internal static float SampleInterval_;
    internal static float SpikeThreshold_;
    internal static int SpikeLingerDuration_;
    internal static bool ModifyHud_;
    internal static bool DrawEnergyUnit_;
    internal static float InverseEnergyDisplay_;

    internal static Vector3 HudPosition_
    {
        get { return Instance.HudPosition; }
        set { Instance.HudPosition = value; Instance.Save(); }
    }
    internal static Vector2 HudSize_ => Instance.HudSize;
    internal static int HudFontSize_
    {
        get { return Instance.HudFontSize; }
        set { Instance.HudFontSize = value; Instance.HudSize = new(value * 10, value * 5); Instance.Save(); }
    }
}