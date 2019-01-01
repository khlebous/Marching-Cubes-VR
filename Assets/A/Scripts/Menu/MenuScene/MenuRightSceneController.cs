using UnityEngine;

public class MenuRightSceneController : MonoBehaviour
{
	public void OpenMenu()
	{
		gameObject.SetActive(true);
	}

	public void CloseMenu()
	{
		gameObject.SetActive(false);
	}
}
