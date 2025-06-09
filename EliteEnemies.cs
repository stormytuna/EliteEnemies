using System.IO;
using EliteEnemies.Content.EliteVariations;

namespace EliteEnemies;

public class EliteEnemies : Mod
{
	public enum MessageType : byte
	{
		SpawnLeechingEliteProjectile
	}

	public override void HandlePacket(BinaryReader reader, int whoAmI) {
		var messageType = (MessageType)reader.ReadByte();

		switch (messageType) {
			case MessageType.SpawnLeechingEliteProjectile:
				if (Main.netMode != NetmodeID.Server) {
					return;
				}
				
				var targetWhoAmI = reader.Read7BitEncodedInt();
				var position = reader.ReadVector2();
				LeechingElite.HandleSpawnLeechingEliteProjectile(targetWhoAmI, position);
				break;
		}
	}
}
