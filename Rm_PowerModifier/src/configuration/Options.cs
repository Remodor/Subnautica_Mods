using UnityEngine;
using SMLHelper.V2.Options;
using Rm_Config;
using System;

namespace Rm_PowerModifier
{
    class Options : ModOptions
    {
        bool constructed = false;
        public Options() : base("Power Modifier")
        {
            SliderChanged += Options_SliderChanged;
            GameObjectCreated += Options_GameObjectCreated;
        }
        public void Options_SliderChanged(object sender, SliderChangedEventArgs e)
        {
            switch (e.Id)
            {
                case "SolarPanelPowerModifier":
                    Config<PowerModifier>.Get().SolarPanelPowerModifier = e.Value;
                    Changed();
                    return;
                case "ThermalPlantPowerModifier":
                    Config<PowerModifier>.Get().ThermalPlantPowerModifier = e.Value;
                    Changed();
                    return;
                case "BioReactorPowerModifier":
                    Config<PowerModifier>.Get().BioReactorPowerModifier = e.Value;
                    Changed();
                    return;
                case "NuclearReactorPowerModifier":
                    Config<PowerModifier>.Get().NuclearReactorPowerModifier = e.Value;
                    Changed();
                    return;
            }
        }
        private void Changed()
        {
            if (constructed)
            {
                Config<PowerModifier>.Get().ApplyModifier();
                Config<PowerModifier>.SaveConfiguration();
            }
        }

        public void Options_GameObjectCreated(object sender, GameObjectCreatedEventArgs e)
        {
            GameObject gameObject = e.GameObject;

            if (e.Id == "SolarPanelPowerModifier" || e.Id == "ThermalPlantPowerModifier" 
                || e.Id == "BioReactorPowerModifier" || e.Id == "NuclearReactorPowerModifier")
            {
                GameObject slider = gameObject.transform.Find("Slider").gameObject;
                slider.AddComponent<CustomSliderValue>();
            }
        }
        private class CustomSliderValue : ModSliderOption.SliderValue
        {
            public override float ConvertToDisplayValue(float sliderValue)
            {

                if (sliderValue < 0.5f)
                {
                    ValueFormat = "{0:F2}";
                    return (float)Math.Round((double) sliderValue * 2, 4);
                } else if (sliderValue < 0.75f)
                {
                    ValueFormat = "{0:F1}";
                    return (float)Math.Round((double) (sliderValue - 0.5f) * 36 + 1, 4);
                }
                ValueFormat = "{0:F0}";
                return (float)Math.Round((double) (sliderValue - 0.75f) * 360 + 10, 4);
            }
            public override float ConvertToSliderValue(float displayValue)
            {
                if (displayValue < 1)
                {
                    return displayValue * 0.5f;
                }
                else if (displayValue < 10)
                {
                    return (displayValue - 1) / 36 + 0.5f;
                }
                return (displayValue - 10) / 360 + 0.75f;
            }
        }
        public override void BuildModOptions()
        {
            AddSliderOption("SolarPanelPowerModifier", "SolarPanel", 0.0f, 1.0f, Config<PowerModifier>.Get().SolarPanelPowerModifier, 1);
            AddSliderOption("ThermalPlantPowerModifier", "ThermalPlant", 0.0f, 1.0f, Config<PowerModifier>.Get().ThermalPlantPowerModifier, 1);
            AddSliderOption("BioReactorPowerModifier", "BaseBioReactor", 0.0f, 1.0f, Config<PowerModifier>.Get().BioReactorPowerModifier, 1);
            AddSliderOption("NuclearReactorPowerModifier", "BaseNuclearReactor", 0.0f, 1.0f, Config<PowerModifier>.Get().NuclearReactorPowerModifier, 1);

            constructed = true;
        }
    }
}
