namespace Rm_PowerModifier
{
    public class PowerModifier
    {
        public float SolarPanelPowerModifier = 0.3f;
        public float ThermalPlantPowerModifier = 1f;
        public float BioReactorPowerModifier = 1f;
        public float NuclearReactorPowerModifier = 1f;

        public float SolarPanelMaxPower = 75f;
        public float ThermalPlantMaxPower = 250f;
        public float BioReactorMaxPower = 500f;
        public float NuclearReactorMaxPower = 2500f;

        public void ApplyModifier()
        {
            SolarPanel_PowerModifier_Patch.SetPowerModifier(SolarPanelPowerModifier);
            SolarPanel_MaxPower_Patch.SetMaxPower(SolarPanelMaxPower);

            ThermalPlant_PowerModifier_Patch.SetPowerModifier(ThermalPlantPowerModifier);
            ThermalPlant_MaxPower_Patch.SetMaxPower(ThermalPlantMaxPower);

            BioReactor_PowerModifier_Patch.SetPowerModifier(BioReactorPowerModifier);
            BioReactor_MaxPower_Patch.SetMaxPower(BioReactorMaxPower);

            NuclearReactor_PowerModifier_Patch.SetPowerModifier(NuclearReactorPowerModifier);
            NuclearReactor_MaxPower_Patch.SetMaxPower(NuclearReactorMaxPower);
        }
    }
}
