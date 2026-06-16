using EliteEnemies.Common;
using Terraria.DataStructures;
using Terraria.GameContent;

namespace EliteEnemies.Content.EliteVariations;

public class StinkyElite : EliteVariation
{
	private int _stinkCloudTimer = 0;

	public override EliteVariationRarity Rarity {
		get => EliteVariationRarity.Rare;
	}

	public override bool CanApply(NPC npc) {
		return ServerConfig.Instance.EnableStinky && NPC.downedMechBossAny;
	}

	public override void AI(NPC npc) {
		if (!ApplyEliteVariation) {
			return;
		}

		npc.AddBuff(BuffID.Stinky, 2, false);

		if (Main.netMode != NetmodeID.MultiplayerClient) {
			_stinkCloudTimer++;
			if (_stinkCloudTimer >= 20) {
				_stinkCloudTimer = 0;

				Vector2 velocity = Main.rand.NextVector2Circular(3f, 3f);
				Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, velocity, ModContent.ProjectileType<StinkyCloud>(), 0, 0f, Main.myPlayer);
			}
		}
	}
}

public class StinkyCloud : ModProjectile
{
	public bool Active => Projectile.Opacity > 0.35f;

	public override void SetStaticDefaults() {
		Main.projFrames[Type] = 3;
	}

	public override void SetDefaults() {
		Projectile.CloneDefaults(ProjectileID.ToxicCloud);
		Projectile.hostile = false;
		Projectile.friendly = false;

		AIType = ProjectileID.ToxicCloud;
	}

	public override void AI() {
		if (Projectile.frameCounter == 0) {
			Projectile.frame = Main.rand.Next(Main.projFrames[Type]);
			Projectile.frameCounter = 1;
		}
	}

	public override bool PreDraw(ref Color lightColor) {
		var texture = TextureAssets.Projectile[Type].Value;

		var sourceRect = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
		var drawData = new DrawData {
			texture = texture,
			position = (Projectile.Center - Main.screenPosition).Floor(),
			sourceRect = sourceRect,
			origin = sourceRect.Size() / 2f,
			color = Projectile.GetAlpha(lightColor),
			rotation = Projectile.rotation,
			scale = new Vector2(Projectile.scale),
		};

		var cloudData = drawData with {
			color = drawData.color * 0.25f,
			scale = drawData.scale * (1f + (Projectile.Opacity * 1.75f)),
		};

		cloudData.Draw(Main.spriteBatch);
		drawData.Draw(Main.spriteBatch);

		return false;
	}
}

public class StinkyCloudPlayer : ModPlayer
{
	public override void UpdateBadLifeRegen() {
		bool nearStinkyCloud = false;

		foreach (var projectile in Main.ActiveProjectiles) {
			if (projectile.ModProjectile is StinkyCloud { Active: true } && projectile.Hitbox.Intersects(Player.Hitbox)) {
				nearStinkyCloud = true;
				break;
			}
		}

		if (!nearStinkyCloud) {
			return;
		}

		if (Player.lifeRegen > 0) {
			Player.lifeRegen = 0;
		}

		Player.lifeRegenTime = 0;
		Player.lifeRegen -= 36;
		Player.AddBuff(BuffID.Stinky, 30 * 60);
	}
}
