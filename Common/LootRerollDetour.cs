using System.Collections.Generic;

namespace EliteEnemies.Common;

public class LootRerollDetour : ILoadable
{
	// TODO: Secret seed fuckery!
	private static Dictionary<EliteVariationRarity, float> _rarityToLootMult = new() {
		{EliteVariationRarity.Common, 1.2f},
		{EliteVariationRarity.Uncommon, 1.4f},
		{EliteVariationRarity.Rare, 1.8f},
		{EliteVariationRarity.SuperRare, 2.5f},
		{EliteVariationRarity.Legendary, 4f},
	};

	public void Load(Mod mod) {
		On_NPC.NPCLoot_DropItems += static (orig, self, closestPlayer) => {
			orig(self, closestPlayer);

			foreach (GlobalNPC global in self.Globals) {
				if (global is not EliteVariation { ApplyEliteVariation: true } eliteVariation) {
					continue;
				}

				float lootMultiplier = _rarityToLootMult[eliteVariation.Rarity];
				while (lootMultiplier > 0f) {
					lootMultiplier--;
					float rollChance = float.Clamp(lootMultiplier, 0f, 1f);
					if (Main.rand.NextFloat() < rollChance) {
						orig(self, closestPlayer);
					}
				}
			}
		};
	}

	public void Unload() { }
}
