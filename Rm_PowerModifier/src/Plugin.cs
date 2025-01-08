using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace Rm_PowerModifier;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.snmodding.nautilus")]
public class Plugin : BaseUnityPlugin
{
    public new static ManualLogSource Logger { get; private set; }

    private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

    private void Awake()
    {
        Logger = base.Logger;
        Rm_PowerModifier.Config.RegisterConfig();
#if DEBUG
        Dbg.Print("### Debug Build ###");
#endif

        Harmony.CreateAndPatchAll(Assembly, $"{PluginInfo.PLUGIN_GUID}");
    }
}