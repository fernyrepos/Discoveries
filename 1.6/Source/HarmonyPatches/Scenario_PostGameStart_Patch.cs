using HarmonyLib;
using RimWorld;
using Verse;
namespace Discoveries
{
    [HarmonyPatch(typeof(Scenario), nameof(Scenario.PostGameStart))]
    public static class Scenario_PostGameStart_Patch
    {
        public static void Postfix()
        {
            if (!DiscoveriesMod.settings.excludeStartingScenario) return;
            foreach (ScenPart scenPart in Find.Scenario.AllParts)
            {
                if (scenPart is ScenPart_StartingThing_Defined scenPart_StartingThing_Defined)
                {
                    foreach (Thing thing in scenPart_StartingThing_Defined.PlayerStartingThings())
                    {
                        if (thing is Pawn pawn)
                        {
                            if (pawn.RaceProps.Animal && !DiscoveriesMod.settings.excludeStartingAnimals)
                                continue;
                            if (pawn.RaceProps.Humanlike && !DiscoveriesMod.settings.excludeStartingXenotypes)
                                continue;
                        }
                        DiscoveryTracker.MarkDiscovered(thing);
                    }
                }
            }
        }
    }
}
