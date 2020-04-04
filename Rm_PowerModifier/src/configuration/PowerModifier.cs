namespace Rm_PowerModifier
{
    public class PowerModifier
    {
        public float SolarPanelPowerModifier = 0.3f;
        public float ThermalPlantPowerModifier = 1;
        public float BioReactorPowerModifier = 1;
        public float NuclearReactorPowerModifier = 1;

        public void ApplyModifier()
        {
            SolarPanel_Update_Patch.SetPowerModifier(SolarPanelPowerModifier);
            ThermalPlant_Update_Patch.SetPowerModifier(ThermalPlantPowerModifier);
            BioReactor_Update_Patch.SetPowerModifier(BioReactorPowerModifier);
            NuclearReactor_Update_Patch.SetPowerModifier(NuclearReactorPowerModifier);
        }
    }
}
