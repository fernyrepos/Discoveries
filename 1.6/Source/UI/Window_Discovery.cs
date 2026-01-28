using RimWorld;
using UnityEngine;
using Verse;
namespace Discoveries
{
    [HotSwappable]
    public class Window_Discovery : Window
    {
        private Def def;
        private Thing thingContext;
        private Pawn pawn;
        private bool isHumanlike;
        private DefType defType;
        public Window_Discovery(Def def, Thing thingContext = null)
        {
            this.def = def;
            this.thingContext = thingContext;
            this.pawn = thingContext as Pawn;
            isHumanlike = pawn != null && pawn.RaceProps.Humanlike && pawn.genes != null;
            if (def is FactionDef) defType = DefType.Faction;
            else if (def is XenotypeDef) defType = DefType.Xenotype;
            else defType = DefType.Thing;
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
            if (defType == DefType.Xenotype)
            {
                GUI.color = XenotypeDef.IconColor;
                GUI.DrawTexture(iconRect, (def as XenotypeDef).Icon);
                GUI.color = Color.white;
            }
            else if (defType == DefType.Faction)
            {
                GUI.DrawTexture(iconRect, (def as FactionDef).FactionIcon);
            }
            else
            {
                Widgets.DefIcon(iconRect, def as ThingDef);
            }
            if (thingContext != null)
            {
                Widgets.InfoCardButton(iconRect.xMax, iconRect.y, thingContext);
            }
            Text.Anchor = TextAnchor.UpperCenter;
            Text.Font = GameFont.Medium;
            string label = GetLabel();
            Rect labelRect = new Rect(inRect.width / 2f - labelWidth / 2f, iconRect.yMax + labelOffset, labelWidth, labelHeight);
            Widgets.Label(labelRect, label);
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            Rect descRect = new Rect(inRect.width / 2f - descWidth / 2f, labelRect.yMax + descOffset, descWidth, descHeight);
            string description = GetDescription();
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
        private string GetLabel()
        {
            if (defType == DefType.Xenotype)
            {
                return (def as XenotypeDef).LabelCap;
            }
            else if (defType == DefType.Faction)
            {
                return (def as FactionDef).LabelCap;
            }
            else
            {
                return def.LabelCap;
            }
        }
        private string GetDescription()
        {
            if (defType == DefType.Xenotype)
            {
                return (def as XenotypeDef).description;
            }
            else if (defType == DefType.Faction)
            {
                return (def as FactionDef).Description;
            }
            else
            {
                return def.description;
            }
        }
        public override void Close(bool doCloseSound = true)
        {
            base.Close(doCloseSound);
            DiscoveryTracker.CheckAndQueueUnlocksFor(def);
            DiscoveryQueue.TryShowNext();
        }
    }
    internal enum DefType
    {
        Thing,
        Xenotype,
        Faction
    }
}
