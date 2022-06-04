using UnityEngine;
using SMLHelper.V2.Options;
using Rm_Config;
using System;

namespace Rm_HudTweaks
{
    class Options : ModOptions
    {
        bool constructed = false;
        public Options() : base("Hud Tweaks Settings")
        {
            ToggleChanged += Options_ToggleChanged;
            SliderChanged += Options_SliderChanged;
        }

        public void Options_ToggleChanged(object sender, ToggleChangedEventArgs e)
        {
            switch (e.Id)
            {
                case "HideNames":
                    Config<HudTweaksConfig>.Get().HideNames = e.Value;
                    Changed();
                    return;
                case "DisableText2":
                    Config<HudTweaksConfig>.Get().DisableText2 = e.Value;
                    Changed();
                    return;
                case "HideToolControlsText":
                    Config<HudTweaksConfig>.Get().HideToolControlsText = e.Value;
                    Changed();
                    return;
                case "HideToolInfo":
                    Config<HudTweaksConfig>.Get().HideToolInfo = e.Value;
                    Changed();
                    return;
                case "DisableInputIcon":
                    Config<HudTweaksConfig>.Get().DisableInputIcon = e.Value;
                    Changed();
                    return;
                case "DisableCrosshair":
                    Config<HudTweaksConfig>.Get().DisableCrosshair = e.Value;
                    Changed();
                    return;
                case "AllowCrosshairCyclops":
                    Config<HudTweaksConfig>.Get().AllowCrosshairCyclops = e.Value;
                    Changed();
                    return;
                case "AllowCrosshairCyclopsOnlyOnHover":
                    Config<HudTweaksConfig>.Get().AllowCrosshairCyclopsOnlyOnHover = e.Value;
                    Changed();
                    return;
                case "ShowCrosshairInteractIcons":
                    Config<HudTweaksConfig>.Get().ShowCrosshairInteractIcons = e.Value;
                    Changed();
                    return;
            }
        }

        public void Options_SliderChanged(object sender, SliderChangedEventArgs e)
        {
            switch (e.Id)
            {
                case "CrosshairScale":
                    Config<HudTweaksConfig>.Get().CrosshairScale = e.Value;
                    Changed();
                    return;
                case "CyclopsCrosshairIcon":
                    Config<HudTweaksConfig>.Get().CyclopsCrosshairIcon = (int) e.Value;
                    Changed();
                    return;
                case "CyclopsSpeedCrosshairLingerTime":
                    Config<HudTweaksConfig>.Get().CyclopsSpeedCrosshairLingerTime = e.Value;
                    Changed();
                    return;
            }
        }

        private void Changed()
        {
            if (constructed)
            {
                Config<HudTweaksConfig>.SaveConfiguration();
            }
        }
        public override void BuildModOptions()
        {
            AddToggleOption("HideNames", "Hide Names", Config<HudTweaksConfig>.Get().HideNames);
            AddToggleOption("DisableText2", "Disable Text 2", Config<HudTweaksConfig>.Get().DisableText2);
            AddToggleOption("HideToolControlsText", "Hide Tool Controls", Config<HudTweaksConfig>.Get().HideToolControlsText);
            AddToggleOption("HideToolInfo", "Hide Tool Info", Config<HudTweaksConfig>.Get().HideToolInfo);
            AddToggleOption("DisableInputIcon", "Disable Input Icon", Config<HudTweaksConfig>.Get().DisableInputIcon);
            AddSliderOption("CrosshairScale", "Crosshair Scale", 0.01f, 2, Config<HudTweaksConfig>.Get().CrosshairScale, 1, "{0:F2}", 0.01f);
            AddToggleOption("DisableCrosshair",                 "Disable Crosshair", Config<HudTweaksConfig>.Get().DisableCrosshair);
            AddToggleOption("ShowCrosshairInteractIcons",       "    Show Interact Icons", Config<HudTweaksConfig>.Get().ShowCrosshairInteractIcons);
            AddToggleOption("AllowCrosshairCyclops",            "    Allow for Cyclops", Config<HudTweaksConfig>.Get().AllowCrosshairCyclops);
            AddSliderOption("CyclopsCrosshairIcon",             "        Crosshair Icon", 1, 10, Config<HudTweaksConfig>.Get().CyclopsCrosshairIcon, 1, "{0:F0}", 1f);
            AddToggleOption("AllowCrosshairCyclopsOnlyOnHover", "        Only on Hover", Config<HudTweaksConfig>.Get().AllowCrosshairCyclopsOnlyOnHover);
            AddSliderOption("CyclopsSpeedCrosshairLingerTime",  "        Speed Linger Time", 0.1f, 3f, Config<HudTweaksConfig>.Get().CyclopsSpeedCrosshairLingerTime, 0.5f, "{0:F1}", 0.1f);
            constructed = true;
        }
    }
}
