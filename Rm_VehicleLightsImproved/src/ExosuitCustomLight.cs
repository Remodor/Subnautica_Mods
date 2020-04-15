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
    public class ExosuitCustomLight : MonoBehaviour
    {
        public ExosuitCustomLight()
        {
            lightsParent = this.transform.Find("lights_parent").gameObject;
            energyInterface = GetComponentInParent<EnergyInterface>();
            var lights = lightsParent.GetComponentsInChildren<Light>();
            lightLeft = lights[0];
            lightRight = lights[1];

            CreateToggleLights();
            CreateVolumetricLights();
        }
        private void CreateToggleLights()
        {
            var seaMothToggleLights = SeaMothTemplate.SeaMoth.toggleLights;
            lightsOnSound = seaMothToggleLights.lightsOnSound;
            lightsOffSound = seaMothToggleLights.lightsOffSound;
            onSound = seaMothToggleLights.onSound;
            offSound = seaMothToggleLights.offSound;
        }
        private void CreateVolumetricLights()
        {
            CreateVolumetricLight(lightLeft);
            CreateVolumetricLight(lightRight);
        }
        private void CreateVolumetricLight(Light light)
        {
            var templateVolumetricLight = SeaMothTemplate.SeaMoth.toggleLights.lightsParent.GetComponentInChildren<VFXVolumetricLight>();
            var volumetricLight = light.gameObject.AddComponent<VFXVolumetricLight>();

            System.Reflection.FieldInfo[] volumetricLightFields = volumetricLight.GetType().GetFields();
            foreach (System.Reflection.FieldInfo field in volumetricLightFields)
            {
                field.SetValue(volumetricLight, field.GetValue(templateVolumetricLight));
            }

            var volume = GameObject.Instantiate(templateVolumetricLight.volumGO, light.transform).gameObject;
            volume.transform.localScale = CalculateLightCone(light);
            var offset = new Vector3(0, 0.41f, -0.37f);
            light.transform.localPosition += offset;

            volumetricLight.volumGO = volume;
            volumetricLight.lightSource = light;
            volumetricLight.block = null;
            volumetricLight.angle = (int)light.spotAngle;
            volumetricLight.intensity = 0.45f;

            volumetricLight.Awake();
        }
        public Vector3 CalculateLightCone(Light light)
        {
            return CalculateLightCone(light.range, light.spotAngle);
        }
        public Vector3 CalculateLightCone(float range, float angle)
        {
            var opposite = Mathf.Tan(Mathf.Deg2Rad * angle / 2) * range;
            var xy = opposite * 2;
            var cone = new Vector3
            (
                xy,
                xy,
                range
            );
            return cone;
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
                FixLightsActive();
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
