using System.IO;
using EliteEnemies.Common;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace EliteEnemies.Content.EliteVariations;

public class GlowingElite : EliteVariation
{
	private Color _lightColor = Color.Transparent;

	public override EliteVariationRarity Rarity => EliteVariationRarity.SuperRare;

	public override float SpawnChance => 1f;

	public override void SafeOnSpawn(NPC npc, IEntitySource source) {
		if (ApplyEliteVariation) {
			_lightColor = new Color(Main.rand.NextFloat(), Main.rand.NextFloat(), Main.rand.NextFloat()) * Main.rand.NextFloat(0.5f, 2f);
		}
	}

	public override void AI(NPC npc) {
		if (!ApplyEliteVariation) {
			return;
		}

		Lighting.AddLight(npc.Center, _lightColor.ToVector3());
	}

	public override void SafeSendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter) {
		binaryWriter.WriteRGB(_lightColor);
	}

	public override void SafeReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader) {
		_lightColor = binaryReader.ReadRGB();
	}
}
