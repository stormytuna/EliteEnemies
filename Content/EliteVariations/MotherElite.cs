using EliteEnemies.Common;
using EliteEnemies.Core;
using Terraria;

namespace EliteEnemies.Content.EliteVariations;

public class MotherElite : EliteVariation
{
	public override EliteVariationRarity Rarity => EliteVariationRarity.SuperRare;

	public override bool CanApply(NPC npc) {
		return !npc.IsWorm() && ServerConfig.Instance.EnableMother;
	}

	public override void OnKill(NPC npc) {
		if (!ApplyEliteVariation) {
			return;
		}

		int numChildren = Main.rand.Next(1, 4);
		for (int i = 0; i < numChildren; i++) {
			NPC child = NPC.NewNPCDirect(npc.GetSource_Death(), Main.rand.NextVector2FromRectangle(npc.Hitbox), npc.type);
			if (ServerConfig.Instance.ApplyScaleChanges) {
				child.scale *= 0.8f;
			}
			child.lifeMax /= 2;
			child.damage /= 2;
		}
	}
}
