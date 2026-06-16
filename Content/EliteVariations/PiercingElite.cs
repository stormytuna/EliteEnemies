using EliteEnemies.Common;

namespace EliteEnemies.Content.EliteVariations;

public class PiercingElite : EliteVariation
{
	private int _armorPenetration = 0;

	public override EliteVariationRarity Rarity {
		get => EliteVariationRarity.Common;
	}

	public override bool CanApply(NPC npc) {
		return ServerConfig.Instance.EnablePiercing;
	}

    public override void OnApply(NPC npc) {
		_armorPenetration = (int)(((npc.damage * 0.2f) + 5f) * Main.rand.NextFloat(0.8f, 1.2f));
    }

    public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers) {
		modifiers.ArmorPenetration += _armorPenetration;
    }

    public override void ModifyHitNPC(NPC npc, NPC target, ref NPC.HitModifiers modifiers)
    {
		modifiers.ArmorPenetration += _armorPenetration;
    }
}
