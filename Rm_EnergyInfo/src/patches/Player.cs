using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace Rm_EnergyInfo;
[HarmonyPatch(typeof(Player))]
class PlayerPatches
{
    private static Text EnergyUiText;
    internal static GameObject EnergyUiObject;
    // Create hud.
    [HarmonyPatch(nameof(Player.Awake))]
    [HarmonyPostfix]
    static void Awake_Postfix(Player __instance)
    {
        var playerHUD = GameObject.Find("HUD");
        GameObject EnergyUI = GameObject.Find("EnergyUI") ?? new GameObject("EnergyUI");
        EnergyUI.transform.SetParent(playerHUD.transform, false);
        var energyDisplayText = EnergyUI.GetComponent<Text>() ?? EnergyUI.gameObject.AddComponent<Text>();
        energyDisplayText.font = __instance.textStyle.font;
        energyDisplayText.fontSize = Config.HudFontSize_;
        energyDisplayText.alignment = TextAnchor.UpperRight;
        energyDisplayText.color = Color.yellow;
        energyDisplayText.raycastTarget = false;

        RectTransform rectTransform = energyDisplayText.GetComponent<RectTransform>();
        rectTransform.localPosition = Config.HudPosition_;
        rectTransform.sizeDelta = Config.HudSize_;

        EnergyUiText = energyDisplayText;
        EnergyUiObject = EnergyUI;
        EnergyUI.SetActive(false);
    }
    public const int MaxFontSize = 65;
    static void ModifyHud()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) && Config.HudFontSize_ < MaxFontSize)
            {
                Config.HudFontSize_++;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) && Config.HudFontSize_ > 1)
            {
                Config.HudFontSize_--;
            }
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            Config.HudPosition_ += new Vector3(0, 1, 0);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            Config.HudPosition_ += new Vector3(0, -1, 0);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            Config.HudPosition_ += new Vector3(-1, 0, 0);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            Config.HudPosition_ += new Vector3(1, 0, 0);
        }
        var rectTransform = EnergyUiText.GetComponent<RectTransform>();
        rectTransform.localPosition = Config.HudPosition_;
        rectTransform.sizeDelta = Config.HudSize_;
        EnergyUiText.fontSize = Config.HudFontSize_;
    }
    private static float Timestamp = 0;
    private static float PreviousEnergyLevel = 0;
    private static float HourlyEnergyConsumption = 0;
    private static float SpikeEnergyConsumption = 0;
    private static float EnergyDeltaSlope = 0;
    private static float OldEnergyDelta = 0;
    private static float OldTimeDelta = 0;
    private static int SpikeLingerDuration = 0;
    private static bool HadSpike = false;
    private static float SampleInterval = 0.1f;
    private static bool _Reset = true;
    internal static EnergyMixin CurrentToolEnergyMixin = null;
    internal static PowerRelay CurrentSubPowerRelay = null;
    internal static EnergyInterface CurrentVehicleEnergyInterface = null;
    internal static void Reset()
    {
#if DEBUG
        Dbg.Print("Reset");
#endif
        _Reset = true;
    }
    [HarmonyPatch(nameof(Player.LateUpdate))]
    [HarmonyPostfix]
    static void LateUpdate_Postfix(Player __instance)
    {
        if (!Config.Enable_)
        {
            return;
        }
        float currentEnergyLevel;
        if (CurrentSubPowerRelay is not null)
        {
            currentEnergyLevel = CurrentSubPowerRelay.GetPower();
        }
        else if (CurrentVehicleEnergyInterface is not null)
        {
            currentEnergyLevel = CurrentVehicleEnergyInterface.TotalCanProvide(out int _sourceCount);
        }
        else if (CurrentToolEnergyMixin is not null)
        {
            currentEnergyLevel = CurrentToolEnergyMixin.charge;
        }
        else
        {
            EnergyUiObject.SetActive(false);
            return;
        }
        if (Config.ModifyHud_) ModifyHud();
        if (_Reset)
        {
            _Reset = false;
            PreviousEnergyLevel = currentEnergyLevel;
            HourlyEnergyConsumption = 0;
            OldEnergyDelta = 0;
            OldTimeDelta = 0;
            SpikeLingerDuration = 0;
            Timestamp = DayNightCycle.main.timePassedAsFloat;
            SampleInterval = Config.SampleInterval_ * DayNightCycle.main.dayNightSpeed;
            return;
        }
        float timeDelta = DayNightCycle.main.timePassedAsFloat - Timestamp;
        if (timeDelta < SampleInterval)
        {
            return;
        }
        float energyDelta = currentEnergyLevel - PreviousEnergyLevel;
        EnergyDeltaSlope = energyDelta - OldEnergyDelta;
        float EnergyDeltaGain = Mathf.Abs(energyDelta) - Mathf.Abs(OldEnergyDelta);
#if DEBUG
        Dbg.FloatChanged("energyDelta", energyDelta, 1);
        Dbg.FloatChanged("timeDelta", timeDelta, 1);
        Dbg.FloatChanged("EnergyDeltaSlope", EnergyDeltaSlope, 1);
#endif
        if (HadSpike)
        {
            HadSpike = false;
#if DEBUG
            Dbg.Print("HadSike");
#endif
            HourlyEnergyConsumption = Config.InverseEnergyDisplay_ * energyDelta * 50f / timeDelta;
        }
        else if (EnergyDeltaGain > Config.SpikeThreshold_)
        {
            HadSpike = true;
#if DEBUG
            Dbg.Print("Spike");
#endif
            SpikeLingerDuration = Config.SpikeLingerDuration_;
            SpikeEnergyConsumption = Config.InverseEnergyDisplay_ * EnergyDeltaSlope;
            HourlyEnergyConsumption = Config.InverseEnergyDisplay_ * OldEnergyDelta * 50f / OldTimeDelta;
        }
        else
        {
            float totalEnergyDelta = OldEnergyDelta + energyDelta;
            float totalTimeDelta = OldTimeDelta + timeDelta;
            HourlyEnergyConsumption = Config.InverseEnergyDisplay_ * totalEnergyDelta * 50f / totalTimeDelta;
        }
        if (SpikeLingerDuration > 0)
        {
            SpikeLingerDuration--;
            if (Config.DrawEnergyUnit_)
            {
                EnergyUiText.text = HourlyEnergyConsumption.ToString("0.0") + " e/h\n" + SpikeEnergyConsumption.ToString("0.0") + " e";
            }
            else
            {
                EnergyUiText.text = HourlyEnergyConsumption.ToString("0.0") + "\n" + SpikeEnergyConsumption.ToString("0.0");

            }
        }
        else
        {
            if (Config.DrawEnergyUnit_)
            {
                EnergyUiText.text = HourlyEnergyConsumption.ToString("0.0") + " e/h";
            }
            else
            {
                EnergyUiText.text = HourlyEnergyConsumption.ToString("0.0");
            }
        }
        EnergyUiObject.SetActive(true);
        PreviousEnergyLevel = currentEnergyLevel;
        OldTimeDelta = timeDelta;
        OldEnergyDelta = energyDelta;
        Timestamp = DayNightCycle.main.timePassedAsFloat;
    }
    [HarmonyPatch(nameof(Player.SetCurrentSub))]
    [HarmonyPostfix]
    static void SetCurrentSub_Postfix(Player __instance)
    {
#if DEBUG
        Dbg.Print("Player.SetCurrentSub: " + (__instance._currentSub is not null));
#endif
        if (Config.EnableSub_ && __instance._currentSub is not null)
        {
            if (CurrentSubPowerRelay is null)
            {
                PlayerPatches.Reset();
            }
            CurrentSubPowerRelay = __instance._currentSub.powerRelay;
        }
        else
        {
            PlayerPatches.Reset();
            CurrentSubPowerRelay = null;
        }
    }
    [HarmonyPatch(nameof(Player.ExitLockedMode))]
    [HarmonyPostfix]
    static void ExitLockedMode_Postfix()
    {
#if DEBUG
        Dbg.Print("Player.ExitLockedMode");
#endif
        CurrentVehicleEnergyInterface = null;
    }
}