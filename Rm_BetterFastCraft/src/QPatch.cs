using System.Reflection;
using System;
using Harmony;
using Rm_Config;
using SMLHelper.V2.Handlers;

namespace Rm_BetterFastCraft
{
    public class QPatch 
    {
        public static void Patch() 
        {
            Console.WriteLine("[Rm_BetterFastCraft] Begin patching:");
            Config<CraftConfig>.LoadConfiguration().ApplyModifier();
            OptionsPanelHandler.RegisterModOptions(new Options());
            var harmony = HarmonyInstance.Create("com.remodor.betterfastcraft");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            Console.WriteLine("[Rm_BetterFastCraft] Finished patching.");
        }
    }
}