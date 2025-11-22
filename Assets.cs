namespace EliteEnemies;

public class Assets : ILoadable
{
	public void Load(Mod mod) {
		Textures.Empty = mod.Assets.Request<Texture2D>("Assets/Textures/EmptyTexture");
		Textures.Noise01 = mod.Assets.Request<Texture2D>("Assets/Textures/Noise01");

		Shaders.Glitch = mod.Assets.Request<Effect>("Assets/Shaders/GlitchShader");
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
		public static Asset<Texture2D> Noise01;
	}

	public static class Shaders
	{
		public static Asset<Effect> Glitch;
		public static Asset<Effect> Outline;
		public static Asset<Effect> Rainbow;
	}
}
