using System;
using UnityEngine;

namespace Rm_VehiclesImproved
{
    public class LightsConfig
    {
        public float CyclopsIdlingEnergyPerDay = 0f;
        public float CyclopsSilentRunningEnergyPerDay = 1200f;

        public bool CyclopsAlternativeCameraControls = true;

        public float CyclopsCameraRotationDamper = 3f;

        public bool CyclopsAutoCloseHatch = true;
        public void ApplyModifier()
        {
            CyclopsSettings.engineIdlingEnergyConsumption= ConvertToSeconds(CyclopsIdlingEnergyPerDay);
            CyclopsSettings.cameraRotationDamper = Mathf.Max(0.1f, CyclopsCameraRotationDamper);
            CyclopsSettings.alternativeCameraControls = CyclopsAlternativeCameraControls;
            CyclopsSettings.silentRunningEnergyConsumption = ConvertToSeconds(CyclopsSilentRunningEnergyPerDay);
            CyclopsSettings.autoCloseHatch = CyclopsAutoCloseHatch;
        }
        public static float ConvertToSeconds(float energyPerDay)
        {
            return energyPerDay / 1200f;
        }
    }
}
