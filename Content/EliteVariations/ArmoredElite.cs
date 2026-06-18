using EliteEnemies.Common;

namespace EliteEnemies.Content.EliteVariations;

public class ArmoredElite : EliteVariation
{
	public override EliteVariationRarity Rarity {
		get => EliteVariationRarity.Uncommon;
	}

	public override bool CanApply(NPC npc) {
		return ServerConfig.Instance.EnableArmored;
	}

	public override void OnApply(NPC npc) {
		if (ApplyEliteVariation) {
			npc.HitSound = SoundID.NPCHit4;
		}
	}

	public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers) {
		if (ApplyEliteVariation) {
			modifiers.DisableCrit();
		}
	}

	public override void DrawEffects(NPC npc, ref Color drawColor) {
		if (ApplyEliteVariation) {
			drawColor = NPC.buffColor(drawColor, 1f, 1f - 0.1f, 1f - 0.8f, 1f);

			if (Main.rand.NextBool(3)) {
				Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.Torch);
				dust.noGravity = true;
			}
		}
	}
}
