using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace EliteEnemies.Common;

public abstract class EliteVariation : GlobalNPC
{
	private bool _firstFrame = true;

	public new LocalizedText Name => Language.GetOrRegister($"Mods.{nameof(EliteEnemies)}.EliteVariations.{GetType().Name}");

	/// <summary>
	/// Whether to apply the elite variation.
	/// Defaults to false.
	/// </summary>
	public bool ApplyEliteVariation { get; private set; } = false;

	/// <summary>
	/// The chance of this variation appearing, as a decimal
	/// Defaults to 0.05f
	/// </summary>
	public virtual float SpawnChance => 0.05f;

	/// <summary>
	/// Multiplier to coins dropped.
	/// Defaults to 1f
	/// </summary>
	public virtual float ValueMultiplier => 1f;

	/// <summary>
	/// Multiplier to loot drops, 1f would be standard amount, 2f would drop twice as much loot, 1.5f would drop twice as much loot half of the time.
	/// Defaults to 1f
	/// </summary>
	public virtual float LootMultiplier => 1f;

	/// <summary>
	/// Whether to allow an enemy with this variation to have more variations
	/// Defaults to true
	/// </summary>
	public virtual bool AllowOtherEliteVariations => true;

	public virtual bool CanApply(NPC npc) => true;

	public virtual void OnApply(NPC npc) { }

	public sealed override bool InstancePerEntity => true;

	public sealed override bool AppliesToEntity(NPC entity, bool lateInstantiation) {
		return /* entity.CanBeChasedBy() && */ !lateInstantiation;
	}

	public override void OnSpawn(NPC npc, IEntitySource source) {
		ApplyEliteVariation = CanApply(npc) && Main.rand.NextFloat() < SpawnChance;
	}

	public override bool PreAI(NPC npc) {
		if (_firstFrame && ApplyEliteVariation) {
			_firstFrame = false;

			OnApply(npc);
			npc.value = (int)(npc.value * ValueMultiplier);
		}

		return base.PreAI(npc);
	}

	public override void ModifyTypeName(NPC npc, ref string typeName) {
		if (!ApplyEliteVariation) {
			return;
		}

		typeName = $"{Name} {typeName}";
	}

	public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter) {
		bitWriter.WriteBit(ApplyEliteVariation);
	}

	public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader) {
		ApplyEliteVariation = bitReader.ReadBit();
	}
}
