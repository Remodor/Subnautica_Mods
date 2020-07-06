using System;
using UnityEngine;

namespace Rm_FastBuild
{
    public class BuildConfig
    {
        public float BuildingSpeed = 1f;
        public float MaximumBuildTime = 6f;
        public float MinimumBuildTime = 2f;
        public bool Progressive = true;

        public void ApplyModifier()
        {
            BuildSettings.buildingSpeed = Mathf.Max(BuildingSpeed, 0.1f);
            BuildSettings.maximumBuildTime = Mathf.Max(MaximumBuildTime, 0.1f, MinimumBuildTime + 0.1f);
            BuildSettings.minimumBuildTime = Mathf.Clamp(MinimumBuildTime, 0.1f, MaximumBuildTime);
            BuildSettings.progressive = Progressive;
        }
    }
}
