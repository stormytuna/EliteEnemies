using EliteEnemies.Common;
using Terraria;

namespace EliteEnemies.Content.EliteVariations;

public class SpelunkerElite : EliteVariation
{
	public override EliteVariationRarity Rarity => EliteVariationRarity.Rare;

	public override bool CanApply(NPC npc) {
		return ServerConfig.Instance.EnableSpelunker;
	}

	public override void AI(NPC npc) {
		if (!ApplyEliteVariation) {
			return;
		}

		Main.instance.SpelunkerProjectileHelper.AddSpotToCheck(npc.Center);
		Lighting.AddLight(npc.Center, 1.05f, 0.95f, 0.55f);
	}
}
