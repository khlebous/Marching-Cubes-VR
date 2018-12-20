using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
	[SerializeField] Text modeText;

	private void SetSceneEditingMode()
	{
		modeText.text = "SetSceneEditingMode";
	}

	private void SetTerrainEditingMode()
	{
		modeText.text = "SetTerrainEditingMode";
	}

	private void SetObjectEditingMode()
	{
		modeText.text = "SetObjectEditingMode";
	}
}
