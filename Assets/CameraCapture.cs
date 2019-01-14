﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CameraCapture : MonoBehaviour
{
	[SerializeField] private GameObject leftHand;
	[SerializeField] private GameObject rightHand;
	[SerializeField] private GameObject controllerLeft;
	[SerializeField] private GameObject controllerRight;

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
		return string.Format("{0}/Resources/screen_{1}x{2}_{3}.png",
							 Application.dataPath,
							 width, height,
							 System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
	}

	public void TakeShot()
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

	public void TakeShot(string path)
	{
		leftHand.SetActive(false);
		rightHand.SetActive(false);
		controllerLeft.SetActive(false);
		controllerRight.SetActive(false);

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
		string fullPath = path;

		System.IO.File.WriteAllBytes(fullPath, bytes);
		Debug.Log(string.Format("Took screenshot to: {0}", fullPath));

		leftHand.SetActive(true);
		rightHand.SetActive(true);
		controllerLeft.SetActive(true);
		controllerRight.SetActive(true);
	}

	public void TakeShot(string path, string filename)
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
		string fullPath = Application.dataPath + "/Resources/" + path
			+ "/" + filename + ".png";

		var fileInfo = new FileInfo(fullPath);
		fileInfo.Directory.Create();

		Debug.Log(fullPath);
		System.IO.File.Delete(fullPath);
		System.IO.File.WriteAllBytes(fullPath, bytes);
		Debug.Log(string.Format("Took screenshot to: {0}", fullPath));
		takeHiResShot = false;
	}
}
