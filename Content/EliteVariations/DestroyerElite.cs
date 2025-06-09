using EliteEnemies.Common;
using MonoMod.Cil;
using Terraria.Audio;

namespace EliteEnemies.Content.EliteVariations;

public class DestroyerElite : EliteVariation
{
	public override EliteVariationRarity Rarity {
		get => EliteVariationRarity.Rare;
	}

	public override bool CanApply(NPC npc) {
		return NPC.downedBoss2 && ServerConfig.Instance.EnableDestroyer;
	}

	public override void AI(NPC npc) {
		if (ApplyEliteVariation && Main.rand.NextBool(5)) {
			var dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.YellowTorch);
			dust.velocity *= 0.8f;
			dust.scale = Main.rand.NextFloat(0.9f, 1.2f);
			dust.noGravity = true;
		}
	}

	public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers) {
		if (ApplyEliteVariation && Main.rand.NextBool(10)) {
			ApplyCrit(ref modifiers, target);
		}
	}
	
	internal static void ApplyCrit(ref Player.HurtModifiers modifiers, Player target) 
	{
		modifiers.FinalDamage *= 2f;
		CritifyPlayerCombatText.MakeCritText = true;
		var sound = SoundID.Item127 with {
			Volume = 2f,
			PitchRange = (-1f, -0.5f),
		};
		SoundEngine.PlaySound(sound, target.Center);
	}
}

public class ShotByDestroyerEliteGlobalProjectile : ShotByEliteVariationGlobalProjectile<DestroyerElite>
{
	public override void ModifyHitPlayer(Projectile projectile, Player target, ref Player.HurtModifiers modifiers) {
		if (ApplyEliteChanges && Main.rand.NextBool(10)) {
			DestroyerElite.ApplyCrit(ref modifiers, target);
		}
	}
}

public class CritifyPlayerCombatText : ILoadable
{
	public static bool MakeCritText;
	
	public void Load(Mod mod) {
		IL_Player.Hurt_HurtInfo_bool += il => {
			var cursor = new ILCursor(il);
			
			cursor.GotoNext(MoveType.Before, 
				i => i.MatchLdloc(5),
				i => i.MatchLdcI4(0),
				i => i.MatchCall<CombatText>(nameof(CombatText.NewText)));

			cursor.Index++;
			cursor.EmitPop();
			cursor.EmitDelegate(() => {
				bool currentMakeCritText = MakeCritText;
				MakeCritText = false;
				return currentMakeCritText;
			});
		};
	}

	public void Unload() { }
}
