using EliteEnemies.Common;
using EliteEnemies.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace EliteEnemies.Content.EliteVariations;

public class VolatileElite : EliteVariation
{
	private float _strength = 1f;

	public override EliteVariationRarity Rarity => EliteVariationRarity.SuperRare;

	public override void OnSpawn(NPC npc, IEntitySource source) {
		base.OnSpawn(npc, source);

		if (ApplyEliteVariation) {
			_strength = Main.rand.NextFloat(1f, 2f);
		}
	}

	private Rectangle GetHurtBox(NPC npc) {
		int width = (int)(npc.width * _strength);
		int height = (int)(npc.height * _strength);
		int x = (int)(npc.position.X - (width / 2));
		int y = (int)(npc.position.Y - (height / 2));
		return new Rectangle(x, y, width, height);
	}

	public override void OnKill(NPC npc) {
		if (!ApplyEliteVariation) {
			return;
		}

		Rectangle hurtBox = GetHurtBox(npc);
		foreach (var player in Main.ActivePlayers) {
			if (hurtBox.Intersects(player.Hitbox)) {
				player.Hurt(PlayerDeathReason.ByNPC(npc.whoAmI), (int)(npc.damage * _strength), int.Sign((int)(player.Center.X - npc.Center.X)));
			}
		}
	}

	public override void HitEffect(NPC npc, NPC.HitInfo hit) {
		if (!ApplyEliteVariation || npc.life > 0) {
			return;
		}

		SoundEngine.PlaySound(SoundID.Item14, npc.Center);

		Rectangle hurtBox = GetHurtBox(npc);
		int numDust = (int)(hurtBox.Width * hurtBox.Height * 0.008f);
		for (int i = 0; i < numDust; i++) {
			Dust dust = Dust.NewDustDirect(hurtBox.TopLeft(), hurtBox.Width, hurtBox.Height, DustID.Torch, 0f, 0f, 100, default, 3f);
			dust.noGravity = true;
			dust.velocity *= 3f * _strength;
			dust = Dust.NewDustDirect(hurtBox.TopLeft(), hurtBox.Width, hurtBox.Height, DustID.Torch, 0f, 0f, 100, default, 2f);
			dust.velocity *= 1.5f * _strength;
		}

		int numGore = (int)(hurtBox.Width * hurtBox.Height * 0.002f);
		for (int i = 0; i < numGore; i++) {
			Vector2 goreSpawnPosition = Main.rand.NextVectorWithin(hurtBox);
			Gore smokeGore = Gore.NewGoreDirect(npc.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64));
			smokeGore.velocity.X *= 0.75f * _strength;
			smokeGore.velocity.Y *= 0.75f * _strength;
		}
	}

	public override void DrawEffects(NPC npc, ref Color drawColor) {
		if (ApplyEliteVariation) {
			drawColor = NPC.buffColor(drawColor, 1f, 1 - 0.4f, 1 - 0.5f, 1f);
		}
	}
}
