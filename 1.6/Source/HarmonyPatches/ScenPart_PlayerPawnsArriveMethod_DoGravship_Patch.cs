using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;
namespace Discoveries
{
	[HarmonyPatch(typeof(ScenPart_PlayerPawnsArriveMethod), "DoGravship")]
	public static class ScenPart_PlayerPawnsArriveMethod_DoGravship_Patch
	{
		public static void Postfix(Map map, List<Thing> startingItems)
		{
			DiscoveryTracker.MarkStartingThingsDiscovered(startingItems);
		}
	}
}
