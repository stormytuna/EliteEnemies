using EliteEnemies.Common;

namespace EliteEnemies;

public static class EliteEnemiesHelpers
{
	/// <summary>
	/// Checks if a given NPC has the specified Elite Variation.
	/// </summary>
	/// <typeparam name="TVariation">The type of the Elite Variation to check for.</typeparam>
	/// <param name="npc">The NPC to check.</param>
	/// <returns>true if the NPC has the specified Elite Variation, false otherwise.</returns>
	public static bool HasEliteVariation<TVariation>(this NPC npc) where TVariation : EliteVariation {
		foreach (GlobalNPC global in npc.Globals) {
			if (global is TVariation t && t.ApplyEliteVariation) {
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Checks if a given NPC does not have the specified Elite Variation.
	/// </summary>
	/// <typeparam name="TVariation">The type of the Elite Variation to check for.</typeparam>
	/// <param name="npc">The NPC to check.</param>
	/// <returns>true if the NPC does not have the specified Elite Variation, false otherwise.</returns>
	public static bool HasNotEliteVariation<TVariation>(this NPC npc) where TVariation : EliteVariation {
		return !HasEliteVariation<TVariation>(npc);
	}
	
	/// <summary>
	/// Counts the number of Elite Variations currently active on the NPC.
	/// </summary>
	/// <param name="npc">The NPC to check.</param>
	/// <returns>The number of Elite Variations currently active on the NPC.</returns>
	public static int NumActiveEliteVariations(this NPC npc) {
		int count = 0;

		foreach (GlobalNPC global in npc.Globals) {
			if (global is EliteVariation variation && variation.ApplyEliteVariation) {
				count++;
			}
		}

		return count;
	}
}
