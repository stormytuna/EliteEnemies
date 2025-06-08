using System;
using System.Collections.Generic;
using System.Linq;
using FishUtils.DataStructures;

namespace EliteEnemies.Common;

/// <summary>
/// Represents an action rendering an NPC.
/// </summary>
/// <param name="npc">The NPC to render.</param>
/// <param name="renderTarget">The render target containing the NPCs sprite on the screen. This is the size of the screen.</param>
/// <param name="spriteBatch">The sprite batch used for rendering.</param>
public delegate void NPCRenderAction(NPC npc, RenderTarget2D renderTarget, SpriteBatch spriteBatch);

/// <summary>
/// Encapsulates an NPCRenderAction with an associated priority.
/// </summary>
/// <param name="action">The render action to be performed.</param>
/// <param name="priority">The priority of the render action.</param>
public record class NPCRenderActionWithPriority(NPCRenderAction action, int priority);

public enum RenderPriority
{
	First = 100,
	Middle = 1000,
	Last = 10000,
}

public class NPCRenderRedirectSystem : ModSystem
{
	private static RenderTarget2D _stagingRT;
	private static RenderTarget2D _stagingRT2;
	private static RenderTarget2D _finalRT;
	
	private static Dictionary<int, List<NPCRenderActionWithPriority>> _renderActions = new();
	
	public static bool Ready = false;

	public override void Load() {
		if (Main.netMode == NetmodeID.Server) {
			return;
		}
		
		Main.QueueMainThreadAction(() => ResizeRenderTargets(Main.ScreenSize));
		Main.OnResolutionChanged += res => { ResizeRenderTargets(res.ToPoint()); };

		On_Main.CheckMonoliths += DrawToRenderTargets;
		On_Main.DrawNPCDirect += PreventDrawingQueuedNPCs;
		On_Main.DoDraw_DrawNPCsBehindTiles += DrawRenderTargetToScreen;
	}

	public override void Unload() {
		Main.QueueMainThreadAction(() => {
			_stagingRT?.Dispose();
			_stagingRT2?.Dispose();
			_finalRT?.Dispose();
		});
	}
	
	private void ResizeRenderTargets(Point screenSize) {
		_stagingRT?.Dispose();
		_stagingRT2?.Dispose();
		_finalRT?.Dispose();
		_stagingRT = new RenderTarget2D(Main.graphics.GraphicsDevice, screenSize.X, screenSize.Y, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
		_stagingRT2 = new RenderTarget2D(Main.graphics.GraphicsDevice, screenSize.X, screenSize.Y, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
		_finalRT = new RenderTarget2D(Main.graphics.GraphicsDevice, screenSize.X, screenSize.Y, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
	}

	private void DrawToRenderTargets(On_Main.orig_CheckMonoliths orig) {
		if (Main.gameMenu) {
			return;
		}
		
		var device = Main.graphics.GraphicsDevice;
		
		device.SetRenderTarget(_finalRT);
		device.Clear(Color.Transparent);
		
		Ready = false;
		
		foreach ((var npcWhoAmI, var renderActions) in _renderActions) {
			NPC npc = Main.npc[npcWhoAmI];
			if (!npc.active) {
				_renderActions.Remove(npcWhoAmI);
				continue;
			}
			
		    device.SetRenderTarget(_stagingRT);
		    device.Clear(Color.Transparent);
		    
		    Main.spriteBatch.Begin(SpriteBatchParams.Default with { TransformMatrix = Matrix.Identity });
		    
		    Main.instance.DrawNPCDirect(Main.spriteBatch, npc, npc.behindTiles, Main.screenPosition);
		    
		    Main.spriteBatch.Restart(SpriteBatchParams.Default with { TransformMatrix = Matrix.Identity });

		    var useStaging2 = false;
		    var sortedRenderActions = renderActions
			    .OrderBy(x => x.priority)
			    .Select(x => x.action)
			    .ToList();
		    for (int i = 0; i < sortedRenderActions.Count; i++) {
			    NPCRenderAction renderAction = sortedRenderActions[i];
			    
			    useStaging2 = !useStaging2;
			    device.SetRenderTarget(i % 2 == 0 ? _stagingRT2 : _stagingRT);
			    device.Clear(Color.Transparent);

			    renderAction(npc, i % 2 == 0 ? _stagingRT : _stagingRT2, Main.spriteBatch);
		    }
		    
		    device.SetRenderTarget(_finalRT);

		    Main.spriteBatch.Draw(useStaging2 ? _stagingRT2 : _stagingRT, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
		    
		    Main.spriteBatch.End();
		}
		
		Ready = true;
	}
	
	private void PreventDrawingQueuedNPCs(On_Main.orig_DrawNPCDirect orig, Main self, SpriteBatch mySpriteBatch, NPC rCurrentNPC, bool behindTiles, Vector2 screenPos) {
		if (Ready && _renderActions.ContainsKey(rCurrentNPC.whoAmI)) {
			return;	
		}
		
		orig(self, mySpriteBatch, rCurrentNPC, behindTiles, screenPos);
	}
	
	private void DrawRenderTargetToScreen(On_Main.orig_DoDraw_DrawNPCsBehindTiles orig, Main self) {
		orig(self);
		
		if (!Ready) {
			return;
		}

		Main.spriteBatch.Begin(SpriteBatchParams.Default);
		Main.spriteBatch.Draw(_finalRT, Vector2.Zero, Color.White);
		Main.spriteBatch.End();
	}
	
	/// <summary>
	/// Registers an NPC render action with the specified NPC and priority. The render action is deregistered automatically when the NPC is inactive.
	/// </summary>
	/// <param name="npc">The NPC.</param>
	/// <param name="priority">The priority of the render action.</param>
	/// <param name="renderAction">The render action to perform.</param>
	public static void RegisterRenderAction(NPC npc, int priority, NPCRenderAction renderAction) {
		if (Main.netMode == NetmodeID.Server) {
			return;
		}

		if (!_renderActions.TryGetValue(npc.whoAmI, out List<NPCRenderActionWithPriority> value)) {
			value = ([]);
			_renderActions.Add(npc.whoAmI, value);
		}

		value.Add(new NPCRenderActionWithPriority(renderAction, priority));
	}
}
