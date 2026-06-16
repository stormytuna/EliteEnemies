using EliteEnemies.Common;
using Terraria.GameContent.Events;

namespace EliteEnemies.Content.EliteVariations;

public class PartyElite : EliteVariation
{
	public override EliteVariationRarity Rarity {
		get => EliteVariationRarity.Rare;
	}

    public override void ModifySpawnWeight(ref float weight) {
		if (BirthdayParty.PartyIsUp) {
			weight *= 10f;
		}
    }

	public override bool CanApply(NPC npc) {
		return ServerConfig.Instance.EnableParty;
	}

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
