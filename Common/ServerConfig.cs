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
}
