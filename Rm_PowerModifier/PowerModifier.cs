using Rm_Config;
using System;

namespace Rm_PowerModifier
{
    public class PowerModifier
    {
        public float SolarPowerModifier = 0.3f;

        public void ApplyModifier()
        {
            SolarPanel_Update_Patch.SetPowerModider(SolarPowerModifier);
        }
    }
}
