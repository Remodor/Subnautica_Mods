using System;
using UnityEngine;

namespace Rm_FastBuild
{
    public class BuildConfig
    {
        public float BuildingSpeed = 1f;

        public void ApplyModifier()
        {
            BuildSettings.buildingSpeed = BuildingSpeed;
        }
    }
}
