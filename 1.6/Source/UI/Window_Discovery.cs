using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;
namespace Discoveries
{
    [HotSwappable]
    public class Window_Discovery : Window
    {
        private Thing thing;
        private Pawn pawn;
        private bool isHumanlike;
        public Window_Discovery(Thing thing)
        {
            this.thing = thing;
            this.pawn = thing as Pawn;
            isHumanlike = pawn != null && pawn.RaceProps.Humanlike && pawn.genes != null;
            forcePause = true;
            absorbInputAroundWindow = true;
            closeOnClickedOutside = false;
            doCloseX = false;
        }
        public override Vector2 InitialSize => new Vector2(600f, 700f);
        public override void DoWindowContents(Rect inRect)
        {
            float titleHeight = 40f;
            float iconSize = 250f;
            float iconOffset = 50f;
            float labelOffset = 10f;
            float labelHeight = 30f;
            float labelWidth = 400f;
            float descOffset = 10f;
            float descHeight = 200f;
            float descWidth = 375f;
            float buttonHeight = 30f;
            float buttonWidth = descWidth;

            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.UpperCenter;
            Rect titleRect = new Rect(inRect.x, inRect.y, inRect.width, titleHeight);
            Widgets.Label(titleRect, "Disc_YouDiscovered".Translate());
            Text.Anchor = TextAnchor.UpperLeft;
            Text.Font = GameFont.Small;
            Rect iconRect = new Rect(inRect.width / 2f - iconSize / 2f, titleRect.yMax + iconOffset, iconSize, iconSize);
            if (isHumanlike)
            {
                GUI.color = XenotypeDef.IconColor;
                GUI.DrawTexture(iconRect, pawn.genes.XenotypeIcon);
                GUI.color = Color.white;
            }
            else
            {
                Widgets.DefIcon(iconRect, thing.def);
            }
            Widgets.InfoCardButton(iconRect.xMax, iconRect.y, thing);
            Text.Anchor = TextAnchor.UpperCenter;
            Text.Font = GameFont.Medium;
            string label = isHumanlike ? GetXenotypeLabel() : thing.def.LabelCap;
            Rect labelRect = new Rect(inRect.width / 2f - labelWidth / 2f, iconRect.yMax + labelOffset, labelWidth, labelHeight);
            Widgets.Label(labelRect, label);
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            Rect descRect = new Rect(inRect.width / 2f - descWidth / 2f, labelRect.yMax + descOffset, descWidth, descHeight);
            string description = isHumanlike ? GetXenotypeDescription() : thing.def.description;
            Widgets.LabelScrollable(descRect, description, ref scrollPosition);
            if (isHumanlike)
            {
                if (Widgets.ButtonText(new Rect(inRect.width / 2f - buttonWidth / 2f, inRect.height - buttonHeight * 2f - 10, buttonWidth, buttonHeight), "Disc_ViewGenes".Translate()))
                {
                    Find.WindowStack.Add(new Dialog_ViewGenes(pawn));
                }
            }
            if (Widgets.ButtonText(new Rect(inRect.width / 2f - buttonWidth / 2f, inRect.height - buttonHeight, buttonWidth, buttonHeight), "Disc_GotIt".Translate()))
            {
                Close();
            }
        }

        private static Vector2 scrollPosition = Vector2.zero;
        private string GetXenotypeLabel()
        {
            if (pawn.genes.Xenotype != null)
            {
                return pawn.genes.Xenotype.LabelCap;
            }
            else if (!pawn.genes.xenotypeName.NullOrEmpty())
            {
                return pawn.genes.xenotypeName;
            }
            return pawn.LabelCap;
        }
        private string GetXenotypeDescription()
        {
            if (pawn.genes.Xenotype != null)
            {
                return pawn.genes.Xenotype.description;
            }
            else if (!pawn.genes.xenotypeName.NullOrEmpty())
            {
                return "Disc_CustomXenotype".Translate(pawn.genes.xenotypeName);
            }
            return pawn.def.description;
        }
        public override void Close(bool doCloseSound = true)
        {
            base.Close(doCloseSound);
            Find.Selector.Select(thing);
            UnlockResearchForThing(thing);
        }

        private static void UnlockResearchForThing(Thing thing)
        {
            if (thing.def.HasModExtension<UnlockResearchOnDiscovery>())
            {
                var extension = thing.def.GetModExtension<UnlockResearchOnDiscovery>();
                if (extension.researchProject != null && !DiscoveryTracker.IsResearchDiscovered(extension.researchProject))
                {
                    DiscoveryTracker.MarkResearchDiscovered(extension.researchProject);
                    DefsOf.Disc_ResearchUnlock.PlayOneShotOnCamera();
                    Find.WindowStack.Add(new Window_Message("Disc_ResearchUnlocked".Translate(extension.researchProject.LabelCap)));
                }
            }
        }
    }
}
