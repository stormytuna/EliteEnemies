using System.Collections.Generic;
using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace EliteEnemies.Common;

public class ServerConfig : ModConfig
{
	public static ServerConfig Instance => ModContent.GetInstance<ServerConfig>();

	public override ConfigScope Mode => ConfigScope.ServerSide;

	[Header("Variations")]
	[DefaultValue(true)]
	public bool EnableBlazing { get; set; }

	[DefaultValue(true)]
	public bool EnableCursed { get; set; }

	[DefaultValue(true)]
	public bool EnableDiscordant { get; set; }

	[DefaultValue(true)]
	public bool EnableGlacial { get; set; }

	[DefaultValue(true)]
	public bool EnableGlowing { get; set; }

	[DefaultValue(true)]
	public bool EnableHuge { get; set; }

	[DefaultValue(true)]
	public bool EnableIchor { get; set; }

	[DefaultValue(true)]
	public bool EnableImmovable { get; set; }

	[DefaultValue(true)]
	public bool EnableInspiring { get; set; }

	[DefaultValue(true)]
	public bool EnableJacked { get; set; }

	[DefaultValue(true)]
	public bool EnableLucky { get; set; }

	[DefaultValue(true)]
	public bool EnableMedicated { get; set; }

	[DefaultValue(true)]
	public bool EnableMother { get; set; }

	[DefaultValue(true)]
	public bool EnablePhasic { get; set; }

	[DefaultValue(true)]
	public bool EnableSneaky { get; set; }


	[DefaultValue(true)]
	public bool EnableSpelunker { get; set; }

	[DefaultValue(true)]
	public bool EnableThief { get; set; }

	[DefaultValue(true)]
	public bool EnableTiny { get; set; }


	[DefaultValue(true)]
	public bool EnableVolatile { get; set; }

	[Header("SpawnChance")]
	[Range(0f, 1f)]
	[DefaultValue(0.1f)]
	public float CommonSpawnChance { get; set; }

	[Range(0f, 1f)]
	[DefaultValue(0.05f)]
	public float UncommonSpawnChance { get; set; }

	[Range(0f, 1f)]
	[DefaultValue(0.025f)]
	public float RareSpawnChance { get; set; }

	[Range(0f, 1f)]
	[DefaultValue(0.0125f)]
	public float SuperRareSpawnChance { get; set; }

	[Range(0f, 1f)]
	[DefaultValue(0.005f)]
	public float LegendarySpawnChance { get; set; }

	[Header("ValueMultiplier")]
	[Range(1f, 10f)]
	[DefaultValue(1.5f)]
	public float CommonValueMultiplier { get; set; }

	[Range(1f, 10f)]
	[DefaultValue(2f)]
	public float UncommonValueMultiplier { get; set; }

	[Range(1f, 10f)]
	[DefaultValue(3f)]
	public float RareValueMultiplier { get; set; }

	[Range(1f, 10f)]
	[DefaultValue(4f)]
	public float SuperRareValueMultiplier { get; set; }

	[Range(1f, 10f)]
	[DefaultValue(5f)]
	public float LegendaryValueMultiplier { get; set; }

	[Header("LootMultiplier")]
	[Range(1f, 10f)]
	[DefaultValue(1.2f)]
	public float CommonLootMultiplier { get; set; }

	[Range(1f, 10f)]
	[DefaultValue(1.4f)]
	public float UncommonLootMultiplier { get; set; }

	[Range(1f, 10f)]
	[DefaultValue(1.6f)]
	public float RareLootMultiplier { get; set; }

	[Range(1f, 10f)]
	[DefaultValue(1.8f)]
	public float SuperRareLootMultiplier { get; set; }

	[Range(1f, 10f)]
	[DefaultValue(2f)]
	public float LegendaryLootMultiplier { get; set; }

	[Header("Misc")]
	[DefaultValue(4)]
	[Range(0, 20)]
	public int MaxSimultaneousVariations { get; set; }

	public HashSet<NPCDefinition> NPCBlacklist { get; set; } = new();

	[DefaultValue(true)]
	public bool ApplyScaleChanges { get; set; }

	[DefaultValue(true)]
	public bool ApplyVelocityChanges { get; set; }

	[DefaultValue(true)]
	public bool ApplyToCritters { get; set; }

	[DefaultValue(false)]
	public bool ApplyToBosses { get; set; }

	[DefaultValue(true)]
	public bool ApplyToModdedNPCs { get; set; }
}
