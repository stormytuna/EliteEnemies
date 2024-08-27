using System.IO;
using EliteEnemies.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace EliteEnemies.Content.EliteVariations;

public class InspiringElite : EliteVariation
{
	private static Asset<Texture2D> _auraTexture;

	private float _strength = 0f;

	public override EliteVariationRarity Rarity => EliteVariationRarity.SuperRare;

	public override bool CanApply(NPC npc) {
		return NPC.downedBoss2;
	}

	public override void SafeLoad() {
		_auraTexture = Mod.Assets.Request<Texture2D>("Content/EliteVariations/InspiringEliteAura");
	}

	public override void SafeOnSpawn(NPC npc, IEntitySource source) {
		if (ApplyEliteVariation) {
			_strength = Main.rand.NextFloat();
		}
	}

	public static float StrengthOfNearbyInspiringElites(NPC npc) {
		float strength = 0;

		foreach (NPC otherNpc in Main.ActiveNPCs) {
			if (otherNpc.GetGlobalNPC<InspiringElite>().ApplyEliteVariation && npc.WithinRange(otherNpc.Center, 25 * 16) && npc.whoAmI != otherNpc.whoAmI) {
				strength += otherNpc.GetGlobalNPC<InspiringElite>()._strength;
			}
		}

		return strength;
	}

	public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers) {
		float strength = StrengthOfNearbyInspiringElites(npc);
		if (strength <= 0f) {
			return;
		}

		modifiers.Defense += float.Lerp(0.2f, 0.6f, strength);
	}

	public override void ModifyHitNPC(NPC npc, NPC target, ref NPC.HitModifiers modifiers) {
		float strength = StrengthOfNearbyInspiringElites(npc);
		if (strength <= 0f) {
			return;
		}

		modifiers.FinalDamage += float.Lerp(0.2f, 0.6f, strength);
	}

	public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers) {
		float strength = StrengthOfNearbyInspiringElites(npc);
		if (strength <= 0f) {
			return;
		}

		modifiers.FinalDamage += float.Lerp(0.2f, 0.6f, strength);
	}

	public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
		if (!ApplyEliteVariation) {
			return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
		}

		Vector2 position = npc.Center - screenPos + new Vector2(0f, npc.gfxOffY);

		// Adding whoAmI for simple offset between each npc with this variation
		float rotation = (float)(Main.timeForVisualEffects * 0.01 + npc.whoAmI) % MathHelper.TwoPi;

		spriteBatch.Draw(_auraTexture.Value, position, _auraTexture.Value.Bounds, Color.LightGoldenrodYellow with { A = 0 } * 0.6f, rotation, _auraTexture.Size() / 2f, 1f, SpriteEffects.None, 0);

		return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
	}

	public override void SafeSendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter) {
		binaryWriter.Write(_strength);
	}

	public override void SafeReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader) {
		_strength = binaryReader.ReadSingle();
	}
}

public class InspiringEliteGlobalProjectile : GlobalProjectile
{
	private bool _apply = false;
	private float _strength = 0f;

	public override bool InstancePerEntity => true;

	public override void OnSpawn(Projectile projectile, IEntitySource source) {
		if (source is EntitySource_Parent { Entity: NPC npc }) {
			float strength = InspiringElite.StrengthOfNearbyInspiringElites(npc);
			if (strength >= 0f) {
				_apply = true;
				_strength = strength;
			}
		}
	}

	public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers) {
		if (!_apply) {
			return;
		}

		modifiers.FinalDamage += float.Lerp(0.2f, 0.6f, _strength);
	}

	public override void ModifyHitPlayer(Projectile projectile, Player target, ref Player.HurtModifiers modifiers) {
		if (!_apply) {
			return;
		}

		modifiers.FinalDamage += float.Lerp(0.2f, 0.6f, _strength);
	}
}

