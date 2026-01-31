using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.Sound;
namespace Discoveries
{
    [HotSwappable]
    [StaticConstructorOnStartup]
    public static class DiscoveryTracker
    {
        public static HashSet<string> discoveredThingDefNames = new HashSet<string>();
        public static HashSet<string> discoveredXenotypeDefNames = new HashSet<string>();
        public static HashSet<string> discoveredCustomXenotypes = new HashSet<string>();
        public static HashSet<string> discoveredResearchProjectDefNames = new HashSet<string>();
        public static HashSet<string> discoveredFactionDefNames = new HashSet<string>();
        private static HashSet<ResearchProjectDef> lockedResearchCache = new HashSet<ResearchProjectDef>();
        static DiscoveryTracker()
        {
            EnsureNotNull();
            BuildDiscoveryCache();
        }
        public static void BuildDiscoveryCache()
        {
            lockedResearchCache.Clear();
            foreach (var thingDef in DefDatabase<ThingDef>.AllDefs)
            {
                foreach (var extension in thingDef.modExtensions?.OfType<UnlockResearchOnDiscovery>() ?? Enumerable.Empty<UnlockResearchOnDiscovery>())
                {
                    foreach (var project in extension.GetProjects())
                    {
                        if (project != null) lockedResearchCache.Add(project);
                    }
                }
            }
            foreach (var factionDef in DefDatabase<FactionDef>.AllDefs)
            {
                if (factionDef.HasModExtension<ExcludeFromDiscoveries>()) continue;
                foreach (var extension in factionDef.modExtensions?.OfType<UnlockResearchOnDiscovery>() ?? Enumerable.Empty<UnlockResearchOnDiscovery>())
                {
                    foreach (var project in extension.GetProjects())
                    {
                        if (project != null) lockedResearchCache.Add(project);
                    }
                }
            }
            foreach (var xenoDef in DefDatabase<XenotypeDef>.AllDefs)
            {
                if (xenoDef.HasModExtension<ExcludeFromDiscoveries>()) continue;
                foreach (var extension in xenoDef.modExtensions?.OfType<UnlockResearchOnDiscovery>() ?? Enumerable.Empty<UnlockResearchOnDiscovery>())
                {
                    foreach (var project in extension.GetProjects())
                    {
                        if (project != null) lockedResearchCache.Add(project);
                    }
                }
            }
        }
        public static bool IsDiscovered(Thing thing)
        {
            if (discoveredThingDefNames is null)
            {
                EnsureNotNull();
            }
            if (thing is Pawn pawn && pawn.RaceProps.Humanlike)
            {
                return IsXenotypeDiscovered(pawn);
            }
            else
            {
                return discoveredThingDefNames.Contains(thing.def.defName);
            }
        }
        public static bool IsDiscovered(FactionDef factionDef)
        {
            return discoveredFactionDefNames.Contains(factionDef.defName);
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
        public static void MarkDiscovered(FactionDef factionDef)
        {
            discoveredFactionDefNames.Add(factionDef.defName);
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
                UnlockResearchForThing(thing, showMessage: false);
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
            return lockedResearchCache.Contains(research);
        }
        public static void UnlockResearchForThing(Thing thing, bool showMessage = true)
        {
            foreach (var extension in thing.def.modExtensions?.OfType<UnlockResearchOnDiscovery>() ?? Enumerable.Empty<UnlockResearchOnDiscovery>())
            {
                foreach (var project in extension.GetProjects())
                {
                    if (project == null || IsResearchDiscovered(project))
                    {
                        continue;
                    }
                    MarkResearchDiscovered(project);
                    if (showMessage)
                    {
                        DefsOf.Disc_ResearchUnlock.PlayOneShotOnCamera();
                        string message = ShouldObscureResearch(project) ? "Disc_ResearchUnlockedFuture".Translate() : "Disc_ResearchUnlocked".Translate(project.LabelCap);
                        Find.WindowStack.Add(new Window_Message(message));
                    }
                }
            }
        }
        public static void CheckAndQueueUnlocksFor(Def discoveredDef)
        {
            foreach (var extension in discoveredDef.modExtensions?.OfType<UnlockResearchOnDiscovery>() ?? Enumerable.Empty<UnlockResearchOnDiscovery>())
            {
                foreach (var project in extension.GetProjects())
                {
                    if (!IsResearchDiscovered(project))
                    {
                        MarkResearchDiscovered(project);
                        string msg = ShouldObscureResearch(project) ? "Disc_ResearchUnlockedFuture".Translate() : "Disc_ResearchUnlocked".Translate(project.LabelCap);
                        DiscoveryQueue.EnqueueMessage(msg);
                    }
                }
            }
        }
        public static bool ShouldExcludeThing(Thing thing)
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
            return false;
        }
        public static bool ShouldObscureResearch(ResearchProjectDef research)
        {
            if (DiscoveriesMod.settings.obscureUnavailableResearch && !research.PrerequisitesCompleted)
            {
                return true;
            }
            if (DiscoveriesMod.settings.obscureHigherTechLevel && research.techLevel > Faction.OfPlayer.def.techLevel)
            {
                return true;
            }
            return false;
        }
        public static Thing GetDiscoveryThing(Thing thing)
        {
            if (thing is Corpse corpse && corpse.InnerPawn != null)
            {
                return corpse.InnerPawn;
            }
            return thing;
        }
        public static void ExposeData()
        {
            Scribe_Collections.Look(ref discoveredThingDefNames, "discoveredThingDefNames", LookMode.Value);
            Scribe_Collections.Look(ref discoveredXenotypeDefNames, "discoveredXenotypeDefNames", LookMode.Value);
            Scribe_Collections.Look(ref discoveredCustomXenotypes, "discoveredCustomXenotypes", LookMode.Value);
            Scribe_Collections.Look(ref discoveredResearchProjectDefNames, "discoveredResearchProjectDefNames", LookMode.Value);
            Scribe_Collections.Look(ref discoveredFactionDefNames, "discoveredFactionDefNames", LookMode.Value);
            EnsureNotNull();
        }

        private static void EnsureNotNull()
        {
            discoveredThingDefNames ??= new HashSet<string>();
            discoveredXenotypeDefNames ??= new HashSet<string>();
            discoveredCustomXenotypes ??= new HashSet<string>();
            discoveredResearchProjectDefNames ??= new HashSet<string>();
            discoveredFactionDefNames ??= new HashSet<string>();
        }

        public static bool IsResearchLockedByDiscovery(ResearchProjectDef research)
        {
            if (DiscoveriesMod.settings.disableResearchUnlockSystem)
            {
                return false;
            }
            return HasDiscoveryRequirement(research) && !IsResearchDiscovered(research);
        }

        public static void Reset()
        {
            EnsureNotNull();
            discoveredThingDefNames.Clear();
            discoveredXenotypeDefNames.Clear();
            discoveredCustomXenotypes.Clear();
            discoveredResearchProjectDefNames.Clear();
            discoveredFactionDefNames.Clear();
        }
    }
}
