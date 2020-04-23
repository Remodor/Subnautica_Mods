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

            //// Create Canvas GameObject.
            //GameObject canvasGO = new GameObject();
            //canvasGO.name = "Canvas";
            //canvasGO.AddComponent<Canvas>();

            //// Get canvas from the GameObject.
            //Canvas canvas;
            //canvas = canvasGO.GetComponent<Canvas>();
            //canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            // Create the Text GameObject.
            GameObject energyDisplayUI = GameObject.Find("EnergyDisplayUI");
            if (energyDisplayUI == null)
            {
                energyDisplayUI = new GameObject("EnergyDisplayUI");
            }
            else
            {
                Console.WriteLine("#E energyDisplayUI already found");
            }
            energyDisplayUI.transform.SetParent(playerHUD.transform, false);
            energyDisplayUI.gameObject.AddComponent<Text>();

            var energyDisplayText = energyDisplayUI.GetComponent<Text>();
            energyDisplayText.font = __instance.textStyle.font;
            energyDisplayText.fontSize = 40;
            energyDisplayText.alignment = TextAnchor.UpperLeft;
            energyDisplayText.color = Color.yellow;

            RectTransform rectTransform;
            rectTransform = energyDisplayText.GetComponent<RectTransform>();
            rectTransform.localPosition = new Vector3(0, 0, 0);
            rectTransform.sizeDelta = new Vector2(150, 500);

            //Console.WriteLine("#hud: {0}", hud);
            //Console.WriteLine("#hud2: {0}", hud2);
            //var go = new GameObject("ad");
            //var energyEx = go.AddComponent<Text>();
            //Player_Update_Patch.energyDisplay.text = "aaaaaaaaaaaaaaaaaaaaaaAaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            //Player_Update_Patch.energyDisplay.enabled = true;
            //go.transform.SetParent(hud, false);
            //go.transform.localPosition = new Vector3(-450, 450, 0);
            //Player_Update_Patch.energyDisplay.font = __instance.textStyle.font;
            //Player_Update_Patch.energyDisplay.canvas.enabled = true;
            //Player_Update_Patch.energyDisplay.color = Color.yellow;
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



            if (__instance != Player.main)
            {
                Console.WriteLine("#E __instance != Player.main");
            }

            if (__instance.currentMountedVehicle != null)
            {

            }


            var timeDeltaToDayFactor = 1200f / DayNightCycle.main.deltaTime;
            var energyProductionExtrapolationPerDay = Mathf.RoundToInt(timeDeltaToDayFactor * EnergyInfo.energyProduction);
            var energyConsumptionExtrapolationPerDay = Mathf.RoundToInt(timeDeltaToDayFactor * EnergyInfo.energyConsumption);
            EnergyInfo.energyDisplayText.text = "Energyproduction:  " + energyProductionExtrapolationPerDay +
                                                  "\nDaily Consumed Energy: " + energyConsumptionExtrapolationPerDay;

 

        var currentEnergyState = __instance.powerRelay.GetPower();
        var currentTimeStamp = DayNightCycle.main.timePassedAsFloat;
                if (previousSub == __instance)
                {
                    var timeDelta = currentTimeStamp - timeStamp;
                    if (timeDelta > 0.5f)
                    {
                        var timeDeltaToDayFactor = 1200f / timeDelta;
        var energyDelta = currentEnergyState - energyState;
        var energyExtrapolationPerDay = Mathf.RoundToInt(timeDeltaToDayFactor * energyDelta);
        ErrorMessage.AddMessage("Daily Energy: " + energyExtrapolationPerDay.ToString());
                        energyState = currentEnergyState;
                        timeStamp = currentTimeStamp;
                    }
}
                else
                {
                    previousSub = __instance;
                    timeStamp = currentTimeStamp;
                    energyState = currentEnergyState;
                }


        //var timeDeltaToDayFactor = 1200f / DayNightCycle.main.deltaTime;
        //var energyProductionExtrapolationPerDay = Mathf.RoundToInt(timeDeltaToDayFactor * EnergyInfo.energyProduction);
        //var energyConsumptionExtrapolationPerDay = Mathf.RoundToInt(timeDeltaToDayFactor * EnergyInfo.energyConsumption);
        //EnergyInfo.energyDisplayText.text = "Energyproduction:  " + energyProductionExtrapolationPerDay +
        //                                      "\nDaily Consumed Energy: " + energyConsumptionExtrapolationPerDay;

        EnergyInfo.energyProduction = 0;
            EnergyInfo.energyConsumption = 0;
            //if (__instance.currentMountedVehicle != null)
            //{
            //    currentEnergyInterface = __instance.currentMountedVehicle.energyInterface;
            //}
            //else
            //{
            //    currentEnergyInterface = null;
            //    if (__instance.currentSub)
            //}


            //if (SubRootSettings.debugDailyEnergyExtrapolation && (__instance.currentSub != null || __instance.currentMountedVehicle != null))
            //{
            //    var currentEnergyState;
            //    if (__instance.currentSub != null)
            //    {

            //    }
            //    else
            //    {

            //    }
            ////        var currentEnergyState = __instance.powerRelay.GetPower();
            ////        var currentTimeStamp = DayNightCycle.main.timePassedAsFloat;
            ////        if (previousSub == __instance)
            ////        {
            ////            var timeDelta = currentTimeStamp - timeStamp;
            ////            if (timeDelta > 0.5f)
            ////            {
            ////                var timeDeltaToDayFactor = 1200f / timeDelta;
            ////                var energyDelta = currentEnergyState - energyState;
            ////                var energyExtrapolationPerDay = Mathf.RoundToInt(timeDeltaToDayFactor * energyDelta);
            ////                ErrorMessage.AddMessage("Daily Energy: " + energyExtrapolationPerDay.ToString());
            ////                energyState = currentEnergyState;
            ////                timeStamp = currentTimeStamp;
            ////            }
            ////        }
            ////        else
            ////        {
            ////            previousSub = __instance;
            ////            timeStamp = currentTimeStamp;
            ////            energyState = currentEnergyState;
            ////        }
            //}
        }
    }
}
