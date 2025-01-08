#if DEBUG
using System.Collections.Generic;
using UnityEngine;

namespace Rm_PowerModifier;
public class Dbg
{
    private static Dictionary<string, float> PreviousFloat;
    private static Dictionary<string, bool> PreviousBool;
    public static void FloatChanged(string label, float value, int digits = 0)
    {
        PreviousFloat ??= new();
        float previous = PreviousFloat.GetOrDefault(label, value + 1);
        float value_rounded = (float)System.Math.Round(value, digits);
        if (previous != value_rounded)
        {
            Plugin.Logger.LogInfo($"#{label}: {value_rounded}");
            PreviousFloat.Remove(label);
            PreviousFloat.Add(label, value_rounded);
        }
    }
    public static void Float(string label, float value)
    {
        float value_rounded = (float)System.Math.Round(value, 2);
        Plugin.Logger.LogInfo($"#{label}: {value_rounded}");
    }
    public static void BoolChanged(string label, bool value)
    {
        PreviousBool ??= new();
        bool previous = PreviousBool.GetOrDefault(label, !value);
        if (previous != value)
        {

            Plugin.Logger.LogInfo($"#{label}: {value}");
            PreviousBool.Remove(label);
            PreviousBool.Add(label, value);

        }
    }
    public static void Bool(string label, bool value)
    {
        Plugin.Logger.LogInfo($"#{label}: {value}");
    }
    public static void Print(string str)
    {
        Plugin.Logger.LogInfo(str);
    }
}
#endif