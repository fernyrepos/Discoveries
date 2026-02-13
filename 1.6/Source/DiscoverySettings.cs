using Verse;
namespace Discoveries
{
    public class DiscoverySettings : ModSettings
    {
        public bool discoveryEnabled = true;
        public bool excludeStartingScenario = true;
        public bool excludeStartingXenotypes = true;
        public bool excludeStartingAnimals = true;
        public bool enableDiscoveryForThings = true;
        public bool enableDiscoveryForPawns = true;
        public bool obscureUnavailableResearch = true;
        public bool obscureHigherTechLevel = false;
        public bool displayOnlyUnlocks = false;
        public bool saveToClient = false;
        public bool disableResearchUnlockSystem = false;
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref discoveryEnabled, "discoveryEnabled", true);
            Scribe_Values.Look(ref excludeStartingScenario, "excludeStartingScenario", true);
            Scribe_Values.Look(ref excludeStartingXenotypes, "excludeStartingXenotypes", true);
            Scribe_Values.Look(ref excludeStartingAnimals, "excludeStartingAnimals", true);
            Scribe_Values.Look(ref enableDiscoveryForThings, "enableDiscoveryForThings", true);
            Scribe_Values.Look(ref enableDiscoveryForPawns, "enableDiscoveryForPawns", true);
            Scribe_Values.Look(ref obscureUnavailableResearch, "obscureUnavailableResearch", true);
            Scribe_Values.Look(ref obscureHigherTechLevel, "obscureHigherTechLevel", false);
            Scribe_Values.Look(ref displayOnlyUnlocks, "displayOnlyUnlocks", false);
            Scribe_Values.Look(ref saveToClient, "saveToClient", false);
            Scribe_Values.Look(ref disableResearchUnlockSystem, "disableResearchUnlockSystem", false);
            if (saveToClient)
            {
                DiscoveryTracker.ExposeData();
            }
        }
    }
}
