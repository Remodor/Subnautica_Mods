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
            }
        }
        public void Options_KeybindChanged(object sender, KeybindChangedEventArgs e)
        {
            if (e.Id != "ExosuitToggleLightKey") return;
            Config<LightsConfig>.Get().ExosuitToggleLightKey = e.Key;
            Changed();
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

            if (e.Id == "SeaMothLightEnergyPerDay" || e.Id == "ExosuitLightEnergyPerDay")
            {
                GameObject slider = gameObject.transform.Find("Slider").gameObject;
                slider.AddComponent<StepSlider_1>();
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
        public override void BuildModOptions()
        {
            AddSliderOption("SeaMothLightEnergyPerDay", "Energy (per Day): Seamoth", 0f, 200f, Config<LightsConfig>.Get().SeaMothLightEnergyPerDay, 50f);
            AddSliderOption("ExosuitLightEnergyPerDay", "Energy (per Day): Prawn Suit", 0f, 200f, Config<LightsConfig>.Get().ExosuitLightEnergyPerDay, 50f);
            AddKeybindOption("ExosuitToggleLightKey", "Exosuit light toggle", GameInput.GetPrimaryDevice(), Config<LightsConfig>.Get().ExosuitToggleLightKey);
            constructed = true;
        }
    }
}
