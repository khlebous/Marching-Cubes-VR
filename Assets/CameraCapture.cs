using UnityEngine;

public class CameraCapture : MonoBehaviour
{
	[SerializeField] private GameObject leftHand;
	[SerializeField] private GameObject rightHand;
	[SerializeField] private GameObject controllerLeft;
	[SerializeField] private GameObject controllerRight;

	private int resWidth = 512;
	private int resHeight = 512;
	private Camera cameraForCapture;

	private void Start()
	{
		cameraForCapture = GetComponent<Camera>();
	}

	public static string ScreenShotName(int width, int height)
	{
		Debug.Log(Application.dataPath);
		return string.Format("{0}/Resources/screen_{1}x{2}_{3}.png",
							 Application.dataPath,
							 width, height,
							 System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
	}

	public void TakeShot(string path)
	{
		leftHand.SetActive(false);
		rightHand.SetActive(false);
		controllerLeft.SetActive(false);
		controllerRight.SetActive(false);

		RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
		cameraForCapture.targetTexture = rt;
		Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
		cameraForCapture.Render();
		RenderTexture.active = rt;
		screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
		cameraForCapture.targetTexture = null;
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
}
