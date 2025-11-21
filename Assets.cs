namespace EliteEnemies;

public class Assets : ILoadable
{
	public void Load(Mod mod) {
		Textures.Empty = mod.Assets.Request<Texture2D>("Assets/Textures/EmptyTexture");

		Shaders.Outline = mod.Assets.Request<Effect>("Assets/Shaders/OutlineShader");
		Shaders.Rainbow = mod.Assets.Request<Effect>("Assets/Shaders/RainbowShader");
	}

	public void Unload() { }

	public static string EmptyTexturePath {
		get => $"{nameof(EliteEnemies)}/Assets/Textures/EmptyTexture";
	}

	public static class Textures
	{
		public static Asset<Texture2D> Empty;
	}

	public static class Shaders
	{
		public static Asset<Effect> Outline;
		public static Asset<Effect> Rainbow;
	}
}
