using HarmonyLib;
using UnityEngine;
using Verse;
namespace Discoveries
{
    public class DiscoveriesMod : Mod
    {
        public static DiscoverySettings settings;
        public DiscoveriesMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<DiscoverySettings>();
            new Harmony("ferny.Discoveries").PatchAll();
        }
        public override string SettingsCategory() => Content.Name;

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);
            listing.CheckboxLabeled("Disc_EnableDiscoverySystem".Translate(), ref settings.discoveryEnabled);
            listing.CheckboxLabeled("Disc_ExcludeResearched".Translate(), ref settings.excludeResearched);
            listing.CheckboxLabeled("Disc_ExcludeStartingScenario".Translate(), ref settings.excludeStartingScenario);
            listing.CheckboxLabeled("Disc_ExcludeStartingXenotypes".Translate(), ref settings.excludeStartingXenotypes);
            listing.CheckboxLabeled("Disc_ExcludeStartingAnimals".Translate(), ref settings.excludeStartingAnimals);
            listing.CheckboxLabeled("Disc_SaveToClient".Translate(), ref settings.saveToClient);
            listing.Gap();

            if (listing.ButtonText("Disc_ResetSaveFile".Translate()))
            {
                if (!settings.saveToClient)
                {
                    DiscoveryTracker.discoveredThingDefNames.Clear();
                    DiscoveryTracker.discoveredXenotypeDefNames.Clear();
                    DiscoveryTracker.discoveredCustomXenotypes.Clear();
                }
            }
            if (listing.ButtonText("Disc_ResetClient".Translate()))
            {
                if (settings.saveToClient)
                {
                    DiscoveryTracker.discoveredThingDefNames.Clear();
                    DiscoveryTracker.discoveredXenotypeDefNames.Clear();
                    DiscoveryTracker.discoveredCustomXenotypes.Clear();
                }
            }
            listing.End();
            base.DoSettingsWindowContents(inRect);
        }
    }
}
