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
		return NPC.downedMechBossAny && ServerConfig.Instance.EnableStinky;
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
				Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, velocity, ModContent.ProjectileType<StinkyCloud>(), 10, 0f, Main.myPlayer);
			}
		}
	}
}

public class StinkyCloud : ModProjectile
{
	public override string Texture {
		get => $"Terraria/Images/Projectile_{ProjectileID.ToxicCloud}";
	}

	public override void SetDefaults() {
		Projectile.CloneDefaults(ProjectileID.ToxicCloud);
		
		Projectile.hostile = true;
		Projectile.friendly = false;
		
		AIType = ProjectileID.ToxicCloud;
	}

	public override bool PreDraw(ref Color lightColor) {
		var texture = TextureAssets.Projectile[Type].Value;
		
		var drawData = new DrawData {
			texture = texture,
			position = (Projectile.Center - Main.screenPosition).Floor(),
			sourceRect = texture.Frame(),
			origin = texture.Size() / 2f,
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
