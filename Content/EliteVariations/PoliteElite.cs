using EliteEnemies.Common;

namespace EliteEnemies.Content.EliteVariations;

public class PoliteElite : EliteVariation
{
	public override EliteVariationRarity Rarity {
		get => EliteVariationRarity.Uncommon;
	}

    public override bool CanApply(NPC npc) {
        return ServerConfig.Instance.EnablePolite;
    }

	public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo) {
		if (!ApplyEliteVariation) {
			return;
		}

		int sorryOption = Main.rand.Next(12);
		string sorryText = Mod.GetLocalization($"Sorries.{sorryOption}").Value;
		var popupText = new AdvancedPopupRequest {
			Color = Color.LightGray,
			Text = sorryText,
			Velocity = new Vector2(0f, -6f),
			DurationInFrames = 60,
		};
		PopupText.NewText(popupText, npc.Top);
	}
}

public class ShotByPoliteEliteGlobalProjectile : ShotByEliteVariationGlobalProjectile<PoliteElite>
{
	public override void OnHitPlayer(Projectile projectile, Player target, Player.HurtInfo info) {
		if (ApplyEliteChanges) {
			Parent.GetGlobalNPC<PoliteElite>().OnHitPlayer(Parent, target, info);
		}
	}
}
