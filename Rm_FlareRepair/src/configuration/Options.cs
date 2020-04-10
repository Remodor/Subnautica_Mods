using UnityEngine;
using SMLHelper.V2.Options;
using Rm_Config;
using System;

namespace Rm_FlareRepair
{
    class Options : ModOptions
    {
        bool constructed = false;
        public Options() : base("Flare Repair Settings")
        {
            SliderChanged += Options_SliderChanged;
            GameObjectCreated += Options_GameObjectCreated;
        }
        public void Options_SliderChanged(object sender, SliderChangedEventArgs e)
        {
            switch (e.Id)
            {
                case "FlareTotalDuration":
                    Config<FlareConfig>.Get().TotalDuration = e.Value;
                    Changed();
                    return;
                case "FlareIntensityModifier":
                    Config<FlareConfig>.Get().IntensityModifier = e.Value;
                    Changed();
                    return;
            }
        }
        private void Changed()
        {
            if (constructed)
            {
                Config<FlareConfig>.Get().ApplyConfig();
                Config<FlareConfig>.SaveConfiguration();
            }
        }

        public void Options_GameObjectCreated(object sender, GameObjectCreatedEventArgs e)
        {
            GameObject gameObject = e.GameObject;

            if (e.Id == "FlareTotalDuration" || e.Id == "FlareIntensityModifier")
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
                    return (float)Math.Round((double)sliderValue * 2, 4);
                }
                else if (sliderValue < 0.75f)
                {
                    ValueFormat = "{0:F1}";
                    return (float)Math.Round((double)(sliderValue - 0.5f) * 36 + 1, 4);
                }
                ValueFormat = "{0:F0}";
                return (float)Math.Round((double)(sliderValue - 0.75f) * 360 + 10, 4);
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
            AddSliderOption("FlareTotalDuration", "Total Duration (Game Days)", 0.0f, 1.0f, Config<FlareConfig>.Get().TotalDuration, 0.5f);
            AddSliderOption("FlareIntensityModifier", "Intensity Modifier", 0.0f, 1.0f, Config<FlareConfig>.Get().IntensityModifier, 1.0f);
            constructed = true;
        }
    }
}
