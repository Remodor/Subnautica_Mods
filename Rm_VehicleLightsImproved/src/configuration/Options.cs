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

            if (e.Id == "SeaMothLightEnergyPerDay" || e.Id == "ExosuitLightEnergyPerDay"
                || e.Id == "CyclopsInternalLightEnergyPerDay" || e.Id == "CyclopsExternalLightEnergyPerDay"
                || e.Id == "CyclopsEmergencyLightEnergyPerDay")
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
            AddSliderOption("SeaMothLightEnergyPerDay", "E.p.Day: Seamoth", 0f, 200f, Config<LightsConfig>.Get().SeaMothLightEnergyPerDay, 50f);
            AddSliderOption("ExosuitLightEnergyPerDay", "E.p.Day: Prawn Suit", 0f, 200f, Config<LightsConfig>.Get().ExosuitLightEnergyPerDay, 50f);
            AddSliderOption("CyclopsInternalLightEnergyPerDay", "E.p.Day: Cyclops Internal", 0f, 200f, Config<LightsConfig>.Get().CyclopsInternalLightEnergyPerDay, 25f);
            AddSliderOption("CyclopsEmergencyLightEnergyPerDay", "E.p.Day: Cyclops Emergency", 0f, 200f, Config<LightsConfig>.Get().CyclopsEmergencyLightEnergyPerDay, 10f);
            AddSliderOption("CyclopsExternalLightEnergyPerDay", "E.p.Day: Cyclops External", 0f, 200f, Config<LightsConfig>.Get().CyclopsExternalLightEnergyPerDay, 100f);
            AddKeybindOption("ExosuitToggleLightKey", "Exosuit light toggle", GameInput.GetPrimaryDevice(), Config<LightsConfig>.Get().ExosuitToggleLightKey);
            constructed = true;
        }
    }
}
