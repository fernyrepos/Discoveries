using Verse;
namespace Discoveries
{
    public class DiscoverySettings : ModSettings
    {
        public bool discoveryEnabled = true;
        public bool excludeResearched = true;
        public bool excludeStartingScenario = true;
        public bool excludeStartingXenotypes = true;
        public bool excludeStartingAnimals = true;
        public bool enableDiscoveryForThings = true;
        public bool enableDiscoveryForPawns = true;
        public bool saveToClient = false;
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref discoveryEnabled, "discoveryEnabled", true);
            Scribe_Values.Look(ref excludeResearched, "excludeResearched", true);
            Scribe_Values.Look(ref excludeStartingScenario, "excludeStartingScenario", true);
            Scribe_Values.Look(ref excludeStartingXenotypes, "excludeStartingXenotypes", true);
            Scribe_Values.Look(ref excludeStartingAnimals, "excludeStartingAnimals", true);
            Scribe_Values.Look(ref enableDiscoveryForThings, "enableDiscoveryForThings", true);
            Scribe_Values.Look(ref enableDiscoveryForPawns, "enableDiscoveryForPawns", true);
            Scribe_Values.Look(ref saveToClient, "saveToClient", false);
            if (saveToClient)
            {
                DiscoveryTracker.ExposeData();
            }
        }
    }
}
