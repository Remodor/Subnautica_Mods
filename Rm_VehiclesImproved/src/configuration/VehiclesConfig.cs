using FMOD;
using System;
using UnityEngine;

namespace Rm_VehiclesImproved
{
    public class DebugEnergyHud
    {
        public Vector3 position = new Vector3(-475, 225, 0);
        public Vector2 size = new Vector2(500, 150);
        public int fontSize = 30;
        public string text = "Energy Consumption: ";
    }
    public class VehiclesConfig
    {
        public float CyclopsIdlingEnergyPerDay = 0f;
        public float CyclopsSilentRunningEnergyPerDay = 1200f;

        public bool CyclopsAlternativeCameraControls = true;

        public float CyclopsCameraRotationDamper = 3f;

        public bool CyclopsAutoCloseHatch = true;
        public bool DebugEnergyInfo = false;
        public bool DebugEnergyInfoModify = false;

        public DebugEnergyHud DebugEnergyInfoHud;

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
            EnergyInfo.hud = DebugEnergyInfoHud;
        }
        public static float ConvertToSeconds(float energyPerDay)
        {
            return energyPerDay / 1200f;
        }
    }
}
