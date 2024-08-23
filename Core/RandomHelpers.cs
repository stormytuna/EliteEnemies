using Microsoft.Xna.Framework;
using Terraria.Utilities;

namespace EliteEnemies.Core;

public static class RandomHelpers
{
	public static Vector2 NextVectorWithin(this UnifiedRandom rand, Rectangle rect) {
		return new Vector2(rect.Left + rand.Next(rect.Width + 1), rect.Top + rand.Next(rect.Height + 1));
	}
}
