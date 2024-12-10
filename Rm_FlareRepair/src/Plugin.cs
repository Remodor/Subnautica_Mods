using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace Rm_FlareRepair;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.snmodding.nautilus")]
public class Plugin : BaseUnityPlugin
{
    public new static ManualLogSource Logger { get; private set; }

    private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

    private void Awake()
    {
        Logger = base.Logger;
        Rm_FlareRepair.Config.RegisterConfig();

        Harmony.CreateAndPatchAll(Assembly, $"{PluginInfo.PLUGIN_GUID}");
    }
}