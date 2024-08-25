
using EliteEnemies.Common;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace EliteEnemies.Content.EliteVariations;

public class CursedElite : EliteVariation
{
	public override EliteVariationRarity Rarity => EliteVariationRarity.Common;

	public override bool CanApply(NPC npc) {
		return Main.hardMode && !WorldGen.crimson;
	}

	public override void OnApply(NPC npc) {
		if (ApplyEliteVariation) {
			npc.buffImmune[BuffID.OnFire] = true;
			npc.buffImmune[BuffID.OnFire3] = true;
			npc.buffImmune[BuffID.Oiled] = true;
			npc.buffImmune[BuffID.CursedInferno] = true;
			npc.buffImmune[BuffID.Ichor] = true;
		}
	}

	public override void AI(NPC npc) {
		if (!ApplyEliteVariation) {
			return;
		}

		foreach (Player player in Main.ActivePlayers) {
			if (npc.WithinRange(player.Center, 10 * 16)) {
				player.AddBuff(BuffID.OnFire3, 10);
			}
		}
	}

	public override void OnHitNPC(NPC npc, NPC target, NPC.HitInfo hit) {
		if (ApplyEliteVariation) {
			target.AddBuff(BuffID.CursedInferno, 3 * 60);
		}
	}

	public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo) {
		if (ApplyEliteVariation) {
			target.AddBuff(BuffID.CursedInferno, 3 * 60);
		}
	}

	public override void DrawEffects(NPC npc, ref Color drawColor) {
		if (ApplyEliteVariation) {
			drawColor = NPC.buffColor(drawColor, 1f - 0.6f, 1f, 1f - 0.7f, 1f);

			if (Main.rand.NextBool(3)) {
				Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.CursedTorch);
				dust.noGravity = true;
			}
		}
	}
}
