using System.IO;
using EliteEnemies.Common;
using EliteEnemies.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace EliteEnemies.Content.EliteVariations;

public class HugeElite : EliteVariation
{
	private float _strength = 1f;

	public override EliteVariationRarity Rarity => EliteVariationRarity.Common;

	public override bool CanApply(NPC npc) {
		return npc.HasNotEliteVariation<TinyElite>();
	}

	public override void SafeOnSpawn(NPC npc, IEntitySource source) {
		if (ApplyEliteVariation) {
			_strength = Main.rand.NextFloat(1f, 2f);
		}
	}

	public override void OnApply(NPC npc) {
		if (ApplyEliteVariation) {
			npc.scale *= 1.5f * _strength;
			npc.width = (int)(npc.width * npc.scale);
			npc.height = (int)(npc.height * npc.scale);
			npc.position.Y -= npc.width / 2; // Fixes npcs getting stuck in ground sometimes
		}
	}

	public override bool SafePreAI(NPC npc) {
		if (ApplyEliteVariation) {
			npc.velocity *= 1.3f * _strength;
		}

		return true;
	}

	public override void PostAI(NPC npc) {
		if (ApplyEliteVariation) {
			npc.velocity *= 1 / (1.3f * _strength);
		}
	}

	public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers) {
		if (ApplyEliteVariation) {
			Main.NewText(_strength);
			modifiers.FinalDamage *= 1.2f * _strength;
		}
	}

	public override void ModifyHitNPC(NPC npc, NPC target, ref NPC.HitModifiers modifiers) {
		if (ApplyEliteVariation) {
			modifiers.FinalDamage *= 1.2f * _strength;
		}
	}

	public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers) {
		if (ApplyEliteVariation) {
			modifiers.Knockback *= 0.75f * _strength;
		}
	}

	public override void SafeSendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter) {
		binaryWriter.Write(_strength);
	}

	public override void SafeReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader) {
		_strength = binaryReader.ReadSingle();
	}
}
