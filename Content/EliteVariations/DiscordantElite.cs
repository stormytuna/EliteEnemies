using System.Collections.Generic;
using System.Linq;
using EliteEnemies.Common;
using EliteEnemies.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace EliteEnemies.Content.EliteVariations;

public class DiscordantElite : EliteVariation
{
	private int _teleportTimer = 0;
	private int _nextTeleportTime = 6 * 60;

	private Queue<Vector2> _afterImagePositions = new(5);
	private Queue<float> _afterImageRotations = new(5);

	public override EliteVariationRarity Rarity => EliteVariationRarity.Legendary;

	public override bool CanApply(NPC npc) {
		return Main.hardMode && !npc.IsWorm() && ServerConfig.Instance.EnableDiscordant;
	}

	public override void AI(NPC npc) {
		if (!ApplyEliteVariation) {
			return;
		}

		if (_teleportTimer > _nextTeleportTime && Main.netMode != NetmodeID.MultiplayerClient) {
			_teleportTimer = 0;
			_nextTeleportTime = Main.rand.Next(5 * 60, 8 * 60);

			Point currentTile = npc.Center.ToTileCoordinates();
			Vector2 chosenTile = default;
			if (npc.AI_AttemptToFindTeleportSpot(ref chosenTile, currentTile.X, currentTile.Y)) {
				NetMessage.SendData(MessageID.SyncNPC, number: npc.whoAmI);
				Vector2 teleportPosition = (chosenTile * 16f) - npc.Size;
				npc.Teleport(teleportPosition, TeleportationStyleID.RodOfDiscord);
			}
		}

		_afterImagePositions.Enqueue(npc.Center);
		if (_afterImagePositions.Count > 5) {
			_afterImagePositions.Dequeue();
		}
		_afterImageRotations.Enqueue(npc.rotation);
		if (_afterImageRotations.Count > 5) {
			_afterImageRotations.Dequeue();
		}

		if (Main.rand.NextBool(3)) {
			Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.HallowedTorch);
			dust.noGravity = true;
		}

		_teleportTimer++;
	}

	public override void DrawEffects(NPC npc, ref Color drawColor) {
		if (ApplyEliteVariation) {
			drawColor = NPC.buffColor(drawColor, 1f, 1f - 0.4f, 1f, 1f);
		}
	}

	public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
		float velocitySquared = npc.velocity.LengthSquared();
		if (velocitySquared < 2f) {
			return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
		}

		for (int i = _afterImagePositions.Count - 1; i >= 1; i--) {
			Vector2 position = _afterImagePositions.ElementAt(i);
			float rotation = _afterImageRotations.ElementAt(i);
			drawColor *= i / (float)_afterImagePositions.Count;
			drawColor *= float.Lerp(0f, 1f, float.Min(velocitySquared / 12f, 1f));
			SpriteEffects effects = npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			spriteBatch.Draw(TextureAssets.Npc[npc.type].Value, (position - Main.screenPosition).Floor(), npc.frame, drawColor, rotation, npc.frame.Size() / 2f, npc.scale, effects, 0);
		}

		return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
	}
}
