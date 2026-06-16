using System;
using System.Collections.Generic;
using System.IO;
using EliteEnemies.Common;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Localization;

namespace EliteEnemies.Content.UI;

public class EliteAbundancySwitcher : ModItem
{
	private WorldEliteAbundancy _abundancy = WorldEliteAbundancy.Regular;

	public static LocalizedText AbundancyTooltip { get; private set; }
	public static LocalizedText ScarceDescription { get; private set; }
	public static LocalizedText RegularDescription { get; private set; }
	public static LocalizedText PlentifulDescription { get; private set; }
	public static LocalizedText CeaselessDescription { get; private set; }
	public static LocalizedText Announcement { get; private set; }

    public override void SetStaticDefaults() {
		AbundancyTooltip = this.GetLocalization("AbundancyTooltip");
		ScarceDescription = this.GetLocalization("ScarceTooltip");
		RegularDescription = this.GetLocalization("RegularTooltip");
		PlentifulDescription = this.GetLocalization("PlentifulTooltip");
		CeaselessDescription = this.GetLocalization("CeaselessTooltip");
		Announcement = this.GetLocalization("Announcement");
    }

    public override void SetDefaults()
    {
		Item.width = Item.height = 18;
		Item.DefaultToThrownWeapon(ModContent.ProjectileType<EliteAbundancySwitcherProjectile>(), 20, 8f);
		Item.UseSound = SoundID.Item106;
		Item.SetShopValues(Terraria.Enums.ItemRarityColor.Green2, Item.buyPrice(gold: 3));
    }

    public override bool CanRightClick() {
		return true;
    }

    public override void RightClick(Player player) {
        _abundancy = _abundancy + 1;
		if (!Enum.IsDefined(_abundancy)) {
			_abundancy = WorldEliteAbundancy.Scarce;
		}

		Item.stack++;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
		var proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback);
		if (proj.ModProjectile is EliteAbundancySwitcherProjectile modProj) {
			modProj.Abundancy = _abundancy;
		}

		return false;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips) {
		Color abundancyColor = WorldEliteAbundancySystem.GetColorForAbundancy(_abundancy);
		string abundancyName = Enum.GetName(_abundancy);
		string abundancyColored = abundancyName.ApplyColor(abundancyColor.WithMouseTextPulsing());

		tooltips.Add(new TooltipLine(Mod, "abundancytooltip", AbundancyTooltip.Format(abundancyColored)));

		LocalizedText abundancyTooltip = _abundancy switch {
			WorldEliteAbundancy.Scarce => ScarceDescription,
			WorldEliteAbundancy.Regular => RegularDescription,
			WorldEliteAbundancy.Plentiful => PlentifulDescription,
			WorldEliteAbundancy.Ceaseless => CeaselessDescription,
			_ => throw new Exception("how are you seeing this, that's scary..."),
		};

		string[] lines = abundancyTooltip.Value.Split('\n');
		foreach (var line in lines) {
			tooltips.Add(new TooltipLine(Mod, "abundancytooltip", line));
		}
    }
}

public class EliteAbundancySwitcherGlobalNPC : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation) {
        return entity.type == NPCID.BestiaryGirl;
    }

    public override void ModifyShop(NPCShop shop) {
		shop.InsertAfter(ItemID.TreeGlobe, ModContent.ItemType<EliteAbundancySwitcher>());
    }
}

public class EliteAbundancySwitcherProjectile : ModProjectile
{
	public WorldEliteAbundancy Abundancy = WorldEliteAbundancy.Regular;

    public override string Texture => $"{nameof(EliteEnemies)}/Content/UI/{nameof(EliteAbundancySwitcher)}";

    public override void SetDefaults() {
		Projectile.width = 18;
		Projectile.height = 18;
		Projectile.aiStyle = ProjAIStyleID.ThrownProjectile;
		Projectile.friendly = true;
		Projectile.penetrate = 1;
    }

    public override void OnKill(int timeLeft) {
		SoundEngine.PlaySound(SoundID.Item107, Projectile.Center);

		for (int i = 0; i < 15; i++) {
			Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Glass, 0f, -2f, 0, default, 1.5f);
		}

		if (Main.netMode == NetmodeID.SinglePlayer) {
			WorldEliteAbundancySystem.Abundancy = Abundancy;
		}

