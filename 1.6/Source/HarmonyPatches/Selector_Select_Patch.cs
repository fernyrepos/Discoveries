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
                bool anyDiscovery = false;
                Thing discoveryThing = DiscoveryTracker.GetDiscoveryThing(thing);

                if (!DiscoveryTracker.IsDiscovered(discoveryThing) && !DiscoveryTracker.ShouldExcludeThing(discoveryThing))
                {
                    Def targetDef = discoveryThing.def;
                    if (discoveryThing is Pawn p && p.genes?.Xenotype != null) targetDef = p.genes.Xenotype;
                    if (!DiscoveriesMod.settings.displayOnlyUnlocks || targetDef.HasModExtension<UnlockResearchOnDiscovery>())
                    {
                        DiscoveryTracker.MarkDiscovered(discoveryThing);
                        DiscoveryQueue.EnqueueDiscovery(targetDef, discoveryThing);
                        anyDiscovery = true;
                    }
                }

                if (discoveryThing.Faction != null && discoveryThing.Faction.def != null)
                {
                    if (!DiscoveryTracker.IsDiscovered(discoveryThing.Faction.def))
                    {
                        if (!discoveryThing.Faction.IsPlayer && !discoveryThing.Faction.def.HasModExtension<ExcludeFromDiscoveries>())
                        {
                            if (!DiscoveriesMod.settings.displayOnlyUnlocks || discoveryThing.Faction.def.HasModExtension<UnlockResearchOnDiscovery>())
                            {
                                DiscoveryTracker.MarkDiscovered(discoveryThing.Faction.def);
                                DiscoveryQueue.EnqueueDiscovery(discoveryThing.Faction.def, discoveryThing);
                                anyDiscovery = true;
                            }
                        }
                    }
                }

                if (anyDiscovery)
                {
                    DiscoveryQueue.StartDiscoverySequence(discoveryThing);
                    __instance.ClearSelection();
                    DiscoveryQueue.TryShowNext();
                }
            }
        }
    }
}
