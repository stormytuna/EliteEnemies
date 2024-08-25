using EliteEnemies.Common;
using Terraria;

namespace EliteEnemies.Content.EliteVariations;

public class LuckyElite : EliteVariation
{
	public override EliteVariationRarity Rarity => EliteVariationRarity.Common;

	public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers) {
		if (ApplyEliteVariation) {
			modifiers.DamageVariationScale *= 0f;
			modifiers.FinalDamage -= 0.075f;
		}
	}
}
