using System.Reflection;
using System;
using Harmony;
using Rm_Config;
using SMLHelper.V2.Handlers;

namespace Rm_PowerModifier
{
    public class QPatch 
    {
        public static void Patch() 
        {
            Console.WriteLine("[Rm_PowerModifier] Begin patching");
            Config<PowerModifier>.LoadConfiguration().ApplyModifier();
            OptionsPanelHandler.RegisterModOptions(new Options());
            var harmony = HarmonyInstance.Create("com.remodor.powermodifier");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            Console.WriteLine("[Rm_PowerModifier] Successfully patched!");
        }
    }
}