using HarmonyLib;
using RimWorld;
using Verse;

namespace Discoveries
{
    [HarmonyPatch(typeof(Frame), nameof(Frame.CompleteConstruction))]
    public static class Frame_CompleteConstruction_Patch
    {
        public struct PatchState
        {
            public Map map;
            public IntVec3 position;
        }

        public static void Prefix(Frame __instance, out PatchState __state)
        {
            __state = new PatchState
            {
                map = __instance.Map,
                position = __instance.Position
            };
        }

        public static void Postfix(Frame __instance, PatchState __state)
        {
            if (__instance.def.entityDefToBuild is not ThingDef buildDef) return;
            var thing = __state.map.thingGrid.ThingAt(__state.position, buildDef);
            if (thing != null)
            {
                if (DiscoveryTracker.TryDiscover(thing))
                {
                    DiscoveryQueue.TryShowNext();
                }
            }
        }
    }
}