using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using MonoMod.Cil;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.Localization;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace EliteEnemies.Common;

public enum WorldEliteAbundancy
{
	Scarce, Regular, Plentiful
}

public class Test : ModPlayer
{
	public override void UpdateEquips() {
		Main.NewText(WorldEliteAbundancySystem.WorldEliteAbundancy);
	}
}

public class WorldEliteAbundancySystem : ModSystem
{
	private static GroupOptionButton<WorldEliteAbundancy>[] _eliteOptionsButtons = [];

	public static WorldEliteAbundancy WorldEliteAbundancy = WorldEliteAbundancy.Regular;

	// Saving to header so we can generate a world without having to immediately open it
	//   for abundancy to save properly
	public override void SaveWorldHeader(TagCompound tag) {
		Mod.Logger.Info(WorldEliteAbundancy);
		tag["WorldEliteAbundancy"] = (byte)WorldEliteAbundancy;
	}

	public override void OnWorldLoad() {
		if (Main.ActiveWorldFileData.TryGetHeaderData<WorldEliteAbundancySystem>(out TagCompound data)) {
			if (data.ContainsKey("WorldEliteAbundancy")) {
				WorldEliteAbundancy = (WorldEliteAbundancy)data.GetByte("WorldEliteAbundancy");
			}
			else {
				WorldEliteAbundancy = WorldEliteAbundancy.Regular;
			}
		}
	}

	public override void SaveWorldData(TagCompound tag) {
		tag["WorldEliteAbundancy"] = (byte)WorldEliteAbundancy;
	}

	public override void LoadWorldData(TagCompound tag) {
		if (tag.ContainsKey("WorldEliteAbundancy")) {
			WorldEliteAbundancy = (WorldEliteAbundancy)tag.GetByte("WorldEliteAbundancy");
		}
	}

	public override void NetSend(BinaryWriter writer) {
		writer.Write((byte)WorldEliteAbundancy);
	}

	public override void NetReceive(BinaryReader reader) {
		WorldEliteAbundancy = (WorldEliteAbundancy)reader.ReadByte();
	}

	public override void Load() {
		IL_UIWorldCreation.BuildPage += ApplyWorldGenerationMenuHeightIncrease;
		IL_UIWorldCreation.MakeInfoMenu += InsertWorldGenerationEliteAbundancyButtons;
	}

	private static void ApplyWorldGenerationMenuHeightIncrease(ILContext il) {
		var cursor = new ILCursor(il);
		cursor.GotoNext(MoveType.Before, i => i.MatchStloc0());
		cursor.EmitLdcI4(48);
		cursor.EmitAdd();

		cursor.GotoNext(MoveType.After,
			i => i.MatchLdcR4(170f),
			i => i.MatchLdloc0(),
			i => i.MatchConvR4(),
			i => i.MatchSub());
		cursor.EmitLdcR4(48f);
		cursor.EmitAdd();
	}

	private static void InsertWorldGenerationEliteAbundancyButtons(ILContext il) {
		var cursor = new ILCursor(il);
		cursor.GotoNext(MoveType.Before,
			i => i.MatchLdarg0(),
			i => i.MatchLdloc0(),
			i => i.MatchLdloc1(),
			i => i.MatchLdarg0(),
			i => i.MatchLdftn<UIWorldCreation>("ClickEvilOption"));
		cursor.EmitLdarg0();
		cursor.EmitLdloc0();
		cursor.EmitLdloca(1);
		cursor.EmitLdloc(10);
		cursor.EmitDelegate((UIWorldCreation uiState, UIElement container, ref float accumulatedHeight, float usableWidthPercent) => {
			static LocalizedText GetLang(string suffix) {
				return ModContent.GetInstance<EliteEnemies>().GetLocalization(suffix);
			}

			WorldEliteAbundancy[] elites = [
				WorldEliteAbundancy.Scarce,
				WorldEliteAbundancy.Regular,
				WorldEliteAbundancy.Plentiful,
			];

			LocalizedText[] titles = [
				GetLang("WorldGen.Titles.Scarce"),
				GetLang("WorldGen.Titles.Regular"),
				GetLang("WorldGen.Titles.Plentiful"),
			];

			LocalizedText[] descriptions = [
				GetLang("WorldGen.Descriptions.Scarce"),
				GetLang("WorldGen.Descriptions.Regular"),
				GetLang("WorldGen.Descriptions.Plentiful"),
			];

			Color[] colors = [
				Color.Pink,
				Color.LimeGreen,
				Color.CornflowerBlue,
			];

			string[] iconPaths = [
				$"{nameof(EliteEnemies)}/Assets/Textures/EliteIconScarce",
				$"{nameof(EliteEnemies)}/Assets/Textures/EliteIconRegular",
				$"{nameof(EliteEnemies)}/Assets/Textures/EliteIconPlentiful",
			];

			List<GroupOptionButton<WorldEliteAbundancy>> groupOptionButtons = [];

			for (int i = 0; i < elites.Length; i++) {
				GroupOptionButton<WorldEliteAbundancy> button = new(elites[i], titles[i], descriptions[i], colors[i], null, titleAlignmentX: 1f) {
					Width = StyleDimension.FromPixelsAndPercent(-4 * (elites.Length - 1), 1f / (elites.Length * usableWidthPercent)),
					Left = StyleDimension.FromPercent(1f - usableWidthPercent),
					Top = StyleDimension.FromPixels(accumulatedHeight),
					HAlign = i / (float)(elites.Length - 1f)
				};

				typeof(GroupOptionButton<WorldEliteAbundancy>)
					.GetField("_iconTexture", ReflectionHelpers.AllFlags)
					.SetValue(button, ModContent.Request<Texture2D>(iconPaths[i]));

				button.OnLeftMouseDown += (evt, listeningElement) => {
					var listeningButton = (GroupOptionButton<WorldEliteAbundancy>)listeningElement;
					WorldEliteAbundancy = listeningButton.OptionValue;
					foreach (var button in _eliteOptionsButtons) {
						ModContent.GetInstance<EliteEnemies>().Logger.Info(button.OptionValue);
						button.SetCurrentOption(WorldEliteAbundancy);
					}
				};

				button.OnMouseOver += (evt, listeningElement) => {
					var listeningButton = (GroupOptionButton<WorldEliteAbundancy>)listeningElement;
					var description = listeningButton.Description;
					GetSetDescriptionText(uiState).SetText(description);
				};

				button.OnMouseOut += uiState.ClearOptionDescription;
				button.SetCurrentOption(WorldEliteAbundancy);
				groupOptionButtons.Add(button);
				container.Append(button);
			}

			_eliteOptionsButtons = groupOptionButtons.ToArray();
			accumulatedHeight += 48f;
			AddHorizontalSeparator(uiState, container, accumulatedHeight);
		});
	}

	[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_descriptionText")]
	private static extern ref UIText GetSetDescriptionText(UIWorldCreation obj);

	[UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "AddHorizontalSeparator")]
	private static extern void AddHorizontalSeparator(UIWorldCreation obj, UIElement container, float accumulatedHeight);
}

