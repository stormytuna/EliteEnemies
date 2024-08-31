using System.IO;
using EliteEnemies.Common;
using EliteEnemies.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace EliteEnemies.Content.EliteVariations;

public class TinyElite : EliteVariation
{
	private float _strength = 1f;

	public override EliteVariationRarity Rarity => EliteVariationRarity.Common;

	public override bool CanApply(NPC npc) {
		return npc.HasNotEliteVariation<HugeElite>() && !npc.IsWorm() && ServerConfig.Instance.EnableTiny;
	}

	public override void SafeOnSpawn(NPC npc, IEntitySource source) {
		if (ApplyEliteVariation) {
			_strength = Main.rand.NextFloat(1f, 2f);
		}
	}

	public override void OnApply(NPC npc) {
		if (ApplyEliteVariation) {
			if (ServerConfig.Instance.ApplyScaleChanges) {
				npc.scale /= 1.5f * _strength;
			}

			npc.width = (int)(npc.width * 1.5f * _strength);
			npc.height = (int)(npc.height * 1.5f * _strength);
		}
	}

	public override bool SafePreAI(NPC npc) {
		if (ApplyEliteVariation && ServerConfig.Instance.ApplyVelocityChanges) {
			npc.velocity *= 1 / (1.3f * _strength);
		}

		return true;
	}

	public override void PostAI(NPC npc) {
		if (ApplyEliteVariation && ServerConfig.Instance.ApplyVelocityChanges) {
			npc.velocity *= 1.3f * _strength;
		}
	}

	public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers) {
		if (ApplyEliteVariation) {
			modifiers.Knockback *= 1.5f * _strength;
		}
	}

	public override void SafeSendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter) {
		binaryWriter.Write(_strength);
	}

	public override void SafeReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader) {
		_strength = binaryReader.ReadSingle();
	}
}
