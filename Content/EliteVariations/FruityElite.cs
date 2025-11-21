using System.Collections.Generic;
using EliteEnemies.Common;
using FishUtils.DataStructures;

namespace EliteEnemies.Content.EliteVariations;

public class FruityElite : EliteVariation
{
	private float _randomColorOffset = Main.rand.NextFloat();
	
	public override EliteVariationRarity Rarity {
		get => EliteVariationRarity.SuperRare;
	}

	public override bool CanApply(NPC npc) {
		return Main.hardMode && ServerConfig.Instance.EnableFruity;
	}

	public override void OnApply(NPC npc) {
		if (!ApplyEliteVariation) {
			return;
		}
		
		NPCRenderRedirectSystem.RegisterRenderAction(npc, (int)RenderPriority.Middle, static (npc, renderTarget, spriteBatch) => {
			Main.spriteBatch.TakeSnapshotAndEnd(out SpriteBatchParams sbParams);

			var shader = Assets.Shaders.Rainbow.Value;
			
			float offset = npc.GetGlobalNPC<FruityElite>()._randomColorOffset;
			float hue = ((float)Main.timeForVisualEffects / 120) % 1f + offset;
			var rainbowColor = Main.hslToRgb(hue, 1f, 0.5f);
			shader.Parameters["rainbow"].SetValue(rainbowColor.ToVector3());
			
			Main.spriteBatch.Begin(sbParams with { Effect = shader });
			
			spriteBatch.Draw(renderTarget, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
			
			Main.spriteBatch.Restart(sbParams);
		});
	}
	
	public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo) {
		if (!ApplyEliteVariation || Main.rand.NextBool()) {
			return;
		}

		(int type, int time) = Main.rand.Next(45) switch {
			0 => (BuffID.Bleeding, 30 * 60),
			1 => (BuffID.Poisoned, 12 * 60),
			2 => (BuffID.OnFire, 15 * 60),
			3 => (BuffID.Venom, 4 * 60),
			4 => (BuffID.Darkness, 15 * 60),
			5 => (BuffID.Blackout, 12 * 60),
			6 => (BuffID.Silenced, 7 * 60),
			7 => (BuffID.Cursed, 5 * 60),
			8 => (BuffID.Confused, 15 * 60),
			9 => (BuffID.Slow, 20 * 60),
			10 => (BuffID.OgreSpit, 15 * 60),
			11 => (BuffID.Weak, 2 * 60 * 60),
			12 => (BuffID.BrokenArmor, 15 * 60),
			13 => (BuffID.WitheredArmor, 15 * 60),
			14 => (BuffID.WitheredWeapon, 5 * 60),
			15 => (BuffID.CursedInferno, 7 * 60),
			16 => (BuffID.Ichor, 12 * 60),
			17 => (BuffID.Frostburn, 6 * 60),
			18 => (BuffID.Chilled, 30 * 60),
			19 => (BuffID.Frozen, 60),
			20 => (BuffID.Webbed, 60),
			21 => (BuffID.Stoned, 2 * 60),
			22 => (BuffID.VortexDebuff, 10 * 60),
			23 => (BuffID.Obstructed, 5 * 60),
			24 => (BuffID.Electrified, 4 * 60),
			25 => (BuffID.Rabies, 60 * 60),
			26 => (BuffID.MoonLeech, 15 * 60),
			27 => (BuffID.ManaSickness, 5 * 60),
			28 => (BuffID.PotionSickness, 30 * 60),
			29 => (BuffID.ChaosState, 20 * 60),
			30 => (BuffID.Suffocation, 2 * 60),
			31 => (BuffID.Burning, 2 * 60),
			32 => (BuffID.Tipsy, 60 * 60),
			33 => (BuffID.Lovestruck, 2 * 60 * 60),
			34 => (BuffID.Stinky, 2 * 60 * 60),
			35 => (BuffID.WaterCandle, 2 * 60 * 60),
			36 => (BuffID.ShadowCandle, 2 * 60 * 60),
			37 => (BuffID.WindPushed, 2 * 60 * 60),
			38 => (BuffID.NeutralHunger, 2 * 60 * 60),
			39 => (BuffID.Hunger, 1 * 60 * 60),
			40 => (BuffID.Starving, 5 * 60),
			41 => (BuffID.Shimmer, 5 * 60),
			42 => (BuffID.Wet, 2 * 60 * 60),
			43 => (BuffID.Slimed, 2 * 60 * 60),
			44 => (BuffID.GelBalloonBuff, 2 * 60 * 60),
			_ => (BuffID.OnFire, 15 * 60),
		};
		
		target.AddBuff(type, time);
	}
}

public class ShotByFruityEliteGlobalProjectile : ShotByEliteVariationGlobalProjectile<FruityElite>
{
	public override void OnHitPlayer(Projectile projectile, Player target, Player.HurtInfo info) {
		if (ApplyEliteChanges) {
			Parent.GetGlobalNPC<FruityElite>().OnHitPlayer(Parent, target, info);
		}
	}
}
