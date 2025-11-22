using EliteEnemies.Common;

namespace EliteEnemies.Content.EliteVariations;

public class StoicElite : EliteVariation
{
	public override EliteVariationRarity Rarity {
		get => EliteVariationRarity.Rare;
	}

	public override bool CanApply(NPC npc) {
		return ServerConfig.Instance.EnableStoic;
	}

	public override bool? DrawHealthBar(NPC npc, byte hbPosition, ref float scale, ref Vector2 position) {
		return ApplyEliteVariation ? false : null;
	}

	public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers) {
		if (ApplyEliteVariation) {
			modifiers.HideCombatText();
		}
	}
}

public class Test : GlobalNPC
{
	public override bool? DrawHealthBar(NPC npc, byte hbPosition, ref float scale, ref Vector2 position) {
		Main.NewText("rock and stone");
		scale = 10f;
		return base.DrawHealthBar(npc, hbPosition, ref scale, ref position);
	}
}
