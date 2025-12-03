using EliteEnemies.Common;

namespace EliteEnemies.Content.EliteVariations;

public class PartyElite : EliteVariation
{
	public override EliteVariationRarity Rarity {
		get => EliteVariationRarity.Rare;
	}

	/* TODO: This was really cute, would like to keep it!
	public override float SpawnWeight {
		get {
			float baseWeight = base.SpawnWeight;
			if (BirthdayParty.PartyIsUp) {
				return baseWeight * 10f;
			}

			return baseWeight;
		}
	}
	*/

	private void MakeConfettiDust(NPC npc, int numDust) {
		for (int i = 0; i < numDust; i++) {
			int dustType = Main.rand.Next(139, 143);
			var dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, dustType);
			dust.scale = Main.rand.NextFloat(0.8f, 1.3f);
		}
	}

	public override void AI(NPC npc) {
		if (ApplyEliteVariation && Main.rand.NextBool(40)) {
			MakeConfettiDust(npc, 1);
		}
	}

	public override void HitEffect(NPC npc, NPC.HitInfo hit) {
		if (ApplyEliteVariation) {
			MakeConfettiDust(npc, npc.life <= 0 ? 10 : 3);
		}
	}
}
