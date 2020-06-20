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

        public float MapRoomCameraLightEnergyPerDay = 0f;

        public bool CyclopsSwapLightButtons = true;

        public float CyclopsCameraLightRange = 55f;
        public float CyclopsCameraLightIntensity = 1f;

        public bool BaseAutoLightDim = true;
        public float BaseLightFadeDuration = 1.5f;
        public bool IncludeBaseLights = true;

        public KeyCode ExosuitToggleLightKey = KeyCode.Mouse2;
        public bool ExosuitToggleLightHud = true;

        public void ApplyModifier()
        {
            SeaMothSettings.lightEnergyConsumption = ConvertToSeconds(SeaMothLightEnergyPerDay);
            ExosuitSettings.lightEnergyConsumption = ConvertToSeconds(ExosuitLightEnergyPerDay);
            ExosuitSettings.toggleLightKey = ExosuitToggleLightKey;
            ExosuitSettings.toggleLightHud = ExosuitToggleLightHud;
            SubRootSettings.defaultLightEnergyConsumption = ConvertToSeconds(BaseDefaultLightEnergyPerDay);
            SubRootSettings.emergencyLightEnergyConsumption = ConvertToSeconds(BaseEmergencyLightEnergyPerDay);
            CyclopsSettings.floodLightEnergyConsumption = ConvertToSeconds(CyclopsFloodLightEnergyPerDay);
            SubRootSettings.autoLightDim = BaseAutoLightDim;
            CyclopsSettings.cameraLightRange = CyclopsCameraLightRange;
            CyclopsSettings.cameraLightIntensity = Mathf.Max(0.1f, CyclopsCameraLightIntensity);
            CyclopsSettings.cameraLightEnergyConsumption = ConvertToSeconds(CyclopsCameraLightEnergyPerDay);
            CyclopsSettings.swapLightButtons = CyclopsSwapLightButtons;
            MapRoomSettings.mapRoomCameraLightEnergyConsumption = ConvertToSeconds(MapRoomCameraLightEnergyPerDay);
            SubRootSettings.includeBaseLights = IncludeBaseLights;
            SubRootSettings.lightFadeDuration = BaseLightFadeDuration;
        }
        public static float ConvertToSeconds(float energyPerDay)
        {
            return energyPerDay / 1200f;
        }
    }
}
