using FMOD;
using System;
using UnityEngine;

namespace Rm_VehiclesImproved
{
    public class VehiclesConfig
    {
        public float CyclopsIdlingEnergyPerDay = 0f;
        public float CyclopsSilentRunningEnergyPerDay = 1200f;

        public bool CyclopsAlternativeCameraControls = true;

        public float CyclopsCameraRotationDamper = 3f;

        public bool CyclopsAutoCloseHatch = true;
        public bool DebugEnergyInfo = false;
        public bool DebugEnergyInfoModify = false;

        public float DebugEnergyHud_Position_X = 15;
        public float DebugEnergyHud_Position_Y = 395;
        public float DebugEnergyHud_Size_X = 500;
        public float DebugEnergyHud_Size_Y = 250;
        public int DebugEnergyHud_FontSize = 20;
        public string DebugEnergyHud_Text = "Energy: ";
        public void ApplyModifier()
        {
            CyclopsSettings.engineIdlingEnergyConsumption= ConvertToSeconds(CyclopsIdlingEnergyPerDay);
            CyclopsSettings.cameraRotationDamper = Mathf.Max(0.1f, CyclopsCameraRotationDamper);
            CyclopsSettings.alternativeCameraControls = CyclopsAlternativeCameraControls;
            CyclopsSettings.silentRunningEnergyConsumption = ConvertToSeconds(CyclopsSilentRunningEnergyPerDay);
            CyclopsSettings.autoCloseHatch = CyclopsAutoCloseHatch;
            EnergyInfo.enabled = DebugEnergyInfo;
            if (EnergyInfo.gameObject != null)
            {
                EnergyInfo.gameObject.SetActive(DebugEnergyInfo);
            }
            EnergyInfo.modify = DebugEnergyInfoModify;
            EnergyInfo.hud_Position = new Vector3(DebugEnergyHud_Position_X, DebugEnergyHud_Position_Y, 0);
            EnergyInfo.hud_Size = new Vector2(DebugEnergyHud_Size_X, DebugEnergyHud_Size_Y);
            EnergyInfo.hud_FontSize = DebugEnergyHud_FontSize;
            EnergyInfo.hud_Text = DebugEnergyHud_Text;
        }
        public static float ConvertToSeconds(float energyPerDay)
        {
            return energyPerDay / 1200f;
        }
    }
}
