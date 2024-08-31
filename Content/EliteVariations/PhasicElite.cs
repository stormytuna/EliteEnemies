using System.IO;
using EliteEnemies.Common;
using EliteEnemies.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace EliteEnemies.Content.EliteVariations;

public class PhasicElite : EliteVariation
{
	private bool _canInteract = true;

	private int _phasicTimer = 0;

	private int _phasicTimerMax = 60;

	public override EliteVariationRarity Rarity => EliteVariationRarity.Legendary;

	public override bool CanApply(NPC npc) {
		return NPC.downedBoss3 && !npc.IsWorm() && ServerConfig.Instance.EnablePhasic;
	}

	public override void SafeLoad() {
		On_Main.DrawNPCDirect += static (orig, self, spriteBatch, npc, behindTiles, screenPos) => {
			if (npc.GetGlobalNPC<PhasicElite>().ApplyEliteVariation && !npc.GetGlobalNPC<PhasicElite>()._canInteract) {
				return;
			}

			orig(self, spriteBatch, npc, behindTiles, screenPos);
		};
	}

	public override void SafeOnSpawn(NPC npc, IEntitySource source) {
		if (ApplyEliteVariation) {
			_phasicTimerMax = Main.rand.Next(45, 180);
		}
	}


	public override void AI(NPC npc) {
		if (!ApplyEliteVariation) {
			return;
		}

		if (++_phasicTimer >= _phasicTimerMax) {
			_phasicTimer = 0;
			_canInteract = !_canInteract;

			for (int i = 0; i < 15; i++) {
				Vector2 dustPosition = npc.Center;
				Vector2 dustVelocity = (MathHelper.TwoPi * (i / 15f)).ToRotationVector2();
				if (!_canInteract) {
					dustPosition += dustVelocity * 48f;
					dustVelocity *= -1f;
				}

				Dust dust = Dust.NewDustPerfect(dustPosition, DustID.UndergroundHallowedEnemies, dustVelocity * 5f + npc.velocity);
				dust.noGravity = true;
			}
		}
	}

	public override void DrawEffects(NPC npc, ref Color drawColor) {
		if (ApplyEliteVariation) {
			drawColor *= Main.rand.NextFloat(0.6f, 0.9f);
		}
	}

	public override bool? DrawHealthBar(NPC npc, byte hbPosition, ref float scale, ref Vector2 position) {
		return _canInteract;
	}

	public override bool CanHitNPC(NPC npc, NPC target) {
		return _canInteract;
	}

	public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot) {
		return _canInteract;
	}

	public override bool? CanBeHitByItem(NPC npc, Player player, Item item) {
		return _canInteract ? null : false;
	}

	public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile) {
		return _canInteract ? null : false;
	}

	public override bool CanBeHitByNPC(NPC npc, NPC attacker) {
		return _canInteract;
	}

	public override void SafeSendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter) {
		binaryWriter.Write7BitEncodedInt(_phasicTimerMax);
	}

	public override void SafeReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader) {
		_phasicTimerMax = binaryReader.Read7BitEncodedInt();
	}
}
