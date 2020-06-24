using System;
using UnityEngine;

namespace Rm_BetterFastCraft
{
    public class CraftConfig
    {
        public float CraftingSpeed = 1f;
        public float MinimumDuration = 2.7f;
        public ApplyMode Mode = ApplyMode.Relative;
        public enum ApplyMode
        {
            Relative,
            Total
        }

        public void ApplyModifier()
        {
            CraftingSettings.craftingSpeed = CraftingSpeed;
            CraftingSettings.minimumDuration = MinimumDuration;
            CraftingSettings.mode = Mode;
        }
    }
}
