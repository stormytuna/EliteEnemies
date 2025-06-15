using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria.DataStructures;
using Terraria.ModLoader.Config;
using Terraria.Utilities;

namespace EliteEnemies.Common;

public class ApplyEliteVariations : ILoadable
{
	public void Load(Mod mod) {
		MonoModHooks.Add(typeof(NPCLoader).GetMethod("OnSpawn", BindingFlags.Static | BindingFlags.NonPublic), TryApplyEliteVariations);
	}

	public void Unload() { }

	private void TryApplyEliteVariations(Action<NPC, IEntitySource> orig, NPC npc, IEntitySource source) {
		bool isAffectableEnemy = !npc.friendly && npc.damage > 0 && !npc.immortal && !npc.dontTakeDamage && !ServerConfig.Instance.NPCBlacklist.Contains(new NPCDefinition(npc.type));
		bool applyToEnemyOrCritter = isAffectableEnemy || (ServerConfig.Instance.ApplyToCritters && npc.CountsAsACritter);
		bool careAboutBoss = ServerConfig.Instance.ApplyToBosses || !npc.CountsAsBoss();
		bool careAboutModded = ServerConfig.Instance.ApplyToModdedNPCs || npc.ModNPC is null;

		if (!applyToEnemyOrCritter || !careAboutBoss || !careAboutModded) {
			return;
		}

		int numVariations = Main.rand.NextRecursiveCount(ServerConfig.Instance.SpawnChance, ServerConfig.Instance.MaxSimultaneousVariations);
		if (numVariations <= 0) {
			return;
		}

		List<EliteVariation> eliteVariations = new();
		foreach (GlobalNPC global in npc.Globals) {
			if (global is EliteVariation eliteVariation) {
				eliteVariations.Add(eliteVariation);
			}
		}

		List<EliteVariation> applicableVariations = eliteVariations.Where(v => v.CanApply(npc)).ToList();
		WeightedRandom<EliteVariation> weightedRandom = new();
		foreach (EliteVariation variation in applicableVariations) {
			weightedRandom.Add(variation, variation.SpawnWeight);
		}

		int numVariationsToApply = int.Min(numVariations, applicableVariations.Count);
		for (int i = 0; i < numVariationsToApply; i++) {
			EliteVariation variation = weightedRandom.GetAndRemove();
			variation.ApplyEliteVariation = true;
		}

		orig(npc, source);
	}
}
