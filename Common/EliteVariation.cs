using System.Collections.Generic;
using System.IO;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace EliteEnemies.Common;

public abstract class EliteVariation : GlobalNPC
{
	// TODO: Secret seed fuckery!
	private static Dictionary<EliteVariationRarity, float> _rarityToValueMult = new() {
		{EliteVariationRarity.Common, 1.5f},
		{EliteVariationRarity.Uncommon, 2f},
		{EliteVariationRarity.Rare, 3.5f},
		{EliteVariationRarity.SuperRare, 5f},
		{EliteVariationRarity.Legendary, 8f},
	};

	private bool _firstFrame = true;

	public new LocalizedText Name {
		get => Language.GetOrRegister($"Mods.{nameof(EliteEnemies)}.EliteVariations.{GetType().Name}");
	}

	public sealed override void Load() {
		SafeLoad();
		_ = Name; // GetOrRegister will only register if we call it during mod load
	}

	/// <summary>
	/// Whether to apply the elite variation.
	/// Defaults to false.
	/// </summary>
	public bool ApplyEliteVariation = false;

	/// <summary>
	/// The rarity of this elite, influences spawn chance, value multiplier and loot multiplier.
	/// Defaults to Common
	/// </summary>
	public virtual EliteVariationRarity Rarity {
		get => EliteVariationRarity.Common;
	}

	public virtual bool CanApply(NPC npc) {
		return true;
	}

	public virtual void SafeLoad() { }

	public virtual bool SafePreAI(NPC npc) {
		return true;
	}

	public virtual void SafeSendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter) { }

	public virtual void SafeReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader) { }

	public virtual void SafeSaveData(NPC npc, TagCompound tag) { }
	public virtual void SafeLoadData(NPC npc, TagCompound tag) { }


	/// <summary>
	/// Called just before the first frame of the NPCs AI. Use this method to apply deterministic changes to an NPC's default fields. For non-deterministic changes, use SafeOnSpawn
	/// </summary>
	public virtual void OnApply(NPC npc) { }

	public sealed override bool InstancePerEntity {
		get => true;
	}

	public sealed override bool PreAI(NPC npc) {
		bool ret = SafePreAI(npc);

		if (_firstFrame && ApplyEliteVariation) {
			_firstFrame = false;

			OnApply(npc);
			npc.value = (int)(npc.value * _rarityToValueMult[Rarity]);
		}

		return ret;
	}

	public sealed override void ModifyTypeName(NPC npc, ref string typeName) {
		if (!ApplyEliteVariation) {
			return;
		}

		typeName = $"{Name} {typeName}";
	}

	public sealed override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter) {
		SafeSendExtraAI(npc, bitWriter, binaryWriter);
		bitWriter.WriteBit(ApplyEliteVariation);
	}

	public sealed override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader) {
		SafeReceiveExtraAI(npc, bitReader, binaryReader);
		ApplyEliteVariation = bitReader.ReadBit();
	}

	public sealed override void SaveData(NPC npc, TagCompound tag) {
		SafeSaveData(npc, tag);
		tag["applyEliteVariation"] = ApplyEliteVariation;
	}

	public sealed override void LoadData(NPC npc, TagCompound tag) {
		SafeLoadData(npc, tag);
		ApplyEliteVariation = tag.GetBool("applyEliteVariation");
	}
}

public enum EliteVariationRarity : byte
{
	Common, Uncommon, Rare, SuperRare, Legendary
}
