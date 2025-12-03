using EliteEnemies.Common;

namespace EliteEnemies.Content.EliteVariations;

public class LeechingElite : EliteVariation
{
	public override EliteVariationRarity Rarity {
		get => EliteVariationRarity.SuperRare;
	}

	public override bool CanApply(NPC npc) {
		return NPC.downedBoss3;
	}

	public override void AI(NPC npc) {
		if (ApplyEliteVariation && Main.rand.NextBool()) {
			Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.VampireHeal);
			dust.scale = Main.rand.NextFloat(0.9f, 1.2f);
			dust.alpha = 100;
			dust.velocity *= 0.5f;
			dust.position += npc.velocity * Main.rand.NextFloat();
			dust.noGravity = true;
		}
	}

	public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo) {
		if (ApplyEliteVariation) {
			BroadcastSpawnLeechingEliteProjectile(npc.whoAmI, target.Center);
		}
	}

	public static void HandleSpawnLeechingEliteProjectile(int targetWhoAmI, Vector2 position) {
		NPC target = Main.npc[targetWhoAmI];
		NPC leechNPC = NPC.NewNPCDirect(target.GetSource_FromThis(), position, ModContent.NPCType<LeechingEliteLeechProjectile>(), ai0: targetWhoAmI);
		leechNPC.velocity = Main.rand.NextVector2CircularEdge(10f, 10f);
	}

	public static void BroadcastSpawnLeechingEliteProjectile(int targetWhoAmI, Vector2 position) {
		if (Main.netMode == NetmodeID.SinglePlayer) {
			HandleSpawnLeechingEliteProjectile(targetWhoAmI, position);
			return;
		}

		ModPacket packet = ModContent.GetInstance<EliteEnemies>().GetPacket();
		packet.Write((byte)EliteEnemies.MessageType.SpawnLeechingEliteProjectile);
		packet.Write7BitEncodedInt(targetWhoAmI);
		packet.WriteVector2(position);
		packet.Send();
	}
}

public class ShotByLeechingEliteGlobalProjectile : ShotByEliteVariationGlobalProjectile<LeechingElite>
{
	public override void OnHitPlayer(Projectile projectile, Player target, Player.HurtInfo info) {
		if (ApplyEliteChanges) {
			LeechingElite.BroadcastSpawnLeechingEliteProjectile(Parent.whoAmI, target.Center);
		}
	}
}

public class LeechingEliteLeechProjectile : ModNPC
{
	private NPC _target;
	private bool _firstFrame = true;
	private int _timer = 30;

	public override string Texture {
		get => Assets.EmptyTexturePath;
	}

	public override void SetDefaults() {
		NPC.width = 10;
		NPC.height = 10;
		NPC.aiStyle = -1;
		NPC.lifeMax = 1;

		NPC.HitSound = SoundID.NPCHit3;
		NPC.DeathSound = SoundID.NPCDeath3;
		NPC.noGravity = true;
		NPC.knockBackResist = 0f;
	}

	public override void AI() {
		if (_firstFrame) {
			_firstFrame = false;
			_target = Main.npc[(int)NPC.ai[0]];
		}

		if (!_target.active && Main.netMode != NetmodeID.MultiplayerClient) {
			NPC.StrikeInstantKill();
			return;
		}

		_timer--;
		if (_timer <= 0) {
			MathHelpers.SmoothHoming(NPC, _target.Center, 0.3f, 16f, _target.velocity);

			if (NPC.Hitbox.Intersects(_target.Hitbox) && Main.netMode != NetmodeID.MultiplayerClient) {
				NPC.StrikeInstantKill();

				float healStrength = _target.CountsAsBoss() ? 0.01f : 0.2f;
				int heal = int.Max((int)(_target.lifeMax * healStrength), 1);
				_target.life += heal;
				if (_target.life > _target.lifeMax) {
					_target.life = _target.lifeMax;
				}

				_target.HealEffect(heal);
			}
		}
		else {
			NPC.velocity *= 0.95f;
		}

		if (NPC.collideX) {
			NPC.velocity.X = -NPC.velocity.X;
		}

		if (NPC.collideY) {
			NPC.velocity.Y = -NPC.velocity.Y;
		}

		for (int i = 0; i < 2; i++) {
			Dust dust = Dust.NewDustPerfect(NPC.Center, DustID.VampireHeal);
			dust.scale = Main.rand.NextFloat(0.9f, 1.2f);
			dust.alpha = 100;
			dust.velocity *= 0.5f;
			dust.position += NPC.velocity * Main.rand.NextFloat();
			dust.noGravity = true;
		}
	}
}
