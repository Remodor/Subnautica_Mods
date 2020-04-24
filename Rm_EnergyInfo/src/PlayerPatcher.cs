using Harmony;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace Rm_VehicleLightsImproved
{

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
            energyDisplayText.fontSize = 40;
            energyDisplayText.alignment = TextAnchor.UpperLeft;
            energyDisplayText.color = Color.yellow;

            RectTransform rectTransform;
            rectTransform = energyDisplayText.GetComponent<RectTransform>();
            rectTransform.localPosition = new Vector3(-475, 225, 0);
            rectTransform.sizeDelta = new Vector2(500, 150);

            EnergyInfo.energyDisplayText = energyDisplayText;
        }
    }
    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch(nameof(Player.LateUpdate))]
    internal class Player_LateUpdate_Patch
    {
        static void Postfix(Player __instance)
        {
            var rectTransform = EnergyInfo.energyDisplayText.GetComponent<RectTransform>();

            if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                rectTransform.localPosition += new Vector3(0, 25, 0);
            }
            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                rectTransform.localPosition += new Vector3(0, -25, 0);
            }
            if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                rectTransform.localPosition += new Vector3(25, 0, 0);
            }
            if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                rectTransform.localPosition += new Vector3(-25, 0, 0);
            }

            if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                rectTransform.sizeDelta += new Vector2(10, 10);
            }
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                rectTransform.sizeDelta += new Vector2(-10, -10);
            }

            if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                EnergyInfo.energyDisplayText.fontSize--;
            }
            if (Input.GetKeyDown(KeyCode.Keypad9))
            {
                EnergyInfo.energyDisplayText.fontSize++;
            }
            Console.WriteLine("#localPosition: {0}", rectTransform.localPosition);
            Console.WriteLine("#sizeDelta: {0}", rectTransform.sizeDelta);
            Console.WriteLine("#fontSize: {0}", EnergyInfo.energyDisplayText.fontSize);

            EnergyInfo.energyDisplayText.text = "";
            var timeDeltaToDayFactor = 1200f / DayNightCycle.main.deltaTime;
            if (EnergyInfo.energyConsumption != 0)
            {
                var energyConsumptionExtrapolationPerDay = Mathf.RoundToInt(timeDeltaToDayFactor * EnergyInfo.energyConsumption);
                EnergyInfo.energyDisplayText.text += "Energy Consumption: " + energyConsumptionExtrapolationPerDay;
                EnergyInfo.energyConsumption = 0;
            }
            if (EnergyInfo.energyProduction != 0)
            {
                var energyProductionExtrapolationPerDay = Mathf.RoundToInt(timeDeltaToDayFactor * EnergyInfo.energyProduction);
                EnergyInfo.energyDisplayText.text += "\nEnergy Production:  " + energyProductionExtrapolationPerDay;
                EnergyInfo.energyProduction = 0;
            }
        }
    }
}
