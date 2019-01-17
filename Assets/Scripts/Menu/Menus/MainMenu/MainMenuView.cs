using UniRx;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;

public class MainMenuView : MonoBehaviour
{
	[Header("UI")]
	[SerializeField] private Text scenesText;
	[SerializeField] private Renderer imageRenderer;

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
			imageRenderer.material.mainTexture = TextureLoader.LoadTextureFromFile
				(PathHelper.GetNewScenePath());
		}
		else
		{
			string path = PathHelper.GetScenePngPath(SceneGuids[ActiveItemIndex - 1]);

			FileInfo fi = new FileInfo(path);
			if (fi.Exists)
			{
				Texture2D texture= TextureLoader.LoadTextureFromFile(path);
				imageRenderer.material.mainTexture = texture;
			}
			else
			{
				path = PathHelper.GetNoImagePath();
				Texture2D texture = TextureLoader.LoadTextureFromFile(path);
				imageRenderer.material.mainTexture = texture;
			}
		}
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
