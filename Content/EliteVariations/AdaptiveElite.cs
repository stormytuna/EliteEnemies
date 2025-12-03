using System.Collections.Generic;
using EliteEnemies.Common;
using FishUtils.DataStructures;

namespace EliteEnemies.Content.EliteVariations;

public class AdaptiveElite : EliteVariation
{
	private const float MinDamageMult = 0.5f;

	private readonly Dictionary<int, float> _damageclassMults = new();

	public override EliteVariationRarity Rarity {
		get => EliteVariationRarity.SuperRare;
	}

	public override bool CanApply(NPC npc) {
		return Main.hardMode;
	}

	public override void OnApply(NPC npc) {
		if (!ApplyEliteVariation) {
			return;
		}

		for (int i = 0; i < DamageClassLoader.DamageClassCount; i++) {
			_damageclassMults.Add(i, 1f);
		}

		NPCRenderRedirectSystem.RegisterRenderAction(npc, (int)RenderPriority.Middle, static (npc, renderTarget, spriteBatch) => {
			Main.spriteBatch.TakeSnapshotAndEnd(out SpriteBatchParams sbParams);

			Effect shader = Assets.Shaders.Outline.Value;
			shader.Parameters["outlineColor"].SetValue(Color.DarkGray.ToVector3());
			shader.Parameters["screenSize"].SetValue(Main.ScreenSize.ToVector2());

			Main.spriteBatch.Begin(sbParams with { Effect = shader });

			spriteBatch.Draw(renderTarget, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);

			Main.spriteBatch.Restart(sbParams);
		});
	}

	public override void AI(NPC npc) {
		if (!ApplyEliteVariation) {
			return;
		}

		foreach (int key in _damageclassMults.Keys) {
			_damageclassMults[key] += 0.001f;
			if (_damageclassMults[key] >= 1f) {
				_damageclassMults[key] = 1f;
			}
		}
	}

	private void ApplyDamageMult(NPC target, ref NPC.HitModifiers modifiers) {
		if (!ApplyEliteVariation || !_damageclassMults.TryGetValue(modifiers.DamageType.Type, out float value)) {
			return;
		}

		modifiers.FinalDamage *= value;

		float damageMult = _damageclassMults[modifiers.DamageType.Type] -= 0.01f;
		if (_damageclassMults[modifiers.DamageType.Type] <= MinDamageMult) {
			_damageclassMults[modifiers.DamageType.Type] = MinDamageMult;
		}

		int numDust = (int)(damageMult * 10f);
		DustHelpers.MakeDustExplosion(target.Center, 8f, DustID.Smoke, numDust);
	}

	public override void ModifyHitByItem(NPC npc, Player player, Item item, ref NPC.HitModifiers modifiers) {
		ApplyDamageMult(npc, ref modifiers);
	}

	public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers) {
		ApplyDamageMult(npc, ref modifiers);
	}

	public override void DrawEffects(NPC npc, ref Color drawColor) {
		if (ApplyEliteVariation) {
			drawColor = NPC.buffColor(drawColor, 0.8f, 0.8f, 0.8f, 1f);
		}
	}
}
