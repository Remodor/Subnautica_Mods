using System.Reflection;
using System;
using Harmony;
using Rm_Config;
using SMLHelper.V2.Handlers;

namespace Rm_FastBuild
{
    public class QPatch 
    {
        public static void Patch() 
        {
            Console.WriteLine("[Rm_FastBuild] Begin patching:");
            Config<BuildConfig>.LoadConfiguration().ApplyModifier();
            OptionsPanelHandler.RegisterModOptions(new Options());
            var harmony = HarmonyInstance.Create("com.remodor.fastbuild");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            Console.WriteLine("[Rm_FastBuild] Finished patching.");
        }
    }
}