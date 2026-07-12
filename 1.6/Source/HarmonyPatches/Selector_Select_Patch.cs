using HarmonyLib;
using RimWorld;
using Verse;
using Verse.Sound;
namespace Discoveries
{
    [HarmonyPatch(typeof(Selector), nameof(Selector.Select))]
    public static class Selector_Select_Patch
    {
        public static void Postfix(Selector __instance, object obj)
        {
            if (obj is Thing thing && __instance.SingleSelectedThing == thing)
            {
                if (DiscoveryTracker.TryDiscover(thing, true))
                {
                    __instance.ClearSelection();
                    DiscoveryQueue.TryShowNext();
                }
            }
        }
    }
}
