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
			imageRenderer.material.mainTexture 
				= Resources.Load<Texture2D>(PathHelper.GetNewScenePath());
		}
		else
		{
			string path = PathHelper.GetScenePngPath(SceneGuids[ActiveItemIndex - 1]);
			if (File.Exists(path))
			{
				Texture2D texture= TextureLoader.LoadTextureFromFile(path);
				imageRenderer.material.mainTexture = texture;
			}
			else
			{
				imageRenderer.material.mainTexture 
					= Resources.Load<Texture2D>(PathHelper.GetNoImagePath());
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
