using EliteEnemies.Common;

namespace EliteEnemies.Content.EliteVariations;

public class MoltenElite : EliteVariation
{
	public override EliteVariationRarity Rarity {
		get => EliteVariationRarity.Legendary;
	}

	public override bool CanApply(NPC npc) {
		bool underground = npc.Center.Y > (Main.rockLayer * 16f);
		return underground && Main.hardMode;
	}

	public override void AI(NPC npc) {
		if (!ApplyEliteVariation) {
			return;
		}

		if (Main.rand.NextBool(5)) {
			Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.RedTorch);
			dust.velocity *= 5f;
			dust.noGravity = true;
			dust.noLight = true;
		}
	}

	public override void OnKill(NPC npc) {
		if (!ApplyEliteVariation) {
			return;
		}

		Point tileCoords = npc.Center.ToTileCoordinates();
		WorldGen.PlaceLiquid(tileCoords.X, tileCoords.Y, (byte)LiquidID.Lava, byte.MaxValue);
		WorldGen.SquareTileFrame(tileCoords.X, tileCoords.Y);
	}

	public override void DrawEffects(NPC npc, ref Color drawColor) {
		if (ApplyEliteVariation) {
			drawColor = NPC.buffColor(drawColor, 1f, 0.4f, 0.4f, 1f);
		}
	}
}
