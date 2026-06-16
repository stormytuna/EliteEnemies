using EliteEnemies.Common;
using FishUtils.DataStructures;

namespace EliteEnemies.Content.EliteVariations;

public class ScramblingElite : EliteVariation
{
	public override EliteVariationRarity Rarity {
		get => EliteVariationRarity.SuperRare;
	}

    public override bool CanApply(NPC npc) {
        return ServerConfig.Instance.EnableScrambling;
    }

	public override void OnApply(NPC npc) {
		NPCRenderRedirectSystem.RegisterRenderAction(npc, (int)RenderPriority.First + 50, static (npc, renderTarget, spriteBatch) => {
			spriteBatch.TakeSnapshotAndEnd(out var sbParams);

			var glitchEffect = Assets.Shaders.Glitch.Value;
			ModContent.GetInstance<EliteEnemies>().Logger.Debug(glitchEffect is null);
			glitchEffect.Parameters["intensity"].SetValue(0.1f);
			glitchEffect.Parameters["textureSize"].SetValue(renderTarget.Size());
			glitchEffect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly);

			Main.graphics.GraphicsDevice.Textures[1] = Assets.Textures.Noise01.Value;

			spriteBatch.Begin(sbParams with { Effect = glitchEffect });

			spriteBatch.Draw(renderTarget, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);

			spriteBatch.Restart(sbParams);
		});
	}
}

public class ScramblingPlayer : ModPlayer
{
	public struct ScrambledStats
	{
		public int MaxLife;
		public int Life;
		public int MaxMana;
		public int Mana;
	}

	public bool Scrambled = false;

	private static ScrambledStats _scrambledStats = new ScrambledStats();

	public override void ResetEffects() {
		Scrambled = false;
	}

	public override void Load() {
		On_Main.GUIBarsDrawInner += static (orig, self) => {
			bool scramble = Main.LocalPlayer.GetModPlayer<ScramblingPlayer>().Scrambled;

			int oldStatLifeMax = Main.LocalPlayer.statLifeMax2;
			int oldStatLife = Main.LocalPlayer.statLife;
			int oldStatManaMax = Main.LocalPlayer.statManaMax2;
			int oldStatMana = Main.LocalPlayer.statMana;

			if (scramble) {
				// Scrambling every frame looks terrible
				if (double.Floor(Main.timeForVisualEffects) % 10 == 0) {
					int maxLife = Main.rand.Next(500);
					int maxMana = Main.rand.Next(400);
					_scrambledStats = new ScrambledStats {
						MaxLife = maxLife,
						Life = Main.rand.Next(maxLife),
						MaxMana = maxMana,
						Mana = Main.rand.Next(maxMana),
					};
				}

				Main.LocalPlayer.statLifeMax2 = _scrambledStats.MaxLife;
				Main.LocalPlayer.statLife = _scrambledStats.Life;
				Main.LocalPlayer.statManaMax2 = _scrambledStats.MaxMana;
				Main.LocalPlayer.statMana = _scrambledStats.Mana;
			}

			orig(self);

			if (scramble) {
				Main.LocalPlayer.statLifeMax2 = oldStatLifeMax;
				Main.LocalPlayer.statLife = oldStatLife;
				Main.LocalPlayer.statManaMax2 = oldStatManaMax;
				Main.LocalPlayer.statMana = oldStatMana;
			}
		};
	}

	public override void PreUpdateBuffs() {
		foreach (var npc in Main.ActiveNPCs) {
			if (npc.GetGlobalNPC<ScramblingElite>().ApplyEliteVariation && npc.WithinRange(Player.Center, 45 * 16)) {
				Player.AddBuff(ModContent.BuffType<ScrambledBuff>(), 2);
				return;
			}
		}
	}
}

public class ScrambledBuff : ModBuff
{
	public override void SetStaticDefaults() {
		Main.debuff[Type] = true;
	}

	public override void Update(Player player, ref int buffIndex) {
		player.GetModPlayer<ScramblingPlayer>().Scrambled = true;
	}
}
