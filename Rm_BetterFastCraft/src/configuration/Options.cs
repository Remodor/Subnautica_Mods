using UnityEngine;
using SMLHelper.V2.Options;
using Rm_Config;
using System;

namespace Rm_BetterFastCraft
{
    class Options : ModOptions
    {
        bool constructed = false;
        public Options() : base("Better Fast Craft")
        {
            SliderChanged += Options_SliderChanged;
            ChoiceChanged += Options_ChoiceChanged;
            GameObjectCreated += Options_GameObjectCreated;
        }

        private void Options_ChoiceChanged(object sender, ChoiceChangedEventArgs e)
        {
            switch (e.Id)
            {
                case "Mode":
                    Config<CraftConfig>.Get().Mode = (CraftConfig.ApplyMode)e.Index;
                    Changed();
                    return;
            }
        }

        public void Options_SliderChanged(object sender, SliderChangedEventArgs e)
        {
            switch (e.Id)
            {
                case "CraftingSpeed":
                    Config<CraftConfig>.Get().CraftingSpeed = e.Value;
                    Changed();
                    return;
                case "MinimumDuration":
                    Config<CraftConfig>.Get().MinimumDuration = e.Value;
                    Changed();
                    return;
            }
        }
        private void Changed()
        {
            if (constructed)
            {
                Config<CraftConfig>.Get().ApplyModifier();
                Config<CraftConfig>.SaveConfiguration();
            }
        }

        public void Options_GameObjectCreated(object sender, GameObjectCreatedEventArgs e)
        {
            GameObject gameObject = e.GameObject;

            if (e.Id == "CraftingSpeed" || e.Id == "MinimumDuration")
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
            AddSliderOption("CraftingSpeed", "Crafting Speed", 0f, 4f, Config<CraftConfig>.Get().CraftingSpeed, 1f);
            AddChoiceOption<CraftConfig.ApplyMode>("Mode", "Speed Apply Mode", Config<CraftConfig>.Get().Mode);
            AddSliderOption("MinimumDuration", "Minimum Duration", 0.1f, 4f, Config<CraftConfig>.Get().MinimumDuration, 2.7f);
            constructed = true;
        }
    }
}
