namespace EliteEnemies;

public class Assets : ILoadable
{
	public void Load(Mod mod) {
		Shaders.Outline = mod.Assets.Request<Effect>("Assets/OutlineShader");
	}

	public void Unload() {
	}

	public static class Shaders
	{
		public static Asset<Effect> Outline;
	}
}
