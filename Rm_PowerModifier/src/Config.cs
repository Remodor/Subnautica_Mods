using System.Collections.Generic;
using Nautilus.Handlers;
using Nautilus.Json;
using Nautilus.Options.Attributes;

namespace Rm_PowerModifier;
[Menu("Power Modifier Settings")]
public class Config : ConfigFile
{
    [Slider("Modifier: Solar Panel", 0.0f, 2.0f, DefaultValue = 0.3f, Format = "{0:F2}", Step = 0.01f, Tooltip = "The solar panel power production multiplier. Directly adjust the power production of the solar panel."), OnChange("OnConfigChange")]
    public float SolarPanelPowerModifier = 0.3f;

    [Slider("Modifier: Thermal Plant", 0.0f, 2.0f, DefaultValue = 1f, Format = "{0:F2}", Step = 0.01f, Tooltip = "The thermal plant power production multiplier. Directly adjust the power production of the thermal plant."), OnChange("OnConfigChange")]
    public float ThermalPlantPowerModifier = 1f;

    [Slider("Modifier: Bioreactor", 0.0f, 2.0f, DefaultValue = 1f, Format = "{0:F2}", Step = 0.01f, Tooltip = "The bioreactor production multiplier. Only modifies the production rate, NOT the energy values of the used biomass (a peeper still produces 420 energy)."), OnChange("OnConfigChange")]
    public float BioReactorPowerModifier = 1f;

    [Slider("Modifier: Nuclear Reactor", 0.0f, 2.0f, DefaultValue = 1f, Format = "{0:F2}", Step = 0.01f, Tooltip = "The nuclear reactor production multiplier. Only modifies the production rate, NOT the energy values of the reactor rods."), OnChange("OnConfigChange")]
    public float NuclearReactorPowerModifier = 1f;

    private const string CapacitySliderTooltip = "Specifies the total capacity of a power source. Requires a save reload or a reconstruction to take effect for already build power sources. When uninstalling the mod, simply reconstruct power sources to reset the power capacity.";
    [Slider("Energy Capacity: Solar Panel", 0, 750, DefaultValue = 75, Step = 5, Tooltip = CapacitySliderTooltip), OnChange("OnConfigChange")]
    public int SolarPanelMaxPower = 75;

    [Slider("Energy Capacity: Thermal Plant", 0, 1000, DefaultValue = 250, Step = 5, Tooltip = CapacitySliderTooltip), OnChange("OnConfigChange")]
    public int ThermalPlantMaxPower = 250;

    [Slider("Energy Capacity: Bioreactor", 0, 2000, DefaultValue = 500, Step = 10, Tooltip = CapacitySliderTooltip), OnChange("OnConfigChange")]
    public int BioReactorMaxPower = 500;

    [Slider("Energy Capacity: Nuclear Reactor", 0, 10000, DefaultValue = 2500, Step = 50, Tooltip = CapacitySliderTooltip), OnChange("OnConfigChange")]
    public int NuclearReactorMaxPower = 2500;

    [Toggle("Enable Power Order", Tooltip = "Enables a custom order for the power sources. Modded and other power sources can be set using the 'PowerOrder' entry inside the config file analogous to the already present power sources ('FoundPowerSources' is a list of all encountered power sources. This can help determine the name of a power source)."), OnChange("OnConfigChange")]
    public bool EnablePowerOrder = true;

    private const string PrioritySliderTooltip = "The priority for the power source ordering. Lower values mean higher priority (1: First, 2: Second..). If two power sources share the same priority, the order is random. Overrides the 'PowerOrder' entry inside the config file.";
    [Slider("Priority: Solar Panel", 1, 20, DefaultValue = 1, Tooltip = PrioritySliderTooltip), OnChange("OnConfigChange")]
    public int SolarPanelPriority = 1;

    [Slider("Priority: Thermal Plant", 1, 20, DefaultValue = 2, Tooltip = PrioritySliderTooltip), OnChange("OnConfigChange")]
    public int ThermalPlantPriority = 2;

    [Slider("Priority: Power Transmitter", 1, 20, DefaultValue = 3, Tooltip = "A connected power transmitter. Connected power sources are also ordered: " + PrioritySliderTooltip), OnChange("OnConfigChange")]
    public int PowerTransmitterPriority = 3;

    [Slider("Priority: Bioreactor", 1, 20, DefaultValue = 4, Tooltip = PrioritySliderTooltip), OnChange("OnConfigChange")]
    public int BioreactorPriority = 4;

    [Slider("Priority: Nuclear Reactor", 1, 20, DefaultValue = 5, Tooltip = PrioritySliderTooltip), OnChange("OnConfigChange")]
    public int NuclearReactorPriority = 5;

    [Slider("Priority: Other Power Sources", 1, 20, DefaultValue = 6, Tooltip = "The priority for all encountered power sources which are not specified in the 'PowerOrder' entry inside the config file."), OnChange("OnConfigChange")]
    public int OthersPriority = 6;

    public HashSet<string> FoundPowerSources = new();

    public Dictionary<string, int> PowerOrder = new();

    private static Config Instance;
    public Config() : base() { }
    public static void RegisterConfig()
    {
        Instance = OptionsPanelHandler.RegisterModOptions<Config>();
        OnConfigChange();
    }
    public static void OnConfigChange()
    {
        SolarPanelPowerModifier_ = Instance.SolarPanelPowerModifier;
        ThermalPlantPowerModifier_ = Instance.ThermalPlantPowerModifier;
        BioReactorPowerModifier_ = Instance.BioReactorPowerModifier;
        NuclearReactorPowerModifier_ = Instance.NuclearReactorPowerModifier;
        SolarPanelMaxPower_ = Instance.SolarPanelMaxPower;
        ThermalPlantMaxPower_ = Instance.ThermalPlantMaxPower;
        BioReactorMaxPower_ = Instance.BioReactorMaxPower;
        NuclearReactorMaxPower_ = Instance.NuclearReactorMaxPower;
        Instance.PowerOrder["SolarPanel"] = Instance.SolarPanelPriority;
        Instance.PowerOrder["ThermalPlant"] = Instance.ThermalPlantPriority;
        Instance.PowerOrder["PowerTransmitter"] = Instance.PowerTransmitterPriority;
        Instance.PowerOrder["BaseBioReactorModule"] = Instance.BioreactorPriority;
        Instance.PowerOrder["BaseNuclearReactorModule"] = Instance.NuclearReactorPriority;
        EnablePowerOrder_ = Instance.EnablePowerOrder;
        Instance.Save();
    }
    public static void AddFoundPowerSource(string powerSource)
    {
        if (Instance.FoundPowerSources.Add(powerSource))
        {
            Instance.Save();
        }
    }
    public static int GetPowerSourcePriority(string powerSource)
    {
        if (!Instance.PowerOrder.TryGetValue(powerSource, out int priority))
        {
            return Instance.OthersPriority;
        }
        return priority;
    }
    internal static float SolarPanelPowerModifier_;
    internal static float ThermalPlantPowerModifier_;
    internal static float BioReactorPowerModifier_;
    internal static float NuclearReactorPowerModifier_;
    internal static float SolarPanelMaxPower_;
    internal static float ThermalPlantMaxPower_;
    internal static float BioReactorMaxPower_;
    internal static float NuclearReactorMaxPower_;
    internal static bool EnablePowerOrder_;
}