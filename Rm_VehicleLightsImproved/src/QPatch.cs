using System.Reflection;
using System;
using Harmony;
using Rm_Config;
using SMLHelper.V2.Handlers;

namespace Rm_VehicleLightsImproved
{
    public class QPatch 
    {
        public static void Patch() 
        {
            Console.WriteLine("[Rm_VehicleLightsImproved] Begin patching:");
            Config<LightsConfig>.LoadConfiguration().ApplyModifier();
            OptionsPanelHandler.RegisterModOptions(new Options());
            var harmony = HarmonyInstance.Create("com.remodor.vehiclelightsimproved");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            Console.WriteLine("[Rm_VehicleLightsImproved] Finished patching.");
        }
    }
}