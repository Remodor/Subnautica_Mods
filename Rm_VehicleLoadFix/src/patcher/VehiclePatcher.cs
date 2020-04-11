using Harmony;

namespace Rm_VehicleLoadFix
{
    [HarmonyPatch(typeof(Vehicle))]
    [HarmonyPatch(nameof(Vehicle.Start))]
    internal class Vehicle_Start_Patch
    {
        [HarmonyPostfix]
        internal static void Postfix(Vehicle __instance)
        {
            UniqueIdentifier.TryGetIdentifier(__instance.pilotId, out UniqueIdentifier uniqueIdentifier);
            if (__instance.pilotId != null)
            {
                if (uniqueIdentifier)
                {
                    __instance.EnterVehicle(uniqueIdentifier.GetComponent<Player>(), true, false);
                }
            }
        }
    }
}
