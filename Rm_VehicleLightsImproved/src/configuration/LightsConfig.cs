using UnityEngine;

namespace Rm_VehicleLightsImproved
{
    public class LightsConfig
    {
        public float SeaMothLightEnergyPerDay = 50f;
        public float ExosuitLightEnergyPerDay = 50f;
        public KeyCode ExosuitToggleLightKey = (KeyCode)GameInput.Button.Deconstruct;

        public void ApplyModifier()
        {
            SeaMothSettings.lightEnergyConsumption = ConvertToSeconds(SeaMothLightEnergyPerDay);
            ExosuitSettings.lightEnergyConsumption = ConvertToSeconds(ExosuitLightEnergyPerDay);
            ExosuitSettings.exosuitToggleLightKey = ExosuitToggleLightKey;
        }

        public static float ConvertToSeconds(float energyPerDay)
        {
            return energyPerDay / 1200f;
        }
    }
}
