namespace Rm_FlareRepair
{
    public class FlareConfig
    {
        public float TotalDuration = 0.5f;
        public void ApplyConfig()
        {
            Flare_EnergyDraw_Patch.SetTotalEnergy(TotalDuration * 1200f);
        }
    }
}
