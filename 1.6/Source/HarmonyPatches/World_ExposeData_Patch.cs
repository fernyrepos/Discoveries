using HarmonyLib;
using RimWorld.Planet;
namespace Discoveries
{
    [HarmonyPatch(typeof(World), nameof(World.ExposeData))]
    public static class World_ExposeData_Patch
    {
        public static void Postfix()
        {
            if (!DiscoveriesMod.settings.saveToClient)
            {
                DiscoveryTracker.ExposeData();
            }
        }
    }
}
