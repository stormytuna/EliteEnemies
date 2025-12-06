namespace EliteEnemies;

public class Assets : ILoadable
{
	public void Load(Mod mod) {
		Textures.Empty = mod.Assets.Request<Texture2D>("Assets/Textures/EmptyTexture");
		Textures.InfoIcon = mod.Assets.Request<Texture2D>("Assets/Textures/InfoIcon", AssetRequestMode.ImmediateLoad);
		Textures.SettingsIcon = mod.Assets.Request<Texture2D>("Assets/Textures/SettingsIcon", AssetRequestMode.ImmediateLoad);
		Textures.LeftArrowIcon = mod.Assets.Request<Texture2D>("Assets/Textures/LeftArrowIcon", AssetRequestMode.ImmediateLoad);
		Textures.RightArrowIcon = mod.Assets.Request<Texture2D>("Assets/Textures/RightArrowIcon", AssetRequestMode.ImmediateLoad);
		Textures.NineSliceOutsetBasic = mod.Assets.Request<Texture2D>("Assets/Textures/NineSliceOutsetBasic");
		Textures.NineSliceOutsetBasicJoinLeft = mod.Assets.Request<Texture2D>("Assets/Textures/NineSliceOutsetBasicJoinLeft");
		Textures.NineSliceOutsetBasicJoinRight = mod.Assets.Request<Texture2D>("Assets/Textures/NineSliceOutsetBasicJoinRight");
		Textures.NineSliceOutsetBasicJoinLeftRight = mod.Assets.Request<Texture2D>("Assets/Textures/NineSliceOutsetBasicJoinLeftRight");
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
		public static Asset<Texture2D> InfoIcon;
		public static Asset<Texture2D> SettingsIcon;
		public static Asset<Texture2D> LeftArrowIcon;
		public static Asset<Texture2D> RightArrowIcon;
		public static Asset<Texture2D> NineSliceOutsetBasic;
		public static Asset<Texture2D> NineSliceOutsetBasicJoinLeft;
		public static Asset<Texture2D> NineSliceOutsetBasicJoinRight;
		public static Asset<Texture2D> NineSliceOutsetBasicJoinLeftRight;
		public static Asset<Texture2D> Noise01;
	}

	public static class Shaders
	{
		public static Asset<Effect> Glitch;
		public static Asset<Effect> Outline;
		public static Asset<Effect> Rainbow;
	}
}
