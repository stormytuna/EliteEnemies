using Terraria.Utilities;

namespace EliteEnemies.Helpers;

public static class RandomHelpers
{
	public static Vector2 NextVectorWithin(this UnifiedRandom rand, Rectangle rect) {
		return new Vector2(rect.Left + rand.Next(rect.Width + 1), rect.Top + rand.Next(rect.Height + 1));
	}

	public static Vector2 NextVectorWithinNormalized(this UnifiedRandom rand, Rectangle rect) {
		return NextVectorWithin(rand, rect with { X = 0, Y = 0 });
	}
}
