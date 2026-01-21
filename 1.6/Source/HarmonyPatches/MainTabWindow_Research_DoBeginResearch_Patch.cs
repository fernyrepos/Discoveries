using HarmonyLib;
using RimWorld;
using Verse;
namespace Discoveries
{
    [HarmonyPatch(typeof(MainTabWindow_Research), "DoBeginResearch")]
    public static class MainTabWindow_Research_DoBeginResearch_Patch
    {
        public static bool Prefix(ResearchProjectDef projectToStart)
        {
            if (DiscoveryTracker.HasDiscoveryRequirement(projectToStart) && !DiscoveryTracker.IsResearchDiscovered(projectToStart))
            {
                Messages.Message("Disc_ResearchNotDiscovered".Translate(), MessageTypeDefOf.CautionInput);
                return false;
            }
            return true;
        }
    }
}
