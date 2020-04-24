using UnityEngine;

namespace Rm_VehicleLightsImproved
{
    class CustomVolumetricLight
    {
        public static void CreateVolumetricLight(Light light,SeaMoth seaMothTemplate, Vector3 offset = new Vector3())
        {
            var templateVolumetricLight = seaMothTemplate.toggleLights.lightsParent.GetComponentInChildren<VFXVolumetricLight>();
            var volumetricLight = light.gameObject.GetComponent<VFXVolumetricLight>();
            if (volumetricLight == null)
            {
                volumetricLight = light.gameObject.AddComponent<VFXVolumetricLight>();
            }

            System.Reflection.FieldInfo[] volumetricLightFields = volumetricLight.GetType().GetFields();
            foreach (System.Reflection.FieldInfo field in volumetricLightFields)
            {
                field.SetValue(volumetricLight, field.GetValue(templateVolumetricLight));
            }

            var volume = GameObject.Instantiate(templateVolumetricLight.volumGO, light.transform).gameObject;
            volume.transform.localScale = CalculateLightCone(light);
            light.transform.localPosition += offset;

            volumetricLight.volumGO = volume;
            volumetricLight.lightSource = light;
            volumetricLight.block = null;
            volumetricLight.angle = (int)light.spotAngle;
            volumetricLight.intensity = 0.45f;

            volumetricLight.Awake();
        }
        public static Vector3 CalculateLightCone(Light light)
        {
            return CalculateLightCone(light.range, light.spotAngle);
        }
        public static Vector3 CalculateLightCone(float range, float angle)
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

    }
}
