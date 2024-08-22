using EliteEnemies.Common;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace EliteEnemies.Content.EliteVariations;

public class ImmovableElite : EliteVariation
{
    public override float SpawnChance => 0.5f;

    public override float ValueMultiplier => 3f;

    public override float LootMultiplier => 1.8f;

    public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers) {
        if (ApplyEliteVariation) {
            modifiers.DisableKnockback();

            SoundEngine.PlaySound(SoundID.Item37 with { PitchVariance = 0.3f, Pitch = 0.3f }, npc.Center);
            int numDust = Main.rand.Next(2, 4);
            for (int i = 0; i < numDust; i++) {
                Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<ImmovableEliteDust>());
            }
        }
    }
}

public class ImmovableEliteDust : ModDust
{
    public override void OnSpawn(Dust dust) {
        dust.noGravity = true;
        dust.frame = new Rectangle(0, Main.rand.Next(3), 16, 16);

        dust.velocity = Main.rand.NextVector2Circular(1f, 1f) * Main.rand.NextFloat(2f, 4f);
        dust.scale = Main.rand.NextFloat(1.2f, 1.5f);
    }

    public override bool Update(Dust dust) {
        dust.position += dust.velocity;
        dust.velocity *= 0.94f;
        dust.scale -= 0.015f;

        if (dust.scale < 0.7f) {
            dust.active = false;
        }

        return false;
    }
}
