using EliteEnemies.Common;
using EliteEnemies.Helpers;
using Terraria.DataStructures;

namespace EliteEnemies.Content.EliteVariations;

public class ElectricElite : EliteVariation
{
	private int _lightningCooldown = 0;

	public override EliteVariationRarity Rarity {
		get {
			return EliteVariationRarity.SuperRare;
		}
	}

	public override float SpawnChance {
		get {
			return 1f;
		}
	}

	public override bool CanApply(NPC npc) {
		return NPC.downedPlantBoss && ServerConfig.Instance.EnableElectric;
	}

	public override void AI(NPC npc) {
		if (!ApplyEliteVariation) {
			return;
		}

		_lightningCooldown--;

		if (Main.rand.NextBool(30)) {
			Vector2 direction = Main.rand.NextVectorWithin(new Rectangle(-npc.width, -npc.height, npc.width * 2, npc.height * 2));
			DustHelpers.MakeLightningDust(npc.Center + (direction * 0.5f), npc.Center + (direction * 2f), DustID.Electric, 0.6f);
		}

		if (Main.rand.NextBool(8)) {
			Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.Electric);
			dust.noGravity = true;
		}

		foreach (Player player in Main.ActivePlayers) {
			if (npc.WithinRange(player.Center, 10f * 16f)) {
				_lightningCooldown--;

				if (Main.rand.NextBool(8)) {
					Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.Electric);
					dust.noGravity = true;
				}

				if (_lightningCooldown < 0) {
					_lightningCooldown = 60;

					DustHelpers.MakeLightningDust(npc.Center, player.Center, DustID.Electric, 0.8f);

					if (Main.netMode != NetmodeID.MultiplayerClient) {
						int direction = (player.Center.X > npc.Center.X).ToDirectionInt();
						player.Hurt(PlayerDeathReason.ByNPC(npc.whoAmI), npc.damage / 2, direction);
					}
				}

				return;
			}
		}

		_lightningCooldown = 60;
	}

	public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo) {
		if (ApplyEliteVariation) {
			target.AddBuff(BuffID.Electrified, 3 * 60);
		}
	}
}
