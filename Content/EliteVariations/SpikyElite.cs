using System.Collections.Generic;
using EliteEnemies.Common;
using Terraria.DataStructures;

namespace EliteEnemies.Content.EliteVariations;

public class SpikyElite : EliteVariation
{
	private static Asset<Texture2D> _spikeTexture;

	private List<Vector2> _spikeOffsets;

	public override void SafeLoad() {
		_spikeTexture = Mod.Assets.Request<Texture2D>("Content/EliteVariations/SpikyEliteThorn");
	}

	public override EliteVariationRarity Rarity {
		get => EliteVariationRarity.Rare;
	}

	public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone) {
		TryHurtAttacker(npc, player);
	}

	public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone) {
		TryHurtAttacker(npc, Main.player[projectile.owner]);
	}

	private void TryHurtAttacker(NPC npc, Player player) {
		if (!ApplyEliteVariation || !player.active) {
			return;
		}

		int maxDamage = NPCHelpers.ScaleDamageForDifficulty(50);
		float distance = npc.Distance(player.Center);
		if (distance > 3f * 16f) {
			return;
		}

		int damage = (int)(float.Lerp(maxDamage, 0f, distance / 3f * 16f));
		int direction = (player.Center.X > npc.Center.X).ToDirectionInt();
		player.Hurt(PlayerDeathReason.ByNPC(npc.whoAmI), damage, direction, knockback: 0f);
	}

	public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
		if (!ApplyEliteVariation) {
			return;
		}

		if (_spikeOffsets is null) {
			int numSpikes = int.Max((npc.width + npc.height) / 20, 2);
			_spikeOffsets = new List<Vector2>(numSpikes);
			for (int i = 0; i < numSpikes; i++) {
				_spikeOffsets.Add(Main.rand.NextVectorWithinNormalized(npc.Hitbox));
			}
		}

		foreach (Vector2 spikeOffset in _spikeOffsets) {
			Vector2 spikePosition = npc.position + spikeOffset;
			DrawData drawData = new() {
				texture = _spikeTexture.Value,
				position = (spikePosition - screenPos).Floor(),
				sourceRect = _spikeTexture.Frame(),
				color = drawColor,
				rotation = npc.Center.AngleTo(spikePosition) + PiOver2,
				scale = new Vector2(1f),
				origin = _spikeTexture.Size() / 2f,
			};
			drawData.Draw(spriteBatch);
		}
	}
}
