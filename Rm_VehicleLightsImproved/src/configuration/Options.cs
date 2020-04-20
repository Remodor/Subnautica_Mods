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
                case "CyclopsInternalLightEnergyPerDay":
                    Config<LightsConfig>.Get().CyclopsInternalLightEnergyPerDay = e.Value;
                    Changed();
                    return;
                case "CyclopsExternalLightEnergyPerDay":
                    Config<LightsConfig>.Get().CyclopsExternalLightEnergyPerDay = e.Value;
                    Changed();
                    return;
                case "CyclopsEmergencyLightEnergyPerDay":
                    Config<LightsConfig>.Get().CyclopsEmergencyLightEnergyPerDay = e.Value;
                    Changed();
                    return;
                case "CyclopsIdlingEnergyPerDay":
                    Config<LightsConfig>.Get().CyclopsIdlingEnergyPerDay = e.Value;
                    Changed();
                    return;
                case "CyclopsCameraRotationDamper":
                    Config<LightsConfig>.Get().CyclopsCameraRotationDamper = e.Value;
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
                || e.Id == "CyclopsInternalLightEnergyPerDay" || e.Id == "CyclopsExternalLightEnergyPerDay"
                || e.Id == "CyclopsEmergencyLightEnergyPerDay"
                || e.Id == "CyclopsIdlingEnergyPerDay"
                || e.Id == "CyclopsCameraLightRange"
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
            AddSliderOption("SeaMothLightEnergyPerDay", "EnergyPerDay: Seamoth", 0f, 200f, Config<LightsConfig>.Get().SeaMothLightEnergyPerDay, 50f);
            AddSliderOption("ExosuitLightEnergyPerDay", "EPD: Prawn Suit", 0f, 200f, Config<LightsConfig>.Get().ExosuitLightEnergyPerDay, 50f);
            AddSliderOption("CyclopsInternalLightEnergyPerDay", "EPD: Cyclops Internal", 0f, 200f, Config<LightsConfig>.Get().CyclopsInternalLightEnergyPerDay, 25f);
            AddSliderOption("CyclopsEmergencyLightEnergyPerDay", "EPD: Cyclops Emergency", 0f, 200f, Config<LightsConfig>.Get().CyclopsEmergencyLightEnergyPerDay, 10f);
            AddSliderOption("CyclopsExternalLightEnergyPerDay", "EPD: Cyclops External", 0f, 200f, Config<LightsConfig>.Get().CyclopsExternalLightEnergyPerDay, 100f);
            AddSliderOption("CyclopsCameraLightEnergyPerDay", "EPD: Cyclops Camera Light", 0f, 200f, Config<LightsConfig>.Get().CyclopsCameraLightEnergyPerDay, 75f);
            AddSliderOption("CyclopsIdlingEnergyPerDay", "EPD: Cyclops Engine Idle", 0f, 200f, Config<LightsConfig>.Get().CyclopsIdlingEnergyPerDay, 10f);
            AddSliderOption("CyclopsSilentRunningEnergyPerDay", "EPD: Cyclops Silent Running", 0f, 4800f, Config<LightsConfig>.Get().CyclopsSilentRunningEnergyPerDay, 1200f);

            AddToggleOption("CyclopsAlternativeCameraControls", "Cyclops Alternative Camera Controls", Config<LightsConfig>.Get().CyclopsAlternativeCameraControls);
            AddSliderOption("CyclopsCameraLightRange", "Cyclops Camera L.Range", 0f, 200f, Config<LightsConfig>.Get().CyclopsCameraLightRange, 55f);
            AddSliderOption("CyclopsCameraLightIntensity", "Cyclops Camera L.Intensity", 0.1f, 2.0f, Config<LightsConfig>.Get().CyclopsCameraLightIntensity, 1.0f, "{0:F2}");
            AddSliderOption("CyclopsCameraRotationDamper", "Cyclops Camera Damper", 0.1f, 5f, Config<LightsConfig>.Get().CyclopsCameraRotationDamper, 3f, "{0:F2}");

            AddKeybindOption("ExosuitToggleLightKey", "Exosuit light toggle", GameInput.GetPrimaryDevice(), Config<LightsConfig>.Get().ExosuitToggleLightKey);
            constructed = true;
        }
    }
}
