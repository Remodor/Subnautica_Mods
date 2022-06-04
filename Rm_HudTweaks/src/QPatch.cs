using System.Reflection;
using System;
using HarmonyLib;
using Rm_Config;
using SMLHelper.V2.Handlers;
using QModManager.API.ModLoading;

namespace Rm_HudTweaks
{
    [QModCore]
    public static class QPatch 
    {
        [QModPatch]
        public static void Patch() 
        {
            Console.WriteLine("[Rm_HudTweaks] Begin patching");
            Config<HudTweaksConfig>.LoadConfiguration();
            OptionsPanelHandler.RegisterModOptions(new Options());
            var assembly = Assembly.GetExecutingAssembly();
            new Harmony($"Remodor_{assembly.GetName().Name}").PatchAll(assembly);
            Console.WriteLine("[Rm_HudTweaks] Successfully patched!");
        }
    }
}