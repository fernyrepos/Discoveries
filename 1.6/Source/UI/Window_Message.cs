using System;
using UnityEngine;
using Verse;
namespace Discoveries
{
    [HotSwappable]
    public class Window_Message : Window
    {
        public TaggedString text;
        public string title;
        public string buttonAText;
        public Action buttonAAction;
        public bool buttonADestructive;
        public float interactionDelay;
        public Texture2D image;
        private Vector2 scrollPosition = Vector2.zero;
        private float creationRealTime = -1f;
        private const float TitleHeight = 42f;
        protected const float ButtonHeight = 35f;
        public override Vector2 InitialSize => new Vector2(640f, 460f);
        private float TimeUntilInteractive => interactionDelay - (Time.realtimeSinceStartup - creationRealTime);
        private bool InteractionDelayExpired => TimeUntilInteractive <= 0f;
        public Window_Message(TaggedString text, string buttonAText = null, Action buttonAAction = null, string title = null, bool buttonADestructive = false, WindowLayer layer = WindowLayer.Dialog)
        {
            this.text = text;
            this.buttonAText = buttonAText;
            this.buttonAAction = buttonAAction;
            this.buttonADestructive = buttonADestructive;
            this.title = title;
            base.layer = layer;
            if (buttonAText.NullOrEmpty())
            {
                this.buttonAText = "OK".Translate();
            }
            forcePause = true;
            absorbInputAroundWindow = true;
            creationRealTime = RealTime.LastRealTime;
            onlyOneOfTypeAllowed = false;
            bool flag = buttonAAction == null;
            forceCatchAcceptAndCancelEventEvenIfUnfocused = flag;
            closeOnAccept = flag;
            closeOnCancel = flag;
        }
        public override void DoWindowContents(Rect inRect)
        {
            float num = inRect.y;
            if (!title.NullOrEmpty())
            {
                Text.Font = GameFont.Medium;
                Widgets.Label(new Rect(0f, num, inRect.width, 42f), title);
                num += 42f;
            }
            if (image != null)
            {
                float num2 = (float)image.width / (float)image.height;
                float num3 = 270f * num2;
                GUI.DrawTexture(new Rect(inRect.x + (inRect.width - num3) / 2f, num, num3, 270f), image);
                num += 280f;
            }
            Text.Font = GameFont.Small;
            Rect outRect = new Rect(inRect.x, num, inRect.width, inRect.height - 35f - 5f - num);
            float width = outRect.width - 16f;
            Rect viewRect = new Rect(0f, 0f, width, Text.CalcHeight(text, width));
            Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
            Widgets.Label(new Rect(0f, 0f, viewRect.width, viewRect.height), text);
            Widgets.EndScrollView();
            float num5 = inRect.width / 2f;
            float width2 = num5 - 10f;
            if (buttonADestructive)
            {
                GUI.color = new Color(1f, 0.3f, 0.35f);
            }
            string label = (InteractionDelayExpired ? buttonAText : (buttonAText + "(" + Mathf.Ceil(TimeUntilInteractive).ToString("F0") + ")"));
            if (Widgets.ButtonText(new Rect((inRect.width - width2) / 2f, inRect.height - 35f, width2, 35f), label) && InteractionDelayExpired)
            {
                if (buttonAAction != null)
                {
                    buttonAAction();
                }
                Close();
            }
            GUI.color = Color.white;
        }
    }
}
