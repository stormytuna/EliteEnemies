using System.IO;
using EliteEnemies.Common;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace EliteEnemies.Content.EliteVariations;

public class JackedElite : EliteVariation
{
	private float _strength = 1f;

	public override EliteVariationRarity Rarity => EliteVariationRarity.Common;

	public override bool CanApply(NPC npc) {
		return ServerConfig.Instance.EnableJacked;
	}

	public override void SafeOnSpawn(NPC npc, IEntitySource source) {
		if (ApplyEliteVariation) {
			_strength = Main.rand.NextFloat(1.5f, 2f);
		}
	}

	public override void OnApply(NPC npc) {
		if (!ApplyEliteVariation) {
			return;
		}

		npc.damage = (int)(npc.damage * _strength);
		npc.defense = (int)(npc.defense * _strength);
		npc.lifeMax = (int)(npc.lifeMax * _strength);
		npc.life = npc.lifeMax;
	}

	public override void SafeSendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter) {
		binaryWriter.Write(_strength);
	}

	public override void SafeReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader) {
		_strength = binaryReader.ReadSingle();
	}
}
