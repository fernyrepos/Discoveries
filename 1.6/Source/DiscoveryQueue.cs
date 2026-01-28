using System;
using System.Collections.Generic;
using Verse;
using Verse.Sound;
namespace Discoveries
{
    public static class DiscoveryQueue
    {
        private static Queue<Action> windowQueue = new Queue<Action>();
        private static Thing thingToReselect;
        public static void StartDiscoverySequence(Thing thing)
        {
            thingToReselect = thing;
        }
        public static void EnqueueDiscovery(Def def, Thing context = null)
        {
            windowQueue.Enqueue(() =>
            {
                DefsOf.Disc_Discovery.PlayOneShotOnCamera();
                Find.WindowStack.Add(new Window_Discovery(def, context));
            });
        }
        public static void EnqueueMessage(string text)
        {
            windowQueue.Enqueue(() =>
            {
                DefsOf.Disc_ResearchUnlock.PlayOneShotOnCamera();
                Find.WindowStack.Add(new Window_Message(text));
            });
        }
        public static void TryShowNext()
        {
            if (windowQueue.Count > 0)
            {
                var action = windowQueue.Dequeue();
                action();
            }
            else if (thingToReselect != null)
            {
                Find.Selector.Select(thingToReselect);
                thingToReselect = null;
            }
        }
    }
}
