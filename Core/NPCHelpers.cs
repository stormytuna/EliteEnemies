using EliteEnemies.Common;
using Terraria;
using Terraria.ID;

namespace EliteEnemies.Core;

public static class NPCHelpers
{
	public static bool HasEliteVariation<TVariation>(this NPC npc) where TVariation : EliteVariation {
		foreach (var global in npc.Globals) {
			if (global is TVariation t && t.ApplyEliteVariation) {
				return true;
			}
		}

		return false;
	}

	public static bool HasNotEliteVariation<TVariation>(this NPC npc) where TVariation : EliteVariation {
		return !HasEliteVariation<TVariation>(npc);
	}

	public static int NumActiveEliteVariations(this NPC npc) {
		int count = 0;

		foreach (var global in npc.Globals) {
			if (global is EliteVariation variation && variation.ApplyEliteVariation) {
				count++;
			}
		}

		return count;
	}

	public static bool IsBoss(this NPC npc) {
		return npc.boss && (npc.type is not NPCID.EaterofWorldsHead or NPCID.EaterofWorldsBody or NPCID.EaterofWorldsTail);
	}

	public static bool IsWorm(this NPC npc) {
		return npc.type switch {
			NPCID.EaterofWorldsHead => true,
			NPCID.EaterofWorldsBody => true,
			NPCID.EaterofWorldsTail => true,
			NPCID.LeechHead => true,
			NPCID.LeechBody => true,
			NPCID.LeechTail => true,
			NPCID.DiggerHead => true,
			NPCID.DiggerBody => true,
			NPCID.DiggerTail => true,
			NPCID.DevourerHead => true,
			NPCID.DevourerBody => true,
			NPCID.DevourerTail => true,
			NPCID.DuneSplicerHead => true,
			NPCID.DuneSplicerBody => true,
			NPCID.DuneSplicerTail => true,
			NPCID.GiantWormHead => true,
			NPCID.GiantWormBody => true,
			NPCID.GiantWormTail => true,
			NPCID.TombCrawlerHead => true,
			NPCID.TombCrawlerBody => true,
			NPCID.TombCrawlerTail => true,
			NPCID.SeekerHead => true,
			NPCID.SeekerBody => true,
			NPCID.SeekerTail => true,
			NPCID.SolarCrawltipedeHead => true,
			NPCID.SolarCrawltipedeBody => true,
			NPCID.SolarCrawltipedeTail => true,
			NPCID.WyvernHead => true,
			NPCID.WyvernBody => true,
			NPCID.WyvernBody2 => true,
			NPCID.WyvernBody3 => true,
			NPCID.WyvernTail => true,
			NPCID.BoneSerpentHead => true,
			NPCID.BoneSerpentBody => true,
			NPCID.BoneSerpentTail => true,
			NPCID.BloodEelHead => true,
			NPCID.BloodEelBody => true,
			NPCID.BloodEelTail => true,
			NPCID.CultistDragonHead => true,
			NPCID.CultistDragonBody1 => true,
			NPCID.CultistDragonBody2 => true,
			NPCID.CultistDragonBody3 => true,
			NPCID.CultistDragonBody4 => true,
			NPCID.CultistDragonTail => true,
			_ => false,
		};
	}
}
