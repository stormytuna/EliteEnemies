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
		return npc.HasNotEliteVariation<HugeElite>();
	}

	public override void OnSpawn(NPC npc, IEntitySource source) {
		base.OnSpawn(npc, source);

		if (ApplyEliteVariation) {
			_strength = Main.rand.NextFloat(1f, 2f);
		}
	}

	public override void OnApply(NPC npc) {
		if (ApplyEliteVariation) {
			npc.scale /= 1.5f * _strength;
			npc.width = (int)(npc.width * npc.scale);
			npc.height = (int)(npc.height * npc.scale);
		}
	}

	public override bool PreAI(NPC npc) {
		if (ApplyEliteVariation) {
			npc.velocity *= 1 / (1.3f * _strength);
		}

		return base.PreAI(npc);
	}

	public override void PostAI(NPC npc) {
		if (ApplyEliteVariation) {
			npc.velocity *= 1.3f * _strength;
		}
	}

	public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers) {
		if (ApplyEliteVariation) {
			modifiers.Knockback *= 1.5f * _strength;
		}
	}

	public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter) {
		binaryWriter.Write(_strength);
		base.SendExtraAI(npc, bitWriter, binaryWriter);
	}

	public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader) {
		_strength = binaryReader.ReadSingle();
		base.ReceiveExtraAI(npc, bitReader, binaryReader);
	}
}
