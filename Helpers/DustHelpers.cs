using System.Collections.Generic;

namespace EliteEnemies.Helpers;

public class DustHelpers
{
	/// <summary>
	///     Creates a lightning bolt made of dust from the source to the destination
	/// </summary>
	/// <param name="source">The source location, where the lightning bolt starts</param>
	/// <param name="dest">The destination location, where the lightning bolt ends</param>
	/// <param name="dustId">The dust id of the dust to create along the lightning bolt</param>
	/// <param name="scale">The scale of the dust</param>
	/// <param name="sway">How far away from the center line the zigzag offset is allowed to be</param>
	/// <param name="jagednessNumerator">
	///     How strictly the sway is moved back towards the center, usually don't make this higher
	///     than 2
	/// </param>
	public static void MakeLightningDust(Vector2 source, Vector2 dest, int dustId, float scale, float sway = 80f,
		float jagednessNumerator = 1f) {
		List<Vector2> dustPoints = CreateLightningBolt(source, dest, sway, jagednessNumerator);

		for (int i = 1; i < dustPoints.Count; i++) {
			Vector2 start = dustPoints[i - 1];
			Vector2 end = dustPoints[i];
			float numDust = (end - start).Length() * 0.4f;

			for (int j = 0; j < numDust; j++) {
				float lerp = j / numDust;
				Vector2 dustPosition = Vector2.Lerp(start, end, lerp);

				Dust d = Dust.NewDustPerfect(dustPosition, dustId, Scale: scale);
				d.noGravity = true;
				d.velocity = Main.rand.NextVector2Circular(0.3f, 0.3f);
			}
		}
	}

	private static List<Vector2> CreateLightningBolt(Vector2 source, Vector2 dest, float sway = 80f,
		float jaggednessNumerator = 1f) {
		List<Vector2> results = new();
		Vector2 tangent = dest - source;
		Vector2 normal = Vector2.Normalize(new Vector2(tangent.Y, -tangent.X));
		float length = tangent.Length();

		List<float> positions = new() { 0 };

		for (int i = 0; i < length / 16f; i++) {
			positions.Add(Main.rand.NextFloat());
		}

		positions.Sort();

		float jaggedness = jaggednessNumerator / sway;

		Vector2 prevPoint = source;
		float prevDisplacement = 0f;
		for (int i = 1; i < positions.Count; i++) {
			float pos = positions[i];

			// used to prevent sharp angles by ensuring very close positions also have small perpendicular variation.
			float scale = length * jaggedness * (pos - positions[i - 1]);

			// defines an envelope. Points near the middle of the bolt can be further from the central line.
			float envelope = pos > 0.95f ? 20 * (1 - pos) : 1;

			float displacement = Main.rand.NextFloat(-sway, sway);
			displacement -= (displacement - prevDisplacement) * (1 - scale);
			displacement *= envelope;

			Vector2 point = source + (pos * tangent) + (displacement * normal);
			results.Add(point);
			prevPoint = point;
			prevDisplacement = displacement;
		}

		results.Add(prevPoint);
		results.Add(dest);
		results.Insert(0, source);

		return results;
	}
}
