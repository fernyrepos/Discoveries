using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;
namespace Discoveries
{
	[HarmonyPatch(typeof(ScenPart_PlayerPawnsArriveMethod), "DoDropPods")]
	public static class ScenPart_PlayerPawnsArriveMethod_DoDropPods_Patch
	{
		public static void Postfix(Map map, List<Thing> startingItems)
		{
			DiscoveryTracker.MarkStartingThingsDiscovered(startingItems);
		}
	}
}
