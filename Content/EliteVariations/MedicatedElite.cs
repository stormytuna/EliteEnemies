using EliteEnemies.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EliteEnemies.Content.EliteVariations;

public class MedicatedElite : EliteVariation
{
	public override EliteVariationRarity Rarity => EliteVariationRarity.Uncommon;

	public override bool CanApply(NPC npc) {
		return ServerConfig.Instance.EnableMedicated;
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
