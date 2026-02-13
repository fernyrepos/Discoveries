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

        public override void WriteSettings()
        {
            base.WriteSettings();
            DiscoveryTracker.BuildDiscoveryCache();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);
            listing.CheckboxLabeled("Disc_EnableDiscoverySystem".Translate(), ref settings.discoveryEnabled);
            listing.CheckboxLabeled("Disc_EnableDiscoveryForThings".Translate(), ref settings.enableDiscoveryForThings);
            listing.CheckboxLabeled("Disc_EnableDiscoveryForPawns".Translate(), ref settings.enableDiscoveryForPawns);
            listing.CheckboxLabeled("Disc_ExcludeStartingScenario".Translate(), ref settings.excludeStartingScenario);
            listing.CheckboxLabeled("Disc_ExcludeStartingXenotypes".Translate(), ref settings.excludeStartingXenotypes);
            listing.CheckboxLabeled("Disc_ExcludeStartingAnimals".Translate(), ref settings.excludeStartingAnimals);
            listing.CheckboxLabeled("Disc_ObscureUnavailableResearch".Translate(), ref settings.obscureUnavailableResearch);
            listing.CheckboxLabeled("Disc_ObscureHigherTechLevel".Translate(), ref settings.obscureHigherTechLevel);
            listing.CheckboxLabeled("Disc_DisplayOnlyUnlocks".Translate(), ref settings.displayOnlyUnlocks);
            listing.CheckboxLabeled("Disc_DisableResearchUnlockSystem".Translate(), ref settings.disableResearchUnlockSystem);
            listing.CheckboxLabeled("Disc_SaveToClient".Translate(), ref settings.saveToClient);
            listing.Gap();

            if (listing.ButtonText("Disc_ResetSaveFile".Translate()))
            {
                if (!settings.saveToClient)
                {
                    DiscoveryTracker.Reset();
                }
            }
            if (listing.ButtonText("Disc_ResetClient".Translate()))
            {
                if (settings.saveToClient)
                {
                    DiscoveryTracker.Reset();
                }
            }
            listing.End();
            base.DoSettingsWindowContents(inRect);
        }
    }
}
