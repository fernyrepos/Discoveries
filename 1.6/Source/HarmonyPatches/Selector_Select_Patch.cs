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
            if (!DiscoveriesMod.settings.discoveryEnabled) return;
            if (obj is Thing thing && __instance.SingleSelectedThing == thing)
            {
                if (DiscoveryTracker.IsDiscovered(thing)) return;
                if (ShouldExclude(thing)) return;
                __instance.ClearSelection();
                DiscoveryTracker.MarkDiscovered(thing);
                DefsOf.Disc_Discovery.PlayOneShotOnCamera();
                Find.WindowStack.Add(new Window_Discovery(thing));
            }
        }
        private static bool ShouldExclude(Thing thing)
        {
            if (thing is Blueprint || thing is Frame)
            {
                return true;
            }
            if (thing.def.HasModExtension<ExcludeFromDiscoveries>())
            {
                return true;
            }
            if (thing is Pawn)
            {
                if (!DiscoveriesMod.settings.enableDiscoveryForPawns)
                {
                    return true;
                }
            }
            else
            {
                if (!DiscoveriesMod.settings.enableDiscoveryForThings)
                {
                    return true;
                }
            }
            if (DiscoveriesMod.settings.excludeResearched)
            {
                if (IsResearched(thing.def))
                {
                    return true;
                }
            }
            return false;
        }
        private static bool IsResearched(ThingDef def)
        {
            if (def.researchPrerequisites != null && def.researchPrerequisites.Count > 0)
            {
                foreach (ResearchProjectDef research in def.researchPrerequisites)
                {
                    if (!research.IsFinished)
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
    }
}
