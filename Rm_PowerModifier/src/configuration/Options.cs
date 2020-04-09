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

                case "SolarPanelMaxPower":
                    Config<PowerModifier>.Get().SolarPanelMaxPower = Mathf.RoundToInt(e.Value);
                    Changed();
                    return;
                case "ThermalPlantMaxPower":
                    Config<PowerModifier>.Get().ThermalPlantMaxPower = Mathf.RoundToInt(e.Value);
                    Changed();
                    return;
                case "BioReactorMaxPower":
                    Config<PowerModifier>.Get().BioReactorMaxPower = Mathf.RoundToInt(e.Value);
                    Changed();
                    return;
                case "NuclearReactorMaxPower":
                    Config<PowerModifier>.Get().NuclearReactorMaxPower = Mathf.RoundToInt(e.Value);
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
                slider.AddComponent<MultiplierSliderValue>();
                return;
            }
            if (e.Id == "SolarPanelMaxPower")
            {
                GameObject slider = gameObject.transform.Find("Slider").gameObject;
                slider.AddComponent<StepSlider_5>();
                return;
            }
            if (e.Id == "ThermalPlantMaxPower")
            {
                GameObject slider = gameObject.transform.Find("Slider").gameObject;
                slider.AddComponent<StepSlider_25>();
                return;
            }
            if (e.Id == "BioReactorMaxPower")
            {
                GameObject slider = gameObject.transform.Find("Slider").gameObject;
                slider.AddComponent<StepSlider_50>();
                return;
            }
            if (e.Id == "NuclearReactorMaxPower")
            {
                GameObject slider = gameObject.transform.Find("Slider").gameObject;
                slider.AddComponent<StepSlider_100>();
                return;
            }
        }
        private class MultiplierSliderValue : ModSliderOption.SliderValue
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
        private class StepSlider_5 : ModSliderOption.SliderValue
        {
            protected override void UpdateLabel()
            {
                slider.value = Mathf.Round(slider.value / 5.0f) * 5.0f;
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
        private class StepSlider_50 : ModSliderOption.SliderValue
        {
            protected override void UpdateLabel()
            {
                slider.value = Mathf.Round(slider.value / 50.0f) * 50.0f;
                base.UpdateLabel();
            }
        }
        private class StepSlider_100 : ModSliderOption.SliderValue
        {
            protected override void UpdateLabel()
            {
                slider.value = Mathf.Round(slider.value / 100.0f) * 100.0f;
                base.UpdateLabel();
            }
        }
        public override void BuildModOptions()
        {
            AddSliderOption("SolarPanelPowerModifier", "Modifier: Solar Panel", 0.0f, 1.0f, Config<PowerModifier>.Get().SolarPanelPowerModifier, 1);
            AddSliderOption("ThermalPlantPowerModifier", "Modifier: Thermal Plant", 0.0f, 1.0f, Config<PowerModifier>.Get().ThermalPlantPowerModifier, 1);
            AddSliderOption("BioReactorPowerModifier", "Modifier: Bioreactor", 0.0f, 1.0f, Config<PowerModifier>.Get().BioReactorPowerModifier, 1);
            AddSliderOption("NuclearReactorPowerModifier", "Modifier: Nuclear Reactor", 0.0f, 1.0f, Config<PowerModifier>.Get().NuclearReactorPowerModifier, 1);

            AddSliderOption("SolarPanelMaxPower", "Energy Capacity: Solar Panel", 0.0f, 750, Config<PowerModifier>.Get().SolarPanelMaxPower, 75f, "{0:F0}");
            AddSliderOption("ThermalPlantMaxPower", "Energy Capacity: Thermal Plant", 0.0f, 2500f, Config<PowerModifier>.Get().ThermalPlantMaxPower, 250f, "{0:F0}");
            AddSliderOption("BioReactorMaxPower", "Energy Capacity: Bioreactor", 0.0f, 5000f, Config<PowerModifier>.Get().BioReactorMaxPower, 500f, "{0:F0}");
            AddSliderOption("NuclearReactorMaxPower", "Energy Capacity: Nuclear Reactor", 0.0f, 20000f, Config<PowerModifier>.Get().NuclearReactorMaxPower, 2500f, "{0:F0}");
            constructed = true;
        }
    }
}
