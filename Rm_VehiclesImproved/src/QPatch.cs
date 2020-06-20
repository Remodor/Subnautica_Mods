using System.Reflection;
using System;
using Harmony;
using Rm_Config;
using SMLHelper.V2.Handlers;

namespace Rm_VehiclesImproved
{
    public class QPatch 
    {
        public static void Patch() 
        {
            Console.WriteLine("[Rm_VehiclesImproved] Begin patching:");
            Config<VehiclesConfig>.LoadConfiguration().ApplyModifier();
            OptionsPanelHandler.RegisterModOptions(new Options());
            var harmony = HarmonyInstance.Create("com.remodor.vehiclesimproved");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            Console.WriteLine("[Rm_VehiclesImproved] Finished patching.");
        }
    }
}