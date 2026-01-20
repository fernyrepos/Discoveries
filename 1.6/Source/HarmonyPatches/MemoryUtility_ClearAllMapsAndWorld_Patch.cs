using HarmonyLib;
using Verse.Profile;
namespace Discoveries
{
    [HarmonyPatch(typeof(MemoryUtility), nameof(MemoryUtility.ClearAllMapsAndWorld))]
    public static class MemoryUtility_ClearAllMapsAndWorld_Patch
    {
        public static void Prefix()
        {
            if (!DiscoveriesMod.settings.saveToClient)
            {
                DiscoveryTracker.discoveredThingDefNames.Clear();
                DiscoveryTracker.discoveredXenotypeDefNames.Clear();
                DiscoveryTracker.discoveredCustomXenotypes.Clear();
            }
        }
    }
}
