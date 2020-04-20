using System;
using UnityEngine;

namespace Rm_VehicleLightsImproved
{
    public class LightsConfig
    {
        public float SeaMothLightEnergyPerDay = 50f;
        public float ExosuitLightEnergyPerDay = 50f;
        public float CyclopsInternalLightEnergyPerDay = 25f;
        public float CyclopsEmergencyLightEnergyPerDay = 10f;
        public float CyclopsExternalLightEnergyPerDay = 100f;
        public float CyclopsCameraLightEnergyPerDay = 75f;

        public float CyclopsIdlingEnergyPerDay = 10f;
        public float CyclopsSilentRunningEnergyPerDay = 1200f;

        public bool CyclopsAlternativeCameraControls = true;
        public float CyclopsCameraRotationDamper = 3f;
        public float CyclopsCameraLightRange = 55f;
        public float CyclopsCameraLightIntensity = 1f;

        public KeyCode ExosuitToggleLightKey = KeyCode.Mouse2;

        public void ApplyModifier()
        {
            SeaMothSettings.lightEnergyConsumption = ConvertToSeconds(SeaMothLightEnergyPerDay);
            ExosuitSettings.lightEnergyConsumption = ConvertToSeconds(ExosuitLightEnergyPerDay);
            ExosuitSettings.exosuitToggleLightKey = ExosuitToggleLightKey;
            CyclopsSettings.internalLightEnergyConsumption = ConvertToSeconds(CyclopsInternalLightEnergyPerDay);
            CyclopsSettings.emergencyLightEnergyConsumption = ConvertToSeconds(CyclopsEmergencyLightEnergyPerDay);
            CyclopsSettings.externalLightEnergyConsumption = ConvertToSeconds(CyclopsExternalLightEnergyPerDay);
            CyclopsSettings.engineIdlingEnergyConsumption= ConvertToSeconds(CyclopsIdlingEnergyPerDay);
            CyclopsSettings.cameraRotationDamper = Mathf.Max(0.1f, CyclopsCameraRotationDamper);
            CyclopsSettings.alternativeCameraControls = CyclopsAlternativeCameraControls;
            CyclopsSettings.cyclopsCameraLightRange = CyclopsCameraLightRange;
            CyclopsSettings.cyclopsCameraLightIntensity = Mathf.Max(0.1f, CyclopsCameraLightIntensity);
            CyclopsSettings.cyclopsCameraLightEnergyConsumption = ConvertToSeconds(CyclopsCameraLightEnergyPerDay);
            CyclopsSettings.CyclopsSilentRunningEnergyConsumption = ConvertToSeconds(CyclopsSilentRunningEnergyPerDay);
        }

        public static float ConvertToSeconds(float energyPerDay)
        {
            return energyPerDay / 1200f;
        }
    }
}
