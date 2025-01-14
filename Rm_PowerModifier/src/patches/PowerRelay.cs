using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace Rm_PowerModifier;

[HarmonyPatch(typeof(PowerRelay))]
class PowerRelayPatches
{
    public static string CleanName(string powerSource)
    {
        var clonePos = powerSource.IndexOf("(Clone)");
        string name;
        if (clonePos > 0)
        {
            name = powerSource.Remove(clonePos);
        }
        else
        {
            name = powerSource;
        }
#if DEBUG
        Dbg.StringUnique("PowerSource: " + powerSource);
#endif
        return name;
    }
    [HarmonyPatch(nameof(PowerRelay.AddInboundPower))]
    [HarmonyPostfix]
    static void AddInboundPower_Postfix(PowerRelay __instance, ref IPowerInterface powerInterface)
    {
        Config.AddFoundPowerSource(CleanName(((MonoBehaviour)powerInterface).name));
        if (Config.EnablePowerOrder_ && __instance.inboundPowerSources.Count > 1)
        {
#if DEBUG
            Dbg.Print("Sorting: Before:");
            foreach (var powerSource in __instance.inboundPowerSources)
            {
                Dbg.Print(CleanName(((MonoBehaviour)powerSource).name));
            }
#endif
            __instance.inboundPowerSources.Sort((x, y) =>
            {
                var xName = CleanName(((MonoBehaviour)x).name);
                var yName = CleanName(((MonoBehaviour)y).name);
                var xPriority = Config.GetPowerSourcePriority(xName);
                var yPriority = Config.GetPowerSourcePriority(yName);
                return xPriority - yPriority;
            });
#if DEBUG
            Dbg.Print("Sorting: After:");
            foreach (var powerSource in __instance.inboundPowerSources)
            {
                Dbg.Print(CleanName(((MonoBehaviour)powerSource).name));
            }
#endif
        }
    }


}