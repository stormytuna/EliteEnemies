namespace EliteEnemies.Common;

public class LootRerollDetour : ILoadable
{
	public void Load(Mod mod) {
		On_NPC.NPCLoot_DropItems += static (orig, self, closestPlayer) => {
			orig(self, closestPlayer);

			foreach (GlobalNPC global in self.Globals) {
				if (global is not EliteVariation eliteVariation || !eliteVariation.ApplyEliteVariation) {
					continue;
				}

				float lootMultiplier = eliteVariation.LootMultiplier;
				while (lootMultiplier > 0f) {
					lootMultiplier--;
					float rollChance = float.Clamp(lootMultiplier, 0f, 1f);
					if (Main.rand.NextFloat() < rollChance) {
						orig(self, closestPlayer);
					}
				}
			}
		};
	}

	public void Unload() { }
}
