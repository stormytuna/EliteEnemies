using EliteEnemies.Common;
using Microsoft.Xna.Framework;
using Terraria;

namespace EliteEnemies.Content.EliteVariations;

public class SneakyElite : EliteVariation
{
	public override EliteVariationRarity Rarity => EliteVariationRarity.Common;

	public override void DrawEffects(NPC npc, ref Color drawColor) {
		if (!ApplyEliteVariation) {
			return;
		}

		float closestDist = float.PositiveInfinity;
		float invisStrength = 1f;

		foreach (Player player in Main.ActivePlayers) {
			float dist = npc.Distance(player.Center);
			if (dist < 20f * 16f && dist < closestDist) {
				invisStrength = dist / (20f * 16f);
				closestDist = dist;
			}
		}

		drawColor *= 1 - invisStrength;
	}
}
