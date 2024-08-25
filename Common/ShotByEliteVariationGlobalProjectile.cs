using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace EliteEnemies.Common;

public abstract class ShotByEliteVariationGlobalProjectile<TElite> : GlobalProjectile
	where TElite : EliteVariation
{
	protected bool ApplyEliteChanges { get; private set; }

	protected NPC Parent { get; private set; }

	public override bool InstancePerEntity => true;


	public virtual void SafeOnSpawn(Projectile projectile, IEntitySource source) { }

	public sealed override void OnSpawn(Projectile entity, IEntitySource source) {
		bool shotByEliteVariation = source is EntitySource_Parent { Entity: NPC npc } && npc.GetGlobalNPC<TElite>().ApplyEliteVariation;
		bool appliesToParent = source is EntitySource_Parent { Entity: Projectile projectile } && projectile.GetGlobalProjectile(this).ApplyEliteChanges;
		ApplyEliteChanges = shotByEliteVariation || appliesToParent;

		if (shotByEliteVariation) {
			Parent = (source as EntitySource_Parent).Entity as NPC;
		}
		else if (appliesToParent) {
			Parent = ((source as EntitySource_Parent).Entity as Projectile).GetGlobalProjectile(this).Parent;
		}

		SafeOnSpawn(entity, source);
	}

	public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter) {
		bitWriter.WriteBit(ApplyEliteChanges);
		binaryWriter.Write7BitEncodedInt(Parent.whoAmI);
	}

	public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader) {
		ApplyEliteChanges = bitReader.ReadBit();
		Parent = Main.npc[binaryReader.Read7BitEncodedInt()];
	}
}
