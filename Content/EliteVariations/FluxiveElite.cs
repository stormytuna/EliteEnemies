using EliteEnemies.Common;
using Terraria.Audio;

namespace EliteEnemies.Content.EliteVariations;

public class FluxiveElite : EliteVariation
{
	public override EliteVariationRarity Rarity {
		get => EliteVariationRarity.SuperRare;
	}

	public override bool CanApply(NPC npc) {
		return NPC.downedMechBossAny && ServerConfig.Instance.EnableFluxive;
	}

	public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo) {
		if (ApplyEliteVariation) {
			target.AddBuff(ModContent.BuffType<FluxiveBuff>(), Main.rand.Next(30, 90));
		}
	}

	public override void DrawEffects(NPC npc, ref Color drawColor) {
		if (ApplyEliteVariation) {
			drawColor = NPC.buffColor(drawColor, 0.9f, 0f, 1f, 1f);
		}
	}
}

public class ShotByFluxiveEliteGlobalProjectile : ShotByEliteVariationGlobalProjectile<FluxiveElite>
{
	public override void OnHitPlayer(Projectile projectile, Player target, Player.HurtInfo info) {
		if (ApplyEliteChanges) {
			target.AddBuff(ModContent.BuffType<FluxiveBuff>(), Main.rand.Next(30, 90));
		}
	}
}

public class FluxiveBuff : ModBuff
{
	public override void SetStaticDefaults() {
		Main.debuff[Type] = true;
	}

	public override void Update(Player player, ref int buffIndex) {
		player.GetModPlayer<FluxivePlayer>().Active = true;
	}
}

public class FluxivePlayer : ModPlayer
{
	public bool Active;

	private bool _activeOld;

	public override void ResetEffects() {
		_activeOld = Active;
		Active = false;
	}

	public override void PreUpdateMovement() {
		if (!Active) {
			return;
		}

		Player.forcedGravity = 2;
		if (!_activeOld) {
			Player.gravDir = -1;
			Player.fallStart = (int)(Player.position.Y / 16f);
			Player.jump = 0;
			SoundEngine.PlaySound(SoundID.Item8, Player.Center);
		}
	}
}
