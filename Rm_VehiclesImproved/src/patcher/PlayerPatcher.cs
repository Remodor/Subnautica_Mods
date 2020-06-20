using Harmony;
using UnityEngine;
using System;
using UnityEngine.UI;
using Rm_Config;
using Steamworks;

namespace Rm_VehiclesImproved
{
    internal static class EnergyInfo
    {
        internal static Text energyDisplayText;
        internal static GameObject gameObject;

        internal static bool enabled;
        internal static bool modify;

        internal static Vector3 hud_Position = new Vector3(-475, 225, 0);
        internal static Vector2 hud_Size = new Vector2(500, 150);
        internal static int hud_FontSize = 30;
        internal static string hud_Text = "Energy Consumption: ";
    }

    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch(nameof(Player.Awake))]
    internal class Player_Awake_Patch
    {
        static void Postfix(Player __instance)
        {
            var playerHUD = GameObject.Find("HUD");

            GameObject energyDisplayUI = GameObject.Find("EnergyDisplayUI");
            if (energyDisplayUI == null)
            {
                energyDisplayUI = new GameObject("EnergyDisplayUI");
            }
            energyDisplayUI.transform.SetParent(playerHUD.transform, false);

            var energyDisplayText = energyDisplayUI.GetComponent<Text>();
            if (energyDisplayText == null)
            {
                energyDisplayText = energyDisplayUI.gameObject.AddComponent<Text>();
            }
            energyDisplayText.font = __instance.textStyle.font;
            energyDisplayText.fontSize = EnergyInfo.hud_FontSize;
            energyDisplayText.alignment = TextAnchor.UpperLeft;
            energyDisplayText.color = Color.yellow;

            RectTransform rectTransform;
            rectTransform = energyDisplayText.GetComponent<RectTransform>();
            rectTransform.localPosition = EnergyInfo.hud_Position;
            rectTransform.sizeDelta = EnergyInfo.hud_Size;

            EnergyInfo.energyDisplayText = energyDisplayText;
            EnergyInfo.gameObject = energyDisplayUI;
            energyDisplayUI.SetActive(false);
        }
    }
    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch(nameof(Player.LateUpdate))]
    internal class Player_LateUpdate_Patch
    {
        internal static float timestamp = 0;
        internal static float energyLevel = 0;

        internal const int timeEnergy_DeltaValues_Size = 3;
        internal static float[,] timeEnergy_DeltaValues = new float[timeEnergy_DeltaValues_Size, 2];
        internal static int currentItteration = 0;
        internal static State currentState = State.None;
        internal enum State{
            None,
            Subroot,
            Vehicle
        }
        static void DrawEnergyInfo(float currentEnergyLevel, float timeDelta)
        {
            float energyDelta = currentEnergyLevel - energyLevel;
            timeEnergy_DeltaValues[currentItteration, 0] = timeDelta;
            timeEnergy_DeltaValues[currentItteration, 1] = energyDelta;
            Console.WriteLine("#8.0");
            currentItteration++;
            if (currentItteration == timeEnergy_DeltaValues_Size)
            {
                currentItteration = 0;
            }

            float totalTimeDelta = 0;
            float totalEnergyDelta = 0;
            Console.WriteLine("#8.1");
            for (int i = 0; i < timeEnergy_DeltaValues_Size; i++)
            {
                totalTimeDelta += timeEnergy_DeltaValues[i, 0];
                totalEnergyDelta += timeEnergy_DeltaValues[i, 1];
            }

            float dailyEnergyConsumption = Mathf.RoundToInt(1200f / totalTimeDelta * totalEnergyDelta);
            EnergyInfo.energyDisplayText.text = EnergyInfo.hud_Text + dailyEnergyConsumption;
            EnergyInfo.gameObject.SetActive(true);
            energyLevel = currentEnergyLevel;
            timestamp = DayNightCycle.main.timePassedAsFloat;
        }
        static void ModifyHud()
        {
            if (EnergyInfo.modify)
            {
                var rectTransform = EnergyInfo.energyDisplayText.GetComponent<RectTransform>();
                //RectPosition
                if (Input.GetKeyDown(KeyCode.Keypad8))
                {
                    EnergyInfo.hud_Position += new Vector3(0, 10, 0);
                }
                else if (Input.GetKeyDown(KeyCode.Keypad2))
                {
                    EnergyInfo.hud_Position += new Vector3(0, -10, 0);
                }
                else if (Input.GetKeyDown(KeyCode.Keypad6))
                {
                    EnergyInfo.hud_Position += new Vector3(10, 0, 0);
                }
                else if (Input.GetKeyDown(KeyCode.Keypad4))
                {
                    EnergyInfo.hud_Position += new Vector3(-10, 0, 0);
                }
                //RectSize
                else if (Input.GetKeyDown(KeyCode.KeypadMultiply))
                {
                    EnergyInfo.hud_Size += new Vector2(10, 10);
                }
                else if (Input.GetKeyDown(KeyCode.KeypadDivide))
                {
                    EnergyInfo.hud_Size += new Vector2(-10, -10);
                }
                //FontSize
                else if (Input.GetKeyDown(KeyCode.KeypadPlus))
                {
                    EnergyInfo.hud_FontSize++;
                }
                else if (Input.GetKeyDown(KeyCode.KeypadMinus))
                {
                    EnergyInfo.hud_FontSize--;
                }
                else
                {
                    return;
                }
                rectTransform.localPosition = EnergyInfo.hud_Position;
                rectTransform.sizeDelta = EnergyInfo.hud_Size;
                EnergyInfo.energyDisplayText.fontSize = EnergyInfo.hud_FontSize;
                SaveDebugEnergyInfoHud();
            }
        }
        static void SaveDebugEnergyInfoHud()
        {
            Config<VehiclesConfig>.Get().DebugEnergyHud_Position_X = EnergyInfo.hud_Position.x;
            Config<VehiclesConfig>.Get().DebugEnergyHud_Position_Y = EnergyInfo.hud_Position.y;
            Config<VehiclesConfig>.Get().DebugEnergyHud_Size_X = EnergyInfo.hud_Size.x;
            Config<VehiclesConfig>.Get().DebugEnergyHud_Size_Y = EnergyInfo.hud_Size.y;
            Config<VehiclesConfig>.Get().DebugEnergyHud_FontSize = EnergyInfo.hud_FontSize;
            Config<VehiclesConfig>.Get().DebugEnergyHud_Text = EnergyInfo.hud_Text;
            Config<VehiclesConfig>.SaveConfiguration();
        }
        static void Postfix(Player __instance)
        {
            float timeDelta;
            if (EnergyInfo.enabled && (timeDelta = DayNightCycle.main.timePassedAsFloat - timestamp) > 0.07f)
            {
                Console.WriteLine("#2");
                if (Player.main.currentMountedVehicle != null && Player.main.currentMountedVehicle.energyInterface != null)
                {
                    Console.WriteLine("#3");

                    float currentEnergyLevel = Player.main.currentMountedVehicle.energyInterface.TotalCanProvide(out int sourceCount);
                    if (currentState != State.Vehicle)
                    {
                        Console.WriteLine("#4");

                        currentState = State.Vehicle;
                        Array.Clear(timeEnergy_DeltaValues, 0, timeEnergy_DeltaValues_Size);
                        energyLevel = currentEnergyLevel;
                        timestamp = DayNightCycle.main.timePassedAsFloat;
                    } else
                    {
                        Console.WriteLine("#5");

                        DrawEnergyInfo(currentEnergyLevel, timeDelta);
                        ModifyHud();
                    }
                }
                else if(Player.main.currentSub != null && Player.main.currentSub.powerRelay != null)
                {
                    Console.WriteLine("#6");
                    float currentEnergyLevel = Player.main.currentSub.powerRelay.GetPower();
                    Console.WriteLine("#6.5");

                    if (currentState != State.Subroot)
                    {
                        Console.WriteLine("#7");

                        currentState = State.Subroot;
                        Array.Clear(timeEnergy_DeltaValues, 0, timeEnergy_DeltaValues_Size);
                        energyLevel = currentEnergyLevel;
                        timestamp = DayNightCycle.main.timePassedAsFloat;
                    }
                    else
                    {
                        Console.WriteLine("#8");

                        DrawEnergyInfo(currentEnergyLevel, timeDelta);
                        ModifyHud();
                    }
                }
                else if(currentState != State.None)
                {
                    Console.WriteLine("#9");

                    currentState = State.None;
                    EnergyInfo.gameObject.SetActive(false);
                }
            }
        }
    }
}
