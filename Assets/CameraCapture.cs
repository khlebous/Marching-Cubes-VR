using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CameraCapture : MonoBehaviour
{
	private int resWidth = 512;
	private int resHeight = 512;

	private bool takeHiResShot = false;

	Camera camera;
	private void Start()
	{
		camera = GetComponent<Camera>();
	}

	public static string ScreenShotName(int width, int height)
	{
		Debug.Log(Application.dataPath);
		return string.Format("{0}/screen_{1}x{2}_{3}.png",
							 Application.dataPath,
							 width, height,
							 System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
	}

	public void TakeHiResShot()
	{
		takeHiResShot = true;
	}

	void LateUpdate()
	{
		takeHiResShot |= Input.GetKeyDown("k");
		if (takeHiResShot)
		{
			RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
			camera.targetTexture = rt;
			Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
			camera.Render();
			RenderTexture.active = rt;
			screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
			camera.targetTexture = null;
			RenderTexture.active = null; // JC: added to avoid errors
			Destroy(rt);
			byte[] bytes = screenShot.EncodeToPNG();
			string filename = ScreenShotName(resWidth, resHeight);
			System.IO.File.WriteAllBytes(filename, bytes);
			Debug.Log(string.Format("Took screenshot to: {0}", filename));
			takeHiResShot = false;
		}
	}
}
