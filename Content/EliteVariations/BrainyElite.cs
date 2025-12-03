using EliteEnemies.Common;
using Terraria.DataStructures;

namespace EliteEnemies.Content.EliteVariations;

public class BrainyElite : EliteVariation
{
	public override EliteVariationRarity Rarity {
		get => EliteVariationRarity.Legendary;
	}

	public override bool CanApply(NPC npc) {
		return NPC.downedBoss2;
	}

	public override void OnApply(NPC npc) {
		if (!ApplyEliteVariation) {
			return;
		}

		NPCRenderRedirectSystem.RegisterRenderAction(npc, (int)RenderPriority.Last + 50, static (npc, renderTarget, spriteBatch) => {
			var drawData = new DrawData(renderTarget, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
			drawData.Draw(spriteBatch);
			(drawData with { effect = SpriteEffects.FlipHorizontally }).Draw(spriteBatch);
			(drawData with { effect = SpriteEffects.FlipVertically }).Draw(spriteBatch);
			(drawData with { effect = SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically }).Draw(spriteBatch);
		});
	}
}
