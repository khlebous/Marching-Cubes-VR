using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuLeftSceneController : MonoBehaviour
{
	internal void OpenMenu()
	{
		gameObject.SetActive(true);
	}

	internal void CloseMenu()
	{
		gameObject.SetActive(false);
	}
}
