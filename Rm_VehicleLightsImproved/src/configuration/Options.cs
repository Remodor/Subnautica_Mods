using UnityEngine;
using SMLHelper.V2.Options;
using Rm_Config;
using System;

namespace Rm_VehicleLightsImproved
{
    class Options : ModOptions
    {
        bool constructed = false;
        public Options() : base("Vehicle Lights Improved")
        {
            SliderChanged += Options_SliderChanged;
            KeybindChanged += Options_KeybindChanged;
            ToggleChanged += Options_ToggleChanged;
            GameObjectCreated += Options_GameObjectCreated;
        }
        public void Options_SliderChanged(object sender, SliderChangedEventArgs e)
        {
            switch (e.Id)
            {
                case "SeaMothLightEnergyPerDay":
                    Config<LightsConfig>.Get().SeaMothLightEnergyPerDay = e.Value;
                    Changed();
                    return;
                case "ExosuitLightEnergyPerDay":
                    Config<LightsConfig>.Get().ExosuitLightEnergyPerDay = e.Value;
                    Changed();
                    return;
                case "BaseDefaultLightEnergyPerDay":
                    Config<LightsConfig>.Get().BaseDefaultLightEnergyPerDay = e.Value;
                    Changed();
                    return;
                case "CyclopsFloodLightEnergyPerDay":
                    Config<LightsConfig>.Get().CyclopsFloodLightEnergyPerDay = e.Value;
                    Changed();
                    return;
                case "BaseEmergencyLightEnergyPerDay":
                    Config<LightsConfig>.Get().BaseEmergencyLightEnergyPerDay = e.Value;
                    Changed();
                    return;
                case "CyclopsIdlingEnergyPerDay":
                    Config<LightsConfig>.Get().CyclopsIdlingEnergyPerDay = e.Value;
                    Changed();
                    return;
                case "CyclopsCameraRotationDamper":
                    Config<LightsConfig>.Get().CyclopsCameraRotationDamper = (float)Math.Round(e.Value, 2);
                    Changed();
                    return;
                case "CyclopsCameraLightRange":
                    Config<LightsConfig>.Get().CyclopsCameraLightRange = e.Value;
                    Changed();
                    return;
                case "CyclopsCameraLightIntensity":
                    Config<LightsConfig>.Get().CyclopsCameraLightIntensity = e.Value;
                    Changed();
                    return;
                case "CyclopsCameraLightEnergyPerDay":
                    Config<LightsConfig>.Get().CyclopsCameraLightEnergyPerDay = e.Value;
                    Changed();
                    return;
                case "CyclopsSilentRunningEnergyPerDay":
                    Config<LightsConfig>.Get().CyclopsSilentRunningEnergyPerDay = e.Value;
                    Changed();
                    return;
                case "MapRoomCameraLightEnergyPerDay":
                    Config<LightsConfig>.Get().MapRoomCameraLightEnergyPerDay = e.Value;
                    Changed();
                    return;
            }
        }
        public void Options_KeybindChanged(object sender, KeybindChangedEventArgs e)
        {
            if (e.Id == "ExosuitToggleLightKey")
            {
                Config<LightsConfig>.Get().ExosuitToggleLightKey = e.Key;
                Changed();
            }
        }
        public void Options_ToggleChanged(object sender, ToggleChangedEventArgs e)
        {
            if (e.Id == "CyclopsAlternativeCameraControls")
            {
                Config<LightsConfig>.Get().CyclopsAlternativeCameraControls = e.Value;
                Changed();
            }
            if (e.Id == "BaseAutoLightDim")
            {
                Config<LightsConfig>.Get().BaseAutoLightDim = e.Value;
                Changed();
            }
            if (e.Id == "CyclopsSwapLightButtons")
            {
                Config<LightsConfig>.Get().CyclopsSwapLightButtons = e.Value;
                Changed();
            }
        }
        private void Changed()
        {
            if (constructed)
            {
                Config<LightsConfig>.Get().ApplyModifier();
                Config<LightsConfig>.SaveConfiguration();
            }
        }

