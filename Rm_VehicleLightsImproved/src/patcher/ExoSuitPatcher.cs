using Harmony;
using System;
using UnityEngine;

namespace Rm_VehicleLightsImproved
{
    internal static class ExosuitSettings
    {
        internal static float lightEnergyConsumption = 0f;
        internal static KeyCode exosuitToggleLightKey = (KeyCode)GameInput.Button.Deconstruct;
    }
    internal static class SeaMothTemplate
    {
        internal static SeaMoth SeaMoth = CraftData.InstantiateFromPrefab(TechType.Seamoth).GetComponent<SeaMoth>();
    }
    public class ToggleLightExosuit : MonoBehaviour
    {
        public ToggleLightExosuit()
        {
            lightsParent = this.transform.Find("lights_parent").gameObject;
            energyInterface = GetComponentInParent<Vehicle>().energyInterface;
            var seaMothToggleLights = SeaMothTemplate.SeaMoth.toggleLights;
            lightsOnSound = seaMothToggleLights.lightsOnSound;
            lightsOffSound = seaMothToggleLights.lightsOffSound;
            onSound = seaMothToggleLights.onSound;
            offSound = seaMothToggleLights.offSound;
        }

        public bool ToggleLights()
        {
            SetLightsActive(!lightIsActive);
            return lightIsActive;
        }
        public void SetLightsActive(bool isActive)
        {
            if (lightIsActive != isActive)
            {
                lightIsActive = isActive;
                lightsParent.SetActive(lightIsActive);
                if (lightIsActive)
                {
                    Utils.PlayEnvSound(this.lightsOnSound, this.lightsOnSound.gameObject.transform.position, 20f);
                    Utils.PlayFMODAsset(this.onSound, base.transform, 20f);
                }
                else
                {
                    Utils.PlayEnvSound(this.lightsOffSound, this.lightsOffSound.gameObject.transform.position, 20f);
                    Utils.PlayFMODAsset(this.offSound, base.transform, 20f);
                }
            }
        }
        public void SyncLight()
        {
            lightsParent.SetActive(lightIsActive);
        }
        private void UpdateLightEnergy()
        {
            if (lightIsActive)
            {
                float energyCost = DayNightCycle.main.deltaTime * ExosuitSettings.lightEnergyConsumption;
                float consumedEnergy = energyInterface.ConsumeEnergy(energyCost);
                if (consumedEnergy == 0 && energyCost != 0)
                {
                    SetLightsActive(false);
                }
            }
        }
        private void Update()
        {
            if (base.gameObject.activeInHierarchy)
            {
                UpdateLightEnergy();
                if (isPilotMode 
                    && Input.GetKeyDown(ExosuitSettings.exosuitToggleLightKey) 
                    && energyInterface.hasCharge 
                    && !Player.main.GetPDA().isInUse
                    && global::Utils.GetLocalPlayerComp().GetMode() == Player.Mode.LockedPiloting)
                {
                    ToggleLights();
                }
            }
        }
        public bool isPilotMode = false;
        private bool lightIsActive = true;
        private GameObject lightsParent;
        private FMOD_StudioEventEmitter lightsOnSound;
        private FMOD_StudioEventEmitter lightsOffSound;
        private FMODAsset onSound;
        private FMODAsset offSound;
        private EnergyInterface energyInterface;
    }
    [HarmonyPatch(typeof(Exosuit))]
    [HarmonyPatch(nameof(Exosuit.Start))]
    internal class Exosuit_Start_Patch
    {
        [HarmonyPostfix]
        internal static void Postfix(Exosuit __instance)
        {
            var exoToggleLights = __instance.gameObject.AddComponent<ToggleLightExosuit>();
            exoToggleLights.SetLightsActive(false);
        }
    }
    [HarmonyPatch(typeof(Exosuit))]
    [HarmonyPatch(nameof(Exosuit.OnPilotModeBegin))]
    internal static class Exosuit_OnPilotModeBegin_Patch
    {
        private static void Postfix(Exosuit __instance)
        {
            __instance.GetAllComponentsInChildren<VFXVolumetricLight>()
                        .ForEach(x => x.DisableVolume());
            __instance.GetComponent<ToggleLightExosuit>().isPilotMode = true;
        }
    }
    [HarmonyPatch(typeof(Exosuit))]
    [HarmonyPatch(nameof(Exosuit.OnPilotModeEnd))]
    internal static class Exosuit_OnPilotModeEnd_Patch
    {
        private static void Postfix(Exosuit __instance)
        {
            __instance.GetAllComponentsInChildren<VFXVolumetricLight>()
                        .ForEach(x => x.RestoreVolume());
            __instance.GetComponent<ToggleLightExosuit>().isPilotMode = false;
        }
    }
    [HarmonyPatch(typeof(Exosuit))]
    [HarmonyPatch(nameof(Exosuit.OnDockedChanged))]
    internal class Exosuit_OnDockedChanged_Patch
    {
        [HarmonyPostfix]
        internal static void Postfix(Exosuit __instance, bool docked)
        {
            if (docked)
            {
                __instance.GetComponent<ToggleLightExosuit>().SetLightsActive(false);
            }
        }
    }
    [HarmonyPatch(typeof(Exosuit))]
    [HarmonyPatch(nameof(Exosuit.OnPoweredChanged))]
    internal class Exosuit_OnPoweredChanged_Patch
    {
        [HarmonyPrefix]
        internal static void Postfix(Exosuit __instance, bool powered)
        {
            var exoToggleLight = __instance.GetComponent<ToggleLightExosuit>();
            if (exoToggleLight)
            {
                exoToggleLight.SyncLight();
                Console.WriteLine("#80 toggled light");
            }
        }
    }
}
