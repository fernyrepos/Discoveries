using System.Collections.Generic;
using RimWorld;
using Verse;
namespace Discoveries
{
    public static class DiscoveryTracker
    {
        public static HashSet<string> discoveredThingDefNames = new HashSet<string>();
        public static HashSet<string> discoveredXenotypeDefNames = new HashSet<string>();
        public static HashSet<string> discoveredCustomXenotypes = new HashSet<string>();
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
        public static void ExposeData()
        {
            Scribe_Collections.Look(ref discoveredThingDefNames, "discoveredThingDefNames", LookMode.Value);
            Scribe_Collections.Look(ref discoveredXenotypeDefNames, "discoveredXenotypeDefNames", LookMode.Value);
            Scribe_Collections.Look(ref discoveredCustomXenotypes, "discoveredCustomXenotypes", LookMode.Value);
        }
    }
}