        public void Options_GameObjectCreated(object sender, GameObjectCreatedEventArgs e)
        {
            GameObject gameObject = e.GameObject;

            if (e.Id == "SeaMothLightEnergyPerDay" || e.Id == "ExosuitLightEnergyPerDay"
                || e.Id == "BaseDefaultLightEnergyPerDay" || e.Id == "CyclopsFloodLightEnergyPerDay"
                || e.Id == "BaseEmergencyLightEnergyPerDay"
                || e.Id == "CyclopsIdlingEnergyPerDay"
                || e.Id == "CyclopsCameraLightRange"
                || e.Id == "MapRoomCameraLightEnergyPerDay"
                || e.Id == "CyclopsCameraLightEnergyPerDay")
            {
                GameObject slider = gameObject.transform.Find("Slider").gameObject;
                slider.AddComponent<StepSlider_1>();
                return;
            }
            if (e.Id == "CyclopsSilentRunningEnergyPerDay")
            {
                GameObject slider = gameObject.transform.Find("Slider").gameObject;
                slider.AddComponent<StepSlider_25>();
                return;
            }
        }
        private class StepSlider_1 : ModSliderOption.SliderValue
        {
            protected override void UpdateLabel()
            {
                slider.value = Mathf.Round(slider.value);
                base.UpdateLabel();
            }
        }
        private class StepSlider_25 : ModSliderOption.SliderValue
        {
            protected override void UpdateLabel()
            {
                slider.value = Mathf.Round(slider.value / 25.0f) * 25.0f;
                base.UpdateLabel();
            }
        }
        public override void BuildModOptions()
        {
            AddSliderOption("SeaMothLightEnergyPerDay",         "EnergyPerDay: Seamoth", 0f, 100f, Config<LightsConfig>.Get().SeaMothLightEnergyPerDay, 10f);
            AddSliderOption("ExosuitLightEnergyPerDay",         "EPD: Prawn Suit", 0f, 130f, Config<LightsConfig>.Get().ExosuitLightEnergyPerDay, 13f);

            AddSliderOption("BaseDefaultLightEnergyPerDay",     "EPD: Base Default Light", 0f, 70f, Config<LightsConfig>.Get().BaseDefaultLightEnergyPerDay, 7f);
            AddSliderOption("BaseEmergencyLightEnergyPerDay",   "EPD: Base Emergency Light", 0f, 30f, Config<LightsConfig>.Get().BaseEmergencyLightEnergyPerDay, 3f);

            AddSliderOption("CyclopsFloodLightEnergyPerDay",    "EPD: Cyclops Flood Light", 0f, 200f, Config<LightsConfig>.Get().CyclopsFloodLightEnergyPerDay, 20f);
            AddSliderOption("CyclopsCameraLightEnergyPerDay",   "EPD: Cyclops Camera Light", 0f, 150f, Config<LightsConfig>.Get().CyclopsCameraLightEnergyPerDay, 15f);
            AddSliderOption("CyclopsIdlingEnergyPerDay",        "EPD: Cyclops Engine Idle", 0f, 30f, Config<LightsConfig>.Get().CyclopsIdlingEnergyPerDay, 3f);
            AddSliderOption("CyclopsSilentRunningEnergyPerDay", "EPD: Cyclops Silent Running", 0f, 9000f, Config<LightsConfig>.Get().CyclopsSilentRunningEnergyPerDay, 1800f);

            AddSliderOption("MapRoomCameraLightEnergyPerDay",   "EPD: Scanner Camera Light", 0f, 80f, Config<LightsConfig>.Get().MapRoomCameraLightEnergyPerDay, 8f);


            AddToggleOption("CyclopsSwapLightButtons",          "Cyclops Swap Light Buttons", Config<LightsConfig>.Get().CyclopsSwapLightButtons);
            AddToggleOption("CyclopsAlternativeCameraControls", "Cyclops Alt. Camera Controls", Config<LightsConfig>.Get().CyclopsAlternativeCameraControls);

            AddSliderOption("CyclopsCameraRotationDamper",      "Cyclops Camera Damper", 0.1f, 5f, Config<LightsConfig>.Get().CyclopsCameraRotationDamper, 1f, "{0:F2}");
            AddSliderOption("CyclopsCameraLightRange",          "Cyclops Camera Light Range", 0f, 200f, Config<LightsConfig>.Get().CyclopsCameraLightRange, 120f);
            AddSliderOption("CyclopsCameraLightIntensity",      "Cyclops Camera Light Intensity", 0.1f, 4.0f, Config<LightsConfig>.Get().CyclopsCameraLightIntensity, 1.5f, "{0:F2}");
            
            AddToggleOption("BaseAutoLightDim",                 "Base Light Dim On Exit", Config<LightsConfig>.Get().BaseAutoLightDim);

            AddKeybindOption("ExosuitToggleLightKey",           "Prawn Suit Light Toggle", GameInput.GetPrimaryDevice(), Config<LightsConfig>.Get().ExosuitToggleLightKey);
            constructed = true;
        }
    }
}
