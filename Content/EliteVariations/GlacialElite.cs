using EliteEnemies.Common;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace EliteEnemies.Content.EliteVariations;

public class GlacialElite : EliteVariation
{
	public override EliteVariationRarity Rarity => EliteVariationRarity.Rare;

	public override bool CanApply(NPC npc) {
		return !Main.hardMode && ServerConfig.Instance.EnableGlacial;
	}

	public override void OnApply(NPC npc) {
		if (ApplyEliteVariation) {
			npc.buffImmune[BuffID.Frostburn] = true;
		}
	}

	public override void AI(NPC npc) {
		if (!ApplyEliteVariation) {
			return;
		}

		foreach (Player player in Main.ActivePlayers) {
			if (npc.WithinRange(player.Center, 10 * 16)) {
				player.AddBuff(BuffID.Frostburn, 10);
			}
		}
	}

	public override void OnHitNPC(NPC npc, NPC target, NPC.HitInfo hit) {
		if (ApplyEliteVariation) {
			target.AddBuff(BuffID.Frostburn, 3 * 60);
		}
	}

	public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo) {
		if (ApplyEliteVariation) {
			target.AddBuff(BuffID.Frostburn, 3 * 60);
		}
	}

	public override void DrawEffects(NPC npc, ref Color drawColor) {
		if (ApplyEliteVariation) {
			drawColor = NPC.buffColor(drawColor, 1f - 0.6f, 1f - 0.6f, 1f, 1f);

			if (Main.rand.NextBool(3)) {
				Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.IceTorch);
				dust.noGravity = true;
			}
		}
	}
}
