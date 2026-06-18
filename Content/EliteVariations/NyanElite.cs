using System.Collections.Generic;
using System.Linq;
using EliteEnemies.Common;
using Terraria.GameContent;

namespace EliteEnemies.Content.EliteVariations;

public class NyanElite : EliteVariation
{
	private readonly Queue<Vector2> _afterImagePositions = new(20);

	public override EliteVariationRarity Rarity {
		get => EliteVariationRarity.Rare;
	}

	public override bool CanApply(NPC npc) {
		return ServerConfig.Instance.EnableNyan;
	}

    public override void OnApply(NPC npc) {
		if (ApplyEliteVariation) {
			npc.HitSound = SoundID.Item57;
		}
    }

	public override void AI(NPC npc) {
		if (!ApplyEliteVariation) {
			return;
		}

		_afterImagePositions.Enqueue(npc.Center);
		if (_afterImagePositions.Count > 5) {
			_afterImagePositions.Dequeue();
		}
	}

	public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
		if (!ApplyEliteVariation) {
			return true;
		}

		Main.instance.LoadProjectile(ProjectileID.RainbowFront);

		Texture2D rainbowTexture = TextureAssets.Projectile[250].Value;
		Vector2 origin = new Vector2(rainbowTexture.Width / 2, 0f);

		Color white = Microsoft.Xna.Framework.Color.White;
		white.A = 127;

		for (int i = _afterImagePositions.Count - 1; i > 0; i--) {
			Vector2 position = _afterImagePositions.ElementAt(i);
			Vector2 nextPosition = _afterImagePositions.ElementAt(i - 1);
			float rotation = (nextPosition - position).ToRotation() - PiOver2;
			Vector2 scale7 = new Vector2(1f, Vector2.Distance(position, nextPosition) / (float)rainbowTexture.Height);
			Color rainbowDrawColor = white * (1f - ((float)i / _afterImagePositions.Count));
			Main.EntitySpriteDraw(rainbowTexture, position - Main.screenPosition, null, rainbowDrawColor, rotation, origin, scale7, npc.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
		}

		return true;
	}
}
