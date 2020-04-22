using System;
using UnityEngine;

namespace Rm_VehicleLightsImproved
{
    public class LightsConfig
    {
        public float SeaMothLightEnergyPerDay = 0f;
        public float ExosuitLightEnergyPerDay = 0f;

        public float BaseDefaultLightEnergyPerDay = 0f;
        public float BaseEmergencyLightEnergyPerDay = 0f;

        public float CyclopsFloodLightEnergyPerDay = 0f;
        public float CyclopsCameraLightEnergyPerDay = 0f;
        public float CyclopsIdlingEnergyPerDay = 0f;
        public float CyclopsSilentRunningEnergyPerDay = 1200f;

        public float MapRoomCameraLightEnergyPerDay = 0f;



        public bool CyclopsSwapLightButtons = true;
        public bool CyclopsAlternativeCameraControls = true;

        public float CyclopsCameraRotationDamper = 3f;
        public float CyclopsCameraLightRange = 55f;
        public float CyclopsCameraLightIntensity = 1f;

        public bool BaseAutoLightDim = true;

        public KeyCode ExosuitToggleLightKey = KeyCode.Mouse2;

        public void ApplyModifier()
        {
            SeaMothSettings.lightEnergyConsumption = ConvertToSeconds(SeaMothLightEnergyPerDay);
            ExosuitSettings.lightEnergyConsumption = ConvertToSeconds(ExosuitLightEnergyPerDay);
            ExosuitSettings.exosuitToggleLightKey = ExosuitToggleLightKey;
            SubRootSettings.defaultLightEnergyConsumption = ConvertToSeconds(BaseDefaultLightEnergyPerDay);
            SubRootSettings.emergencyLightEnergyConsumption = ConvertToSeconds(BaseEmergencyLightEnergyPerDay);
            CyclopsSettings.floodLightEnergyConsumption = ConvertToSeconds(CyclopsFloodLightEnergyPerDay);
            CyclopsSettings.engineIdlingEnergyConsumption= ConvertToSeconds(CyclopsIdlingEnergyPerDay);
            CyclopsSettings.cameraRotationDamper = Mathf.Max(0.1f, CyclopsCameraRotationDamper);
            SubRootSettings.autoLightDim = BaseAutoLightDim;
            CyclopsSettings.alternativeCameraControls = CyclopsAlternativeCameraControls;
            CyclopsSettings.cameraLightRange = CyclopsCameraLightRange;
            CyclopsSettings.cameraLightIntensity = Mathf.Max(0.1f, CyclopsCameraLightIntensity);
            CyclopsSettings.cameraLightEnergyConsumption = ConvertToSeconds(CyclopsCameraLightEnergyPerDay);
            CyclopsSettings.silentRunningEnergyConsumption = ConvertToSeconds(CyclopsSilentRunningEnergyPerDay);
            CyclopsSettings.swapLightButtons = CyclopsSwapLightButtons;
            MapRoomSettings.mapRoomCameraLightEnergyConsumption = ConvertToSeconds(MapRoomCameraLightEnergyPerDay);
        }
        public static float ConvertToSeconds(float energyPerDay)
        {
            return energyPerDay / 1200f;
        }
    }
}
