using Harmony;
using System;
using UnityEngine;

namespace Rm_VehicleLightsImproved
{
    internal static class ExosuitSettings
    {
        internal static float lightEnergyConsumption = 0f;
        internal static KeyCode exosuitToggleLightKey = KeyCode.Mouse2;
    }
    public class ExosuitCustomLight : MonoBehaviour
    {
        public ExosuitCustomLight()
        {
            lightsParent = this.transform.Find("lights_parent").gameObject;
            energyInterface = GetComponentInParent<EnergyInterface>();
            var lights = lightsParent.GetComponentsInChildren<Light>();
            lightLeft = lights[0];
            lightRight = lights[1];

            SeaMoth seaMothTemplate = SeaMothTemplate.Get();
            CreateToggleLights(seaMothTemplate);
            var offset = new Vector3(0, 0.41f, -0.37f);
            CustomVolumetricLight.CreateVolumetricLight(lightLeft, seaMothTemplate, offset);
            CustomVolumetricLight.CreateVolumetricLight(lightRight, seaMothTemplate, offset);
        }
        private void CreateToggleLights(SeaMoth seaMothTemplate)
        {
            var seaMothToggleLights = seaMothTemplate.toggleLights;
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
        private void FixLightsActive()
        {
            lightLeft.gameObject.SetActive(true);
            lightRight.gameObject.SetActive(true);
        }
        private void UpdateLightEnergy()
        {
            if (lightIsActive)
            {
                float energyCost = DayNightCycle.main.deltaTime * ExosuitSettings.lightEnergyConsumption;
                float consumedEnergy = energyInterface.ConsumeEnergy(energyCost);
                if (consumedEnergy == 0 && energyCost != 0 && GameModeUtils.RequiresPower())
                {
                    SetLightsActive(false);
                }
            }
        }
        private void Update()
        {
            if (base.gameObject.activeInHierarchy)
            {
                FixLightsActive();
                UpdateLightEnergy();
                if (isPilotMode
                    && Input.GetKeyUp(ExosuitSettings.exosuitToggleLightKey)
                    && energyInterface.hasCharge
                    && !Player.main.GetPDA().isInUse
                    && global::Utils.GetLocalPlayerComp().GetMode() == Player.Mode.LockedPiloting)
                {
                    ToggleLights();
                }
            }
        }
        public bool IsLightActive()
        {
            return lightIsActive;
        }
        private readonly Light lightLeft;
        private readonly Light lightRight;
        public bool isPilotMode = false;
        private bool lightIsActive = true;
        private readonly GameObject lightsParent;
        private FMOD_StudioEventEmitter lightsOnSound;
        private FMOD_StudioEventEmitter lightsOffSound;
        private FMODAsset onSound;
        private FMODAsset offSound;
        private readonly EnergyInterface energyInterface;
    }
}
