using EliteEnemies.Common;
using Terraria;

namespace EliteEnemies.Content.EliteVariations;

public class ImmovableElite : EliteVariation
{
    public override float SpawnChance => 0.5f;

    public override float ValueMultiplier => 3f;

    public override float LootMultiplier => 1.8f;

    public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
    {
        if (ApplyEliteVariation) {
            modifiers.DisableKnockback();
        }
    }
}