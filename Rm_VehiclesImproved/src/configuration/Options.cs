using UnityEngine;
using SMLHelper.V2.Options;
using Rm_Config;
using System;

namespace Rm_VehiclesImproved
{
    class Options : ModOptions
    {
        bool constructed = false;
        public Options() : base("Vehicles Improved")
        {
            SliderChanged += Options_SliderChanged;
            ToggleChanged += Options_ToggleChanged;
            GameObjectCreated += Options_GameObjectCreated;
        }
        public void Options_SliderChanged(object sender, SliderChangedEventArgs e)
        {
            switch (e.Id)
            {
                case "CyclopsIdlingEnergyPerDay":
                    Config<LightsConfig>.Get().CyclopsIdlingEnergyPerDay = e.Value;
                    Changed();
                    return;
                case "CyclopsCameraRotationDamper":
                    Config<LightsConfig>.Get().CyclopsCameraRotationDamper = (float)Math.Round(e.Value, 2);
                    Changed();
                    return;
                case "CyclopsSilentRunningEnergyPerDay":
                    Config<LightsConfig>.Get().CyclopsSilentRunningEnergyPerDay = e.Value;
                    Changed();
                    return;
            }
        }
        public void Options_ToggleChanged(object sender, ToggleChangedEventArgs e)
        {
            if (e.Id == "CyclopsAlternativeCameraControls")
            {
                Config<LightsConfig>.Get().CyclopsAlternativeCameraControls = e.Value;
                Changed();
            }
            if (e.Id == "CyclopsAutoCloseHatch")
            {
                Config<LightsConfig>.Get().CyclopsAutoCloseHatch = e.Value;
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

            if (e.Id == "CyclopsIdlingEnergyPerDay")
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
            AddSliderOption("CyclopsIdlingEnergyPerDay",        "EPD: Cyclops Engine Idle", 0f, 30f, Config<LightsConfig>.Get().CyclopsIdlingEnergyPerDay, 3f);
            AddSliderOption("CyclopsSilentRunningEnergyPerDay", "EPD: Cyclops Silent Running", 0f, 6000f, Config<LightsConfig>.Get().CyclopsSilentRunningEnergyPerDay, 1200f);

            AddToggleOption("CyclopsAlternativeCameraControls", "Cyclops Alt. Camera Controls", Config<LightsConfig>.Get().CyclopsAlternativeCameraControls);
            AddToggleOption("CyclopsAutoCloseHatch", "Cyclops Auto Close Hatch", Config<LightsConfig>.Get().CyclopsAutoCloseHatch);

            AddSliderOption("CyclopsCameraRotationDamper",      "Cyclops Camera Damper", 0.01f, 5f, Config<LightsConfig>.Get().CyclopsCameraRotationDamper, 1f, "{0:F2}");
            constructed = true;
        }
    }
}
