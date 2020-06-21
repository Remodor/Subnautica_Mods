using UnityEngine;
using SMLHelper.V2.Options;
using Rm_Config;
using System;

namespace Rm_FastBuild
{
    class Options : ModOptions
    {
        bool constructed = false;
        public Options() : base("Fast Build")
        {
            SliderChanged += Options_SliderChanged;
            GameObjectCreated += Options_GameObjectCreated;
        }
        public void Options_SliderChanged(object sender, SliderChangedEventArgs e)
        {
            switch (e.Id)
            {
                case "BuildingSpeed":
                    Config<BuildConfig>.Get().BuildingSpeed = e.Value;
                    Changed();
                    return;
            }
        }
        private void Changed()
        {
            if (constructed)
            {
                Config<BuildConfig>.Get().ApplyModifier();
                Config<BuildConfig>.SaveConfiguration();
            }
        }

        public void Options_GameObjectCreated(object sender, GameObjectCreatedEventArgs e)
        {
            GameObject gameObject = e.GameObject;

            if (e.Id == "BuildingSpeed")
            {
                GameObject slider = gameObject.transform.Find("Slider").gameObject;
                slider.AddComponent<StepSlider_001>();
                return;
            }
        }
        private class StepSlider_001 : ModSliderOption.SliderValue
        {
            protected override void UpdateLabel()
            {
                slider.value = Mathf.Round(slider.value * 100) / 100;
                base.UpdateLabel();
            }
        }
        public override void BuildModOptions()
        {
            AddSliderOption("BuildingSpeed",         "Building speed", 0.1f, 2f, Config<BuildConfig>.Get().BuildingSpeed, 1f);
            constructed = true;
        }
    }
}
