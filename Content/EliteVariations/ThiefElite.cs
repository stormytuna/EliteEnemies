using System.Collections.Generic;
using EliteEnemies.Common;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace EliteEnemies.Content.EliteVariations;

public class ThiefElite : EliteVariation
{
	private List<Item> _items = new();

	public override EliteVariationRarity Rarity => EliteVariationRarity.Common;

	public override bool CanApply(NPC npc) {
		return !NPCID.Sets.CantTakeLunchMoney[npc.type];
	}

	public override void PostAI(NPC npc) {
		if (!ApplyEliteVariation) {
			return;
		}

		foreach (var item in Main.ActiveItems) {
			if (npc.Hitbox.Intersects(item.Hitbox) && !item.beingGrabbed) {
				_items.Add(item.Clone());
				item.active = false;
				npc.extraValue += 1; // Easiest way of making little sparkles appear, don't care enough to write a workaround that doesn't also drop 1 extra copper
				npc.moneyPing(npc.position);

				CombatText.NewText(npc.Hitbox, Color.Red, Mod.GetLocalization("EliteVariations.ThiefElite.Stolen").WithFormatArgs(item.Name).Value, true);
			}
		}
	}

	public override void OnKill(NPC npc) {
		if (!ApplyEliteVariation) {
			return;
		}

		foreach (var item in _items) {
			Item.NewItem(npc.GetSource_Death(), npc.Hitbox, item);
		}
	}

	public override bool NeedSaving(NPC npc) {
		return _items.Count > 0 && ApplyEliteVariation;
	}

	public override void SaveData(NPC npc, TagCompound tag) {
		base.SaveData(npc, tag);

		if (_items.Count > 0 && ApplyEliteVariation) {
			tag["Items"] = _items;
		}
	}

	public override void LoadData(NPC npc, TagCompound tag) {
		base.LoadData(npc, tag);

		_items = tag.Get<List<Item>>("Items");
	}
}
