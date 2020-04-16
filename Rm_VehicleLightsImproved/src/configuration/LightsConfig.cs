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
        public KeyCode ExosuitToggleLightKey = KeyCode.Mouse2;

        public void ApplyModifier()
        {
            SeaMothSettings.lightEnergyConsumption = ConvertToSeconds(SeaMothLightEnergyPerDay);
            ExosuitSettings.lightEnergyConsumption = ConvertToSeconds(ExosuitLightEnergyPerDay);
            ExosuitSettings.exosuitToggleLightKey = ExosuitToggleLightKey;
            CyclopsSettings.internalLightEnergyConsumption = ConvertToSeconds(CyclopsInternalLightEnergyPerDay);
            CyclopsSettings.emergencyLightEnergyConsumption = ConvertToSeconds(CyclopsEmergencyLightEnergyPerDay);
            CyclopsSettings.externalLightEnergyConsumption = ConvertToSeconds(CyclopsExternalLightEnergyPerDay);
        }

        public static float ConvertToSeconds(float energyPerDay)
        {
            return energyPerDay / 1200f;
        }
    }
}