		// Cheating slightly
		// You can't send coloured text via NetworkText, so printing for MP clients regardless of if server actually changed it successfully
		// Shouldn't have any meaningful gameplay difference
		if (Main.netMode != NetmodeID.Server) {
			Color abundancyColor = WorldEliteAbundancySystem.GetColorForAbundancy(Abundancy);
			string abundancyName = Enum.GetName(Abundancy);
			string abundancyColored = abundancyName.ApplyColor(abundancyColor.WithMouseTextPulsing());

			Main.NewText(EliteAbundancySwitcher.Announcement.Format(abundancyColored));
		}

		if (Main.netMode == NetmodeID.Server) {
			WorldEliteAbundancySystem.Abundancy = Abundancy;
			NetMessage.SendData(MessageID.WorldData);
		}
    }

    public override void SendExtraAI(BinaryWriter writer) {
		writer.Write7BitEncodedInt((int)Abundancy);
    }

    public override void ReceiveExtraAI(BinaryReader reader) {
		Abundancy = (WorldEliteAbundancy)reader.Read7BitEncodedInt();
    }
}

/*
public class BigBookOfElitesItem : ModItem
{
	public override void SetDefaults() {
		Item.width = 20;
		Item.height = 20;
		Item.useStyle = ItemUseStyleID.HoldUp;
		Item.useAnimation = Item.useTime = 15;
	}

	public override bool? UseItem(Player player) {
		if (Main.myPlayer == player.whoAmI) {
			BigBookOfElites.Toggle();
		}

		return true;
	}
}

[Autoload(Side = ModSide.Client)]
public class BigBookOfElites : ModSystem
{
	private static UserInterface _interface;
	private static BigBookOfElitesUIState _uiState;
	private static GameTime _oldGameTime;

	public override void Load() {
		_interface = new UserInterface();
		_uiState = new BigBookOfElitesUIState();
		_uiState.Activate();
	}

	public static void Show() {
		_interface.SetState(_uiState);
	}

	public static void Hide() {
		_interface.SetState(null);
	}

	public static void Toggle() {
		if (_interface.CurrentState is null) {
			Show();
		}
		else {
			Hide();
		}
	}

	public override void UpdateUI(GameTime gameTime) {
		_oldGameTime = gameTime;
		if (_interface?.CurrentState is not null) {
			_interface.Update(gameTime);
		}
	}

	public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
		int index = layers.FindIndex(x => x.Name == "Vanilla: Inventory");
		if (index == -1) {
			Mod.Logger.Error("Couldn't find the vanilla inventory, tf?");
			return;
		}

		layers.Insert(index, new LegacyGameInterfaceLayer(
			$"{nameof(EliteEnemies)}:{nameof(BigBookOfElites)}",
			() => {
				if (_oldGameTime is not null && _interface.CurrentState is not null) {
					_interface.Draw(Main.spriteBatch, _oldGameTime);
				}
				return true;
			},
			InterfaceScaleType.UI
		));
	}
}

public class BigBookOfElitesUIState : UIState
{
	public static class Theming
	{
		public static Color Title => Color.Violet;
		public static Color Info => Color.CornflowerBlue;
		public static Color Settings => Color.SlateGray;

		public static Color ToHoverColor(Color color) {
			Vector3 hsl = Main.rgbToHsl(color);
			hsl.Y += 0.2f;
			hsl.Z += 0.1f;
			return Main.hslToRgb(hsl);
		}

		public static Color ToClickColor(Color color) {
			Vector3 hsl = Main.rgbToHsl(color);
			hsl.Y += 0.2f;
			hsl.Z -= 0.1f;
			return Main.hslToRgb(hsl);
		}
	}

	static LocalizedText GetLang(string suffix) {
		return ModContent.GetInstance<EliteEnemies>().GetLocalization(suffix);
	}


	NineSliceUIPanel _mainPanel;
	NineSliceUIPanel _titlePanel;
	UIText _titleText;
	NineSliceUIPanel _infoButton;
	UIImage _infoIcon;
	NineSliceUIPanel _settingsButton;
	UIImage _settingsIcon;

	UIList _eliteVariantsList;
	NineSliceUIPanel _eliteVariationsListPanel;
	UIScrollbar _eliteVariantsListScrollbar;

	NineSliceUIPanel _eliteInfoPanel;

	NineSliceUIPanel _infoPanel;

	NineSliceUIPanel _settingsPanel;
	UIText _changeEliteAbundancyName;
	UIText _changeEliteAbundancyDescription;
	NineSliceUIPanel _changeEliteAbundancyOptionsPanel;
	NineSliceUIPanel _changeEliteAbundancyLeftArrow;
	UIImage _changeEliteAbundancyLeftArrowIcon;
	NineSliceUIPanel _changeEliteAbundancyRightArrow;
	UIImage _changeEliteAbundancyRightArrowIcon;

	public override void OnInitialize() {
		_mainPanel = new NineSliceUIPanel(Assets.Textures.NineSliceOutsetBasic, Color.Transparent);
		_mainPanel.Width = StyleDimension.FromPixels(400f);
		_mainPanel.Height = StyleDimension.FromPixels(600f);
		_mainPanel.HAlign = _mainPanel.VAlign = 0.3f;
		Append(_mainPanel);

		_eliteInfoPanel = new NineSliceUIPanel(Assets.Textures.NineSliceOutsetBasic, Color.Blue);

		_eliteVariationsListPanel = new NineSliceUIPanel(Assets.Textures.NineSliceOutsetBasic, Theming.Title);
		_eliteVariationsListPanel.Width = StyleDimension.FromPixelsAndPercent(-10, 1f);
		_eliteVariationsListPanel.Height = StyleDimension.Fill;
		_eliteVariationsListPanel.SetPadding(8f);
		_eliteVariationsListPanel.HAlign = 0.5f;
		_mainPanel.Append(_eliteVariationsListPanel);

		_infoPanel = new NineSliceUIPanel(Assets.Textures.NineSliceOutsetBasic, Theming.Info);
		_infoPanel.Width = StyleDimension.FromPixelsAndPercent(-10, 1f);
		_infoPanel.Height = StyleDimension.Fill;
		_infoPanel.SetPadding(8f);
		_infoPanel.HAlign = 0.5f;
		_infoPanel.Hide();
		_mainPanel.Append(_infoPanel);

		_settingsPanel = new NineSliceUIPanel(Assets.Textures.NineSliceOutsetBasic, Theming.Settings);
		_settingsPanel.Width = StyleDimension.FromPixelsAndPercent(-10, 1f);
		_settingsPanel.Height = StyleDimension.Fill;
		_settingsPanel.SetPadding(8f);
		_settingsPanel.HAlign = 0.5f;
		_settingsPanel.Hide();
		_mainPanel.Append(_settingsPanel);

		_titlePanel = new NineSliceUIPanel(Assets.Textures.NineSliceOutsetBasicJoinRight, Theming.Title, Theming.ToHoverColor(Theming.Title), Theming.ToClickColor(Theming.Title));
		_titlePanel.Width = StyleDimension.FromPixelsAndPercent(-2 * 50f, 1f);
		_titlePanel.Height = StyleDimension.FromPixels(50f);
		_titlePanel.OnLeftClick += (evt, ele) => {
			_settingsPanel.Hide();
			_infoPanel.Hide();
			_eliteVariationsListPanel.Show();
		};
		_mainPanel.Append(_titlePanel);

		// TODO: Localisation
		_titleText = new UIText("Elites");
		_titleText.Width = _titleText.Height = StyleDimension.Fill;
		_titleText.TextOriginX = 0.1f;
		_titleText.TextOriginY = 0.5f;
		_titlePanel.Append(_titleText);

		_infoButton = new NineSliceUIPanel(Assets.Textures.NineSliceOutsetBasicJoinLeftRight, Theming.Info, Theming.ToHoverColor(Theming.Info), Theming.ToClickColor(Theming.Info));
		_infoButton.Width = _infoButton.Height = StyleDimension.FromPixels(50f);
		_infoButton.Left = StyleDimension.FromPixelsAndPercent(-100f, 1f);
		_infoButton.OnLeftClick += (evt, ele) => {
			_eliteVariationsListPanel.Hide();
			_settingsPanel.Hide();
			_infoPanel.Show();
		};
		_mainPanel.Append(_infoButton);

		_infoIcon = new UIImage(Assets.Textures.InfoIcon);
		_infoIcon.HAlign = _infoIcon.VAlign = 0.5f;
		_infoButton.Append(_infoIcon);

		_settingsButton = new NineSliceUIPanel(Assets.Textures.NineSliceOutsetBasicJoinLeft, Theming.Settings, Theming.ToHoverColor(Theming.Settings), Theming.ToClickColor(Theming.Settings));
		_settingsButton.Width = _settingsButton.Height = StyleDimension.FromPixels(50f);
		_settingsButton.Left = StyleDimension.FromPixelsAndPercent(-50f, 1f);
		_settingsButton.OnLeftClick += (evt, ele) => {
			_infoPanel.Hide();
			_eliteVariationsListPanel.Hide();
			_settingsPanel.Show();
		};
		_mainPanel.Append(_settingsButton);

		_settingsIcon = new UIImage(Assets.Textures.SettingsIcon);
		_settingsIcon.HAlign = _settingsIcon.VAlign = 0.5f;
		_settingsButton.Append(_settingsIcon);

		_eliteVariantsList = new UIList {
			Width = StyleDimension.FromPixelsAndPercent(-20f, 1f),
			Height = StyleDimension.FromPixelsAndPercent(-50f, 1f),
			Top = StyleDimension.FromPixels(50f),
			ListPadding = 2f
			// TODO: Custom sorting, sort by rarity, then name
		};
		_eliteVariationsListPanel.Append(_eliteVariantsList);

		// TODO: Make custom scrollbar that looks actually nice
		_eliteVariantsListScrollbar = new UIScrollbar() {
			Width = StyleDimension.FromPixels(20f),
			Height = StyleDimension.FromPixelsAndPercent(-50f, 1f),
			Top = StyleDimension.FromPixels(50f),
			Left = StyleDimension.FromPixelsAndPercent(-10f, 1f),
		};
		_eliteVariationsListPanel.Append(_eliteVariantsListScrollbar);
		_eliteVariantsList.SetScrollbar(_eliteVariantsListScrollbar);

		foreach (EliteVariation elite in ModContent.GetContent<EliteVariation>()) {
			_eliteVariantsList.Add(new EliteDetailButtonUI(elite));
		}

		// TODO: Localisation
		_changeEliteAbundancyName = new UIText("Abundancy:");
		_changeEliteAbundancyName.Width = StyleDimension.Fill;
		_changeEliteAbundancyName.Height = StyleDimension.FromPixels(50f);
		_changeEliteAbundancyName.Top = StyleDimension.FromPixels(50f);
		_changeEliteAbundancyName.TextOriginX = 0.1f;
		_changeEliteAbundancyName.TextOriginY = 0.5f;
		_settingsPanel.Append(_changeEliteAbundancyName);

		_changeEliteAbundancyOptionsPanel = new NineSliceUIPanel(Assets.Textures.NineSliceOutsetBasic, Color.Red);
		_changeEliteAbundancyOptionsPanel.Width = StyleDimension.FromPercent(0.8f);
		_changeEliteAbundancyOptionsPanel.Height = StyleDimension.FromPixels(50f);
		_changeEliteAbundancyOptionsPanel.Top = StyleDimension.FromPixels(100f);
		_changeEliteAbundancyOptionsPanel.HAlign = 0.5f;
		_settingsPanel.Append(_changeEliteAbundancyOptionsPanel);

		// TODO: Finish!
		_changeEliteAbundancyLeftArrow = new NineSliceUIPanel(Assets.Textures.NineSliceOutsetBasic, Color.Red);
		_changeEliteAbundancyLeftArrow.Width = _changeEliteAbundancyLeftArrow.Height = StyleDimension.FromPixels(50f);
		_changeEliteAbundancyLeftArrow.OnLeftClick += (evt, ele) => {
			ChangeEliteAbundancy((int)WorldEliteAbundancySystem.Abundancy - 1);
		};
		_changeEliteAbundancyOptionsPanel.Append(_changeEliteAbundancyLeftArrow);

		_changeEliteAbundancyLeftArrowIcon = new UIImage(Assets.Textures.LeftArrowIcon);
		_changeEliteAbundancyLeftArrowIcon.HAlign = _changeEliteAbundancyLeftArrowIcon.VAlign = 0.5f;
		_changeEliteAbundancyLeftArrow.Append(_changeEliteAbundancyLeftArrowIcon);

		_changeEliteAbundancyRightArrow = new NineSliceUIPanel(Assets.Textures.NineSliceOutsetBasic, Color.Red);
		_changeEliteAbundancyRightArrow.Width = _changeEliteAbundancyRightArrow.Height = StyleDimension.FromPixels(50f);
		_changeEliteAbundancyRightArrow.Left = StyleDimension.FromPixelsAndPercent(-50f, 1f);
		_changeEliteAbundancyRightArrow.OnLeftClick += (evt, ele) => {
			ChangeEliteAbundancy((int)WorldEliteAbundancySystem.Abundancy + 1);
		};
		_changeEliteAbundancyOptionsPanel.Append(_changeEliteAbundancyRightArrow);

		_changeEliteAbundancyRightArrowIcon = new UIImage(Assets.Textures.RightArrowIcon);
		_changeEliteAbundancyRightArrowIcon.HAlign = _changeEliteAbundancyRightArrowIcon.VAlign = 0.5f;
		_changeEliteAbundancyRightArrow.Append(_changeEliteAbundancyRightArrowIcon);

		_changeEliteAbundancyName = new UIText("");
		_changeEliteAbundancyName.Width = StyleDimension.FromPixelsAndPercent(-100f, 1f);
		_changeEliteAbundancyName.Height = StyleDimension.Fill;
		_changeEliteAbundancyName.Left = StyleDimension.FromPixels(50f);
		_changeEliteAbundancyName.TextOriginX = _changeEliteAbundancyName.TextOriginY = 0.5f;
		_changeEliteAbundancyOptionsPanel.Append(_changeEliteAbundancyName);

		_changeEliteAbundancyDescription = new UIText("");
		_changeEliteAbundancyDescription.Width = StyleDimension.FromPixelsAndPercent(0, 1f);
		_changeEliteAbundancyDescription.Height = StyleDimension.FromPixelsAndPercent(-100f, 1f);
		_changeEliteAbundancyDescription.Top = StyleDimension.FromPixels(100f);
		_changeEliteAbundancyDescription.TextOriginX = _changeEliteAbundancyDescription.TextOriginY = 0f;
		_changeEliteAbundancyDescription.IsWrapped = true;
		_changeEliteAbundancyOptionsPanel.Append(_changeEliteAbundancyDescription);
	}

	private void ChangeEliteAbundancy(int newAbundancy) {
		if (Main.netMode == NetmodeID.MultiplayerClient && !Main.countsAsHostForGameplay[Main.myPlayer]) {
			// TODO: localisation
			Main.NewText("Only the host can change the Elite Abundancy!");
			return;
		}

		if (newAbundancy == Enum.GetNames<WorldEliteAbundancy>().Length) {
			newAbundancy = 0;
		}
		else if (newAbundancy < 0) {
			newAbundancy = Enum.GetNames<WorldEliteAbundancy>().Length - 1;
		}

		WorldEliteAbundancySystem.Abundancy = (WorldEliteAbundancy)newAbundancy;
		UpdateChosenEliteAbundancyText((WorldEliteAbundancy)newAbundancy);

		// TODO: Message in chat, and sync changes with other clients
	}

	private void UpdateChosenEliteAbundancyText(WorldEliteAbundancy abundancy) {
		LocalizedText abundancyName = GetLang($"WorldGen.Titles.{abundancy}");
		LocalizedText abundancyDescription = GetLang($"WorldGen.DescriptionsLong.{abundancy}");
		_changeEliteAbundancyName.SetText(abundancyName);
		_changeEliteAbundancyDescription.SetText(abundancyDescription);
	}

	public override void OnActivate() {
		base.OnActivate();

		UpdateChosenEliteAbundancyText(WorldEliteAbundancySystem.Abundancy);
	}

	protected override void DrawSelf(SpriteBatch spriteBatch) {
		base.DrawSelf(spriteBatch);

		if (_mainPanel.ContainsPoint(Main.MouseScreen)) {
			Main.LocalPlayer.mouseInterface = true;
		}
	}

	public class ChangeEliteAbundancyCommand : ModCommand
	{
		public override string Command {
			get => "abundancy";
		}

		public override CommandType Type {
			get => CommandType.World;
		}

		public override void Action(CommandCaller caller, string input, string[] args) {
			// TODO: Implement
		}
	}
}

public class EliteDetailButtonUI : UIElement
{
	readonly EliteVariation _elite;
	readonly NineSliceUIPanel _mainPanel;
	readonly UIText _nameText;
	readonly NineSliceUIPanel _rarityIcon;

	public EliteDetailButtonUI(EliteVariation elite) {
		_elite = elite;
		Width = StyleDimension.FromPixelsAndPercent(-50f, 1f);
		Height = StyleDimension.FromPixels(50f);

		_mainPanel = new NineSliceUIPanel(Assets.Textures.NineSliceOutsetBasicJoinRight, Color.Red);
		_mainPanel.Width = _mainPanel.Height = StyleDimension.Fill;
		Append(_mainPanel);

		_nameText = new UIText(_elite.Name, 0.8f);
		_nameText.Width = _nameText.Height = StyleDimension.Fill;
		_nameText.TextOriginX = 0.05f;
		_nameText.TextOriginY = 0.5f;
		_mainPanel.Append(_nameText);

		_rarityIcon = new NineSliceUIPanel(Assets.Textures.NineSliceOutsetBasicJoinLeft, Color.Pink);
		_rarityIcon.Width = _rarityIcon.Height = StyleDimension.FromPixels(50f);
		_rarityIcon.Left = StyleDimension.FromPercent(1f);
		_mainPanel.Append(_rarityIcon);
	}
}
*/
