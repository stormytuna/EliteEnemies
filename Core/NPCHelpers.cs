using EliteEnemies.Common;
using Terraria;
using Terraria.GameContent.Bestiary;

namespace EliteEnemies.Core;

public static class NPCHelpers
{
    public static bool HasEliteVariation<TVariation>(this NPC npc) where TVariation : EliteVariation {
        foreach (var global in npc.Globals) {
            if (global is TVariation t && t.ApplyEliteVariation) {
                return true;
            }
        }

        return false;
    }

    public static bool HasNotEliteVariation<TVariation>(this NPC npc) where TVariation : EliteVariation {
        return !HasEliteVariation<TVariation>(npc);
    }
}
