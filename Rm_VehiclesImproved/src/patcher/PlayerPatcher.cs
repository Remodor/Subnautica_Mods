using Harmony;
using UnityEngine;
using System;
using UnityEngine.UI;
using Rm_Config;
using Steamworks;
using System.IO;

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

        internal static float oldTimeDelta = 0;
        internal static float oldEnergyDelta = 0;

        internal static State currentState = State.None;
        internal enum State{
            None,
            Subroot,
            Vehicle
        }
        static void DrawEnergyInfo(float currentEnergyLevel, float timeDelta)
        {
            float energyDelta = currentEnergyLevel - energyLevel;

            float totalTimeDelta = timeDelta + oldTimeDelta;
            float totalEnergyDelta = energyDelta + oldEnergyDelta;

            float dailyEnergyConsumption = Mathf.RoundToInt(1200f / totalTimeDelta * totalEnergyDelta);
            EnergyInfo.energyDisplayText.text = EnergyInfo.hud_Text + dailyEnergyConsumption;
            EnergyInfo.gameObject.SetActive(true);
            energyLevel = currentEnergyLevel;
            timestamp = DayNightCycle.main.timePassedAsFloat;
            oldTimeDelta = timeDelta;
            oldEnergyDelta = energyDelta;
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
                //Reload
                else if(Input.GetKeyDown(KeyCode.Keypad5))
                {
                    Config<VehiclesConfig>.LoadConfiguration();
                    Config<VehiclesConfig>.Get().ApplyModifier();
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
            if (EnergyInfo.enabled)
            {
                ModifyHud();
                if ((timeDelta = DayNightCycle.main.timePassedAsFloat - timestamp) > 0.1f) {
                    if (Player.main.currentMountedVehicle != null && Player.main.currentMountedVehicle.energyInterface != null)
                    {
                        float currentEnergyLevel = Player.main.currentMountedVehicle.energyInterface.TotalCanProvide(out int sourceCount);
                        if (currentState != State.Vehicle)
                        {
                            currentState = State.Vehicle;
                            oldTimeDelta = oldEnergyDelta = 0;
                            energyLevel = currentEnergyLevel;
                            timestamp = DayNightCycle.main.timePassedAsFloat;
                        }
                        else
                        {
                            DrawEnergyInfo(currentEnergyLevel, timeDelta);
                        }
                    }
                    else if (Player.main.currentSub != null && Player.main.currentSub.powerRelay != null)
                    {
                        float currentEnergyLevel = Player.main.currentSub.powerRelay.GetPower();
                        if (currentState != State.Subroot)
                        {
                            currentState = State.Subroot;
                            oldTimeDelta = oldEnergyDelta = 0;
                            energyLevel = currentEnergyLevel;
                            timestamp = DayNightCycle.main.timePassedAsFloat;
                        }
                        else
                        {
                            DrawEnergyInfo(currentEnergyLevel, timeDelta);
                        }
                    }
                    else if (currentState != State.None)
                    {
                        currentState = State.None;
                        EnergyInfo.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}
