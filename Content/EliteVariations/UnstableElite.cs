using EliteEnemies.Common;

namespace EliteEnemies.Content.EliteVariations;

public class UnstableElite : EliteVariation
{
	public override EliteVariationRarity Rarity {
		get => EliteVariationRarity.SuperRare;
	}

	public override bool CanApply(NPC npc) {
		return Main.hardMode && ServerConfig.Instance.EnableUnstable;
	}

	public override void OnApply(NPC npc) {
		if (!ApplyEliteVariation) {
			return;
		}

		NPCRenderRedirectSystem.RegisterRenderAction(npc, (int)RenderPriority.Last, static (npc, renderTarget, spriteBatch) => {
			var randomDir = Main.rand.NextVector2Circular(1f, 1f);
			var randomOffset = (randomDir * npc.Size.Length() * 0.1f).ToPoint();
			spriteBatch.Draw(renderTarget, new Rectangle(randomOffset.X, randomOffset.Y, Main.screenWidth, Main.screenHeight), Color.White);
		});
	}

	private int _projectileTimer;

	public override void AI(NPC npc) {
		if (!ApplyEliteVariation) {
			return;
		}

		if (Main.netMode != NetmodeID.MultiplayerClient) {
			_projectileTimer++;
			if (_projectileTimer >= 60) {
				_projectileTimer = 0;

				var velocity = Main.rand.NextVector2CircularEdge(16f, 16f);
				Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, velocity, ProjectileID.ShadowBeamHostile, 10, 0f, Main.myPlayer);
			}
		}
	}

	public override void DrawEffects(NPC npc, ref Color drawColor) {
		if (ApplyEliteVariation) {
			drawColor = NPC.buffColor(drawColor, 0.95f, 0.2f, 0.95f, 1f);
		}
	}
}
