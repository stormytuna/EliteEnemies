using System.Collections.Generic;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace EliteEnemies.Common;

public class ServerConfig : ModConfig
{
	public static ServerConfig Instance {
		get => ModContent.GetInstance<ServerConfig>();
	}

	public override ConfigScope Mode {
		get => ConfigScope.ServerSide;
	}

	[Header("Misc")]
	public HashSet<NPCDefinition> NPCBlacklist { get; set; } = new();

	[DefaultValue(true)]
	public bool ApplyScaleChanges { get; set; }

	[DefaultValue(true)]
	public bool ApplyVelocityChanges { get; set; }

	[DefaultValue(true)]
	public bool ApplyToCritters { get; set; }

	[DefaultValue(false)]
	public bool ApplyToBosses { get; set; }

	[Header("Variations")]
	[DefaultValue(true)]
	public bool EnableAdaptive { get; set; }

	[DefaultValue(true)]
	public bool EnableArmored { get; set; }

	[DefaultValue(true)]
	public bool EnableBlazing { get; set; }

	[DefaultValue(true)]
	public bool EnableBrainy { get; set; }

	[DefaultValue(true)]
	public bool EnableCursed { get; set; }

	[DefaultValue(true)]
	public bool EnableDestroyer { get; set; }

	[DefaultValue(true)]
	public bool EnableDiscordant { get; set; }

	[DefaultValue(true)]
	public bool EnableElectric { get; set; }

	[DefaultValue(true)]
	public bool EnableFluxive { get; set; }

	[DefaultValue(true)]
	public bool EnableFruity { get; set; }

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
	public bool EnableLeeching { get; set; }

	[DefaultValue(true)]
	public bool EnableLucky { get; set; }

	[DefaultValue(true)]
	public bool EnableMedicated { get; set; }

	[DefaultValue(true)]
	public bool EnableMother { get; set; }

	[DefaultValue(true)]
	public bool EnableMolten { get; set; }

	[DefaultValue(true)]
	public bool EnableNyan { get; set; }

	[DefaultValue(true)]
	public bool EnableParty { get; set; }

	[DefaultValue(true)]
	public bool EnablePhasic { get; set; }

	[DefaultValue(true)]
	public bool EnablePiercing { get; set; }

	[DefaultValue(true)]
	public bool EnablePolite { get; set; }

	[DefaultValue(true)]
	public bool EnableScrambling { get; set; }

	[DefaultValue(true)]
	public bool EnableSneaky { get; set; }

	[DefaultValue(true)]
	public bool EnableSpelunker { get; set; }

	[DefaultValue(true)]
	public bool EnableSpiky { get; set; }

	[DefaultValue(true)]
	public bool EnableStoic { get; set; }

	[DefaultValue(true)]
	public bool EnableStinky { get; set; }

	[DefaultValue(true)]
	public bool EnableThief { get; set; }

	[DefaultValue(true)]
	public bool EnableTiny { get; set; }

	[DefaultValue(true)]
	public bool EnableUnstable { get; set; }

	[DefaultValue(true)]
	public bool EnableVolatile { get; set; }
}
