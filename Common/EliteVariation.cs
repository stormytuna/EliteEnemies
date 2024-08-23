using System.IO;
using EliteEnemies.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
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
	/// The rarity of this elite, influences spawn chance, value multiplier and loot multiplier.
	/// Defaults to Common
	/// </summary>
	public virtual EliteVariationRarity Rarity => EliteVariationRarity.Common;

	/// <summary>
	/// The chance of this variation appearing, as a decimal.
	/// Influenced by Rarity by default.
	/// </summary>
	public virtual float SpawnChance => Rarity switch {
		EliteVariationRarity.Common => ServerConfig.Instance.CommonSpawnChance,
		EliteVariationRarity.Uncommon => ServerConfig.Instance.UncommonSpawnChance,
		EliteVariationRarity.Rare => ServerConfig.Instance.RareSpawnChance,
		EliteVariationRarity.SuperRare => ServerConfig.Instance.SuperRareSpawnChance,
		EliteVariationRarity.Legendary => ServerConfig.Instance.LegendarySpawnChance,
		_ => 0f,
	};

	/// <summary>
	/// Multiplier to coins dropped.
	/// Influenced by Rarity by default.
	/// </summary>
	public virtual float ValueMultiplier => Rarity switch {
		EliteVariationRarity.Common => ServerConfig.Instance.CommonValueMultiplier,
		EliteVariationRarity.Uncommon => ServerConfig.Instance.UncommonValueMultiplier,
		EliteVariationRarity.Rare => ServerConfig.Instance.RareValueMultiplier,
		EliteVariationRarity.SuperRare => ServerConfig.Instance.SuperRareValueMultiplier,
		EliteVariationRarity.Legendary => ServerConfig.Instance.LegendaryValueMultiplier,
		_ => 0f,
	};

	/// <summary>
	/// Multiplier to loot drops, 1f would be standard amount, 2f would drop twice as much loot, 1.5f would drop twice as much loot half of the time.
	/// Influenced by Rarity by default.
	/// </summary>
	public virtual float LootMultiplier => Rarity switch {
		EliteVariationRarity.Common => ServerConfig.Instance.CommonLootMultiplier,
		EliteVariationRarity.Uncommon => ServerConfig.Instance.UncommonLootMultiplier,
		EliteVariationRarity.Rare => ServerConfig.Instance.RareLootMultiplier,
		EliteVariationRarity.SuperRare => ServerConfig.Instance.SuperRareLootMultiplier,
		EliteVariationRarity.Legendary => ServerConfig.Instance.LegendaryLootMultiplier,
		_ => 0f,
	};

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
		bool careAboutCritter = ServerConfig.Instance.ApplyToCritters || !npc.CountsAsACritter;
		bool careAboutBoss = ServerConfig.Instance.ApplyToBosses || (!npc.boss && (npc.type is not NPCID.EaterofWorldsHead or NPCID.EaterofWorldsBody or NPCID.EaterofWorldsTail));
		bool underMaxVariationsLimit = npc.NumActiveEliteVariations() < ServerConfig.Instance.MaxSimultaneousVariations;
		ApplyEliteVariation = CanApply(npc) && Main.rand.NextFloat() < SpawnChance && careAboutCritter && careAboutBoss && underMaxVariationsLimit;
		if (ApplyEliteVariation) {
			Main.NewText(Name);
		}
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

public enum EliteVariationRarity : byte
{
	Common, Uncommon, Rare, SuperRare, UltraRare, Legendary
}
