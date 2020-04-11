using System.Reflection;
using System;
using Harmony;

namespace Rm_VehicleLoadFix
{
    public class QPatch 
    {
        public static void Patch() 
        {
            Console.WriteLine("[Rm_VehicleLoadFix] Begin patching:");
            var harmony = HarmonyInstance.Create("com.remodor.vehicleloadfix");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            Console.WriteLine("[Rm_VehicleLoadFix] Finished patching.");
        }
    }
}