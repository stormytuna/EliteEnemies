using EliteEnemies.Common;

namespace EliteEnemies.Content.EliteVariations;

public class MedicatedElite : EliteVariation
{
	public override EliteVariationRarity Rarity {
		get => EliteVariationRarity.Uncommon;
	}

	public override void OnApply(NPC npc) {
		if (!ApplyEliteVariation) {
			return;
		}

		for (int i = 0; i < BuffLoader.BuffCount; i++) {
			if (!BuffID.Sets.IsATagBuff[i]) {
				npc.buffImmune[i] = true;
			}
		}
	}
}
