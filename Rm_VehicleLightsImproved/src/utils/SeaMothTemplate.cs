namespace Rm_VehicleLightsImproved
{
    internal static class SeaMothTemplate
    {
        public static SeaMoth Get()
        {
            return CraftData.GetBuildPrefab(TechType.Seamoth).GetComponent<SeaMoth>();
        }
    }
}
