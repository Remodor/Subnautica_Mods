namespace Rm_FlareRepair
{
    public class FlareConfig
    {
        public float TotalDuration = 0.5f;
        public float IntensityModifier = 1.0f;
        public void ApplyConfig()
        {
            Flare_EnergyDraw_Patch.SetTotalEnergy(TotalDuration * 1200f);
            Flare_EnergyDraw_Patch.SetIntensityModifier(IntensityModifier);
        }
    }
}
