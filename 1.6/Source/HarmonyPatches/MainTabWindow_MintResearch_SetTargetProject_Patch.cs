using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using Verse;
namespace Discoveries
{
    [HarmonyPatch]
    public static class MainTabWindow_MintResearch_SetTargetProject_Patch
    {
        public static bool Prepare() => ModsConfig.IsActive("Dubwise.DubsMintMenus");

        [HarmonyTargetMethods]
        public static IEnumerable<MethodBase> TargetMethods()
        {
            var type = AccessTools.TypeByName("DubsMintMenus.MainTabWindow_MintResearch+MysterBox");
            yield return AccessTools.Method(type, "SetTargetProject");
        }

        public static bool Prefix(ResearchProjectDef proj)
        {
            if (DiscoveryTracker.HasDiscoveryRequirement(proj) && !DiscoveryTracker.IsResearchDiscovered(proj))
            {
                Messages.Message("Disc_ResearchNotDiscovered".Translate(), MessageTypeDefOf.CautionInput);
                return false;
            }
            return true;
        }
    }
}
