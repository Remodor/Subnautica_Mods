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
            var exosuit = GetComponentInParent<Exosuit>();
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
                if (field != null)
                {
                    Console.WriteLine("#2 {0}: {1}, {2}", field, field.GetValue(templateVolumetricLight), field.GetValue(volumetricLight));
                }
            }

            var volume = GameObject.Instantiate(templateVolumetricLight.volumGO, light.transform).gameObject;
            volume.transform.localScale = CalculateLightCone(light);
            var offset = new Vector3(0, 0.41f, -0.37f);
            light.transform.localPosition += offset;
            //volume.transform.localPosition = offset;


            volumetricLight.volumGO = volume;
            volumetricLight.lightSource = light;
            volumetricLight.block = null;
            volumetricLight.angle = (int)light.spotAngle;
            volumetricLight.intensity = 0.45f;
            Console.WriteLine("#6 lightRotation: {0} / {1} ", light.transform.localRotation.eulerAngles, volume.transform.localRotation.eulerAngles);


            volumetricLight.Awake();
            Console.WriteLine("#3 lightIntensity: {0} / {1} / {2} / {3}", light.intensity, templateVolumetricLight.lightSource.intensity, volumetricLight.lightIntensity, volumetricLight.intensity);
            Console.WriteLine("#3 lightRange: {0} / {1} / {2}", light.range, templateVolumetricLight.lightSource.range, volumetricLight.range);
            Console.WriteLine("#3 spotAngle: {0} / {1} / {2}", light.spotAngle, templateVolumetricLight.lightSource.spotAngle, light.innerSpotAngle);
            Console.WriteLine("#3 angle: {0} / {1}", volumetricLight.angle, templateVolumetricLight.angle);
            Console.WriteLine("#3 localScale: {0} / {1}", volumetricLight.volumGO.transform.localScale, templateVolumetricLight.volumGO.transform.localScale);
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
            Console.WriteLine("#5 {0}", cone);

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
                //TODO check docking behavior
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
