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

	private int NumMaxVariations {
		get {
			return WorldEliteAbundancySystem.Abundancy switch {
				WorldEliteAbundancy.Scarce => 1,
				WorldEliteAbundancy.Regular => 3,
				WorldEliteAbundancy.Plentiful => 6,
				WorldEliteAbundancy.Ceaseless => 10,
				_ => 3,
			};
		}
	}

	private float SpawnChance {
		get {
			return WorldEliteAbundancySystem.Abundancy switch {
				WorldEliteAbundancy.Scarce => 0.05f,
				WorldEliteAbundancy.Regular => 0.1f,
				WorldEliteAbundancy.Plentiful => 0.25f,
				WorldEliteAbundancy.Ceaseless => 0.50f,
				_ => 0.1f,
			};
		}
	}

	// TODO: Secret seed fuckery!
	private Dictionary<EliteVariationRarity, float> _rarityToSpawnWeight = new() {
		{EliteVariationRarity.Common, 20f},
		{EliteVariationRarity.Uncommon, 15f},
		{EliteVariationRarity.Rare, 10f},
		{EliteVariationRarity.SuperRare, 5f},
		{EliteVariationRarity.Legendary, 2f},
	};

	private void TryApplyEliteVariations(Action<NPC, IEntitySource> orig, NPC npc, IEntitySource source) {
		bool isAffectableEnemy = !npc.friendly && npc.damage > 0 && !npc.immortal && !npc.dontTakeDamage && !ServerConfig.Instance.NPCBlacklist.Contains(new NPCDefinition(npc.type));
		bool applyToEnemyOrCritter = isAffectableEnemy || (ServerConfig.Instance.ApplyToCritters && npc.CountsAsACritter);
		bool careAboutBoss = ServerConfig.Instance.ApplyToBosses || !npc.CountsAsBoss();

		if (!applyToEnemyOrCritter || !careAboutBoss) {
			return;
		}

		int numVariations = Main.rand.NextRecursiveCount(SpawnChance, NumMaxVariations);
		if (WorldEliteAbundancySystem.Abundancy == WorldEliteAbundancy.Ceaseless) {
			numVariations++;
		}

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
			float weight = _rarityToSpawnWeight[variation.Rarity];
			variation.ModifySpawnWeight(ref weight);
			weightedRandom.Add(variation, weight);
		}

		weightedRandom.CalculateTotalWeight();

		int numVariationsToApply = int.Min(numVariations, applicableVariations.Count);
		for (int i = 0; i < numVariationsToApply; i++) {
			if (weightedRandom.elements.Count <= 0) {
				break;
			}

			EliteVariation variation = weightedRandom.GetAndRemove();

			// Easiest way to catch mutually exclusive variations without reconstructing the weighted random
			if (variation.CanApply(npc)) {
				variation.ApplyEliteVariation = true;
			}
			else {
				numVariationsToApply++;
			}

			weightedRandom.CalculateTotalWeight();
		}

		orig(npc, source);
	}
}
