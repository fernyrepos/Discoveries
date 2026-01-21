using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Verse;
namespace Discoveries
{
    [HarmonyPatch(typeof(MainTabWindow_Research), "DrawStartButton")]
    public static class MainTabWindow_Research_DrawStartButton_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
        {
            var codes = codeInstructions.ToList();
            var selectedProjectInfo = AccessTools.Field(typeof(MainTabWindow_Research), "selectedProject");
            var lockedReason = AccessTools.Field(typeof(MainTabWindow_Research), "lockedReasons");
            bool patched = false;
            for (var i = 0; i < codes.Count; i++)
            {
                var code = codes[i];
                yield return code;
                if (!patched && code.LoadsField(lockedReason))
                {
                    patched = true;
                    yield return codes[++i];
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, selectedProjectInfo);
                    yield return new CodeInstruction(OpCodes.Ldsfld, lockedReason);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(MainTabWindow_Research_DrawStartButton_Patch), "AddDiscoveryLockedReason"));
                }
            }
        }
        public static void AddDiscoveryLockedReason(ResearchProjectDef researchProject, List<string> ___lockedReasons)
        {
            if (DiscoveryTracker.HasDiscoveryRequirement(researchProject) && !DiscoveryTracker.IsResearchDiscovered(researchProject))
            {
                ___lockedReasons.Add("Disc_ResearchNotDiscovered".Translate());
            }
        }
    }
}
