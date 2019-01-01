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

	[Header("Controller")]
	[SerializeField]
	private MainMenuController controller;

	private int ActiveItemIndex { get { return controller.ActiveItemIndex; } }
	private int MaxItemIndex { get { return controller.MaxItemIndex; } }

	private List<Guid> SceneGuids { get { return controller.ScenesGuids; } }

	void Start()
	{
		OnMenuDisable();

		controller.ItemChangedStream.Subscribe(_ => UpdateUI());
		controller.MenuEnabledStream.Subscribe(_ => OnMenuEnable());
		controller.ItemSelectedStream.Subscribe(_ => OnMenuDisable());
	}

	private void UpdateUI()
	{
		if (ActiveItemIndex == 0)
			scenesText.text = (ActiveItemIndex + 1) + "/" + MaxItemIndex + "\n (New scene)";
		else
			scenesText.text = (ActiveItemIndex + 1) + "/" + MaxItemIndex + "\n " + SceneGuids[ActiveItemIndex - 1];
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
