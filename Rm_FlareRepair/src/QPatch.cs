using System.Reflection;
using System;
using Harmony;
using Rm_Config;
using SMLHelper.V2.Handlers;

namespace Rm_FlareRepair
{
    public class QPatch 
    {
        public static void Patch() 
        {
            Console.WriteLine("[Rm_FlareRepair] Begin patching");
            Config<FlareConfig>.LoadConfiguration().ApplyConfig();
            OptionsPanelHandler.RegisterModOptions(new Options());
            var harmony = HarmonyInstance.Create("com.remodor.flarerepair");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            Console.WriteLine("[Rm_FlareRepair] Successfully patched!");
        }
    }
}