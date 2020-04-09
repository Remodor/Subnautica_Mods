using System.Reflection;
using System;
using Harmony;
using Rm_Config;

namespace Rm_FastKnownScan
{
    public class QPatch 
    {
        public static void Patch() 
        {
            Console.WriteLine("[Rm_FastKnownScan] Begin patching");
            Config<ScanConfig>.LoadConfiguration().ApplyModifier();
            var harmony = HarmonyInstance.Create("com.remodor.fastknownscan");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            Console.WriteLine("[Rm_FastKnownScan] Finished patching!");
        }
    }
}