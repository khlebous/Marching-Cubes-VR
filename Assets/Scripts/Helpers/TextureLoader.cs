using UnityEngine;

public static class TextureLoader
{
	public static Texture2D LoadTextureFromFile(string path)
	{
		Texture2D tex;
		tex = new Texture2D(4, 4, TextureFormat.DXT1, false);
		WWW www = new WWW(path);
		while (!www.isDone);
		www.LoadImageIntoTexture(tex);
		return tex;
	}
}
