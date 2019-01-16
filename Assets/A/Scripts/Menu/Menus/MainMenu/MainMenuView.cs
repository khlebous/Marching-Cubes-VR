using UniRx;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class MainMenuView : MonoBehaviour
{
	[Header("UI")]
	[SerializeField]
	private Text scenesText;
	[SerializeField] private Renderer renderer;

	[Header("Controller")]
	[SerializeField]
	private MainMenuController controller;

	private int ActiveItemIndex { get { return controller.ActiveSceneIndex; } }
	private int MaxItemIndex { get { return controller.MaxItemIndex; } }

	private List<Guid> SceneGuids { get { return controller.ScenesGuids; } }

	void Start()
	{
		controller.ItemChangedStream.Subscribe(_ => UpdateUI());
		controller.MenuEnabledStream.Subscribe(_ => OnMenuEnable());
		controller.ItemSelectedStream.Subscribe(_ => OnMenuDisable());
	}

	private void UpdateUI()
	{
		scenesText.text = ActiveItemIndex + "/" + (MaxItemIndex - 1);
		if (ActiveItemIndex == 0)
		{
			renderer.material.mainTexture = TextureLoader.LoadTextureFromFile
				(Application.dataPath + "/Resources/NewScene.png");
		}
		else
		{
			string path = GetFullPath(SceneGuids[ActiveItemIndex - 1]);
			renderer.material.mainTexture = TextureLoader.LoadTextureFromFile(path);
		}
	}

	private string GetFullPath(Guid guid)
	{
		return Application.dataPath + "/Resources/" + guid.ToString()
			+ "/" + guid.ToString() + ".png";
	}


	private void OnMenuEnable()
	{
		UpdateUI();
		gameObject.SetActive(true);
	}

	private void OnMenuDisable()
	{
		gameObject.SetActive(false);
	}
}
