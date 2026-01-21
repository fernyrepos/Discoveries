using System.Collections.Generic;
using Verse;
namespace Discoveries
{
    [HotSwappable]
    public static class DiscoveryTracker
    {
        public static HashSet<string> discoveredThingDefNames = new HashSet<string>();
        public static HashSet<string> discoveredXenotypeDefNames = new HashSet<string>();
        public static HashSet<string> discoveredCustomXenotypes = new HashSet<string>();
        public static HashSet<string> discoveredResearchProjectDefNames = new HashSet<string>();
        public static bool IsDiscovered(Thing thing)
        {
            if (thing is Pawn pawn && pawn.RaceProps.Humanlike)
            {
                return IsXenotypeDiscovered(pawn);
            }
            else
            {
                return discoveredThingDefNames.Contains(thing.def.defName);
            }
        }
        private static bool IsXenotypeDiscovered(Pawn pawn)
        {
            if (pawn.genes == null) return true;
            if (pawn.genes.Xenotype != null)
            {
                return discoveredXenotypeDefNames.Contains(pawn.genes.Xenotype.defName);
            }
            else if (!pawn.genes.xenotypeName.NullOrEmpty())
            {
                return discoveredCustomXenotypes.Contains(pawn.genes.xenotypeName);
            }
            return true;
        }
        public static void MarkDiscovered(Thing thing)
        {
            if (thing is Pawn pawn && pawn.RaceProps.Humanlike)
            {
                MarkXenotypeDiscovered(pawn);
            }
            else
            {
                discoveredThingDefNames.Add(thing.def.defName);
            }
        }
        private static void MarkXenotypeDiscovered(Pawn pawn)
        {
            if (pawn.genes == null) return;
            if (pawn.genes.Xenotype != null)
            {
                discoveredXenotypeDefNames.Add(pawn.genes.Xenotype.defName);
            }
            else if (!pawn.genes.xenotypeName.NullOrEmpty())
            {
                discoveredCustomXenotypes.Add(pawn.genes.xenotypeName);
            }
        }
        public static void MarkStartingThingsDiscovered(List<Thing> startingThings)
        {
            if (!DiscoveriesMod.settings.excludeStartingScenario) return;
            foreach (var pawn in Find.GameInitData.startingAndOptionalPawns)
            {
                if (startingThings.Contains(pawn) is false)
                {
                    startingThings.Add(pawn);
                }
            }
            foreach (Thing thing in startingThings)
            {
                if (thing is Pawn pawn)
                {
                    if (pawn.RaceProps.Animal && !DiscoveriesMod.settings.excludeStartingAnimals)
                        continue;
                    if (pawn.RaceProps.Humanlike && !DiscoveriesMod.settings.excludeStartingXenotypes)
                        continue;
                }
                MarkDiscovered(thing);
            }
        }
        public static bool IsResearchDiscovered(ResearchProjectDef research)
        {
            return research.IsFinished || discoveredResearchProjectDefNames.Contains(research.defName);
        }
        public static void MarkResearchDiscovered(ResearchProjectDef research)
        {
            discoveredResearchProjectDefNames.Add(research.defName);
        }
        public static bool HasDiscoveryRequirement(ResearchProjectDef research)
        {
            foreach (var thingDef in DefDatabase<ThingDef>.AllDefs)
            {
                if (thingDef.HasModExtension<UnlockResearchOnDiscovery>())
                {
                    var extension = thingDef.GetModExtension<UnlockResearchOnDiscovery>();
                    if (extension.researchProject == research)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public static void ExposeData()
        {
            Scribe_Collections.Look(ref discoveredThingDefNames, "discoveredThingDefNames", LookMode.Value);
            Scribe_Collections.Look(ref discoveredXenotypeDefNames, "discoveredXenotypeDefNames", LookMode.Value);
            Scribe_Collections.Look(ref discoveredCustomXenotypes, "discoveredCustomXenotypes", LookMode.Value);
            Scribe_Collections.Look(ref discoveredResearchProjectDefNames, "discoveredResearchProjectDefNames", LookMode.Value);
        }
    }
}
