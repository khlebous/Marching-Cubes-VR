﻿using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class MenuLeftSceneController : MonoBehaviour
{
	[Header("Input")]
	[SerializeField] private OVRInput.Button selectItemButton = OVRInput.Button.PrimaryThumbstick;
	[SerializeField] private OVRInput.Button nextItemButton = OVRInput.Button.PrimaryThumbstickDown;
	[SerializeField] private OVRInput.Button prevItemButton = OVRInput.Button.PrimaryThumbstickUp;

	[Header("Menu items")]
	[SerializeField] private MenuItemV saveExitItem;
	[SerializeField] private MenuItemV dontSaveExitItem;
	
	protected ISubject<Unit> exitToMainModeSubject = new Subject<Unit>();
	public IObservable<Unit> ExitToMainModeStream { get { return exitToMainModeSubject; } }

	protected ISubject<Unit> saveAndExitToMainModeSubject = new Subject<Unit>();
	public IObservable<Unit> SaveAndExitToMainModeStream { get { return saveAndExitToMainModeSubject; } }


	private ButtonState currThumbstickState = ButtonState.Normal;
	private bool isMenuActive;
	private int activeItemIndex = 0;

	private List<MenuItemV> items;

	private void Start()
	{
		items = new List<MenuItemV>
		{
			saveExitItem,
			dontSaveExitItem
		};

		foreach (var item in items)
		{
			item.SetInactive();
		}
		items[activeItemIndex].SetActive();
	}

	public void OpenMenu()
	{
		gameObject.SetActive(true);
		isMenuActive = true;
	}

	public void CloseMenu()
	{
		gameObject.SetActive(false);
		isMenuActive = false;
	}

	void Update()
	{
		if (isMenuActive)
		{
			if (OVRInput.Get(selectItemButton))
			{
				isMenuActive = false;
				ItemSelected();
			}
			else
			{
				if (OVRInput.Get(prevItemButton))
				{
					if (currThumbstickState == ButtonState.Normal)
					{
						currThumbstickState = ButtonState.Up;
						items[activeItemIndex].SetInactive();
						DecreaseActiveItemIndex();
						items[activeItemIndex].SetActive();
					}
				}
				else if (OVRInput.Get(nextItemButton))
				{
					if (currThumbstickState == ButtonState.Normal)
					{
						currThumbstickState = ButtonState.Down;
						items[activeItemIndex].SetInactive();
						IncreaseActiveItemIndex();
						items[activeItemIndex].SetActive();
					}
				}
				else
				{
					if (currThumbstickState != ButtonState.Normal)
					{
						currThumbstickState = ButtonState.Normal;
					}
				}
			}
		}
	}

	private void ItemSelected()
	{
		if (activeItemIndex == 0)
		{
			saveAndExitToMainModeSubject.OnNext(Unit.Default);
		}
		else if (activeItemIndex == 1)
		{
			exitToMainModeSubject.OnNext(Unit.Default);
		}
	}

	private void IncreaseActiveItemIndex()
	{
		activeItemIndex++;
		activeItemIndex %= items.Count;
	}

	private void DecreaseActiveItemIndex()
	{
		if (activeItemIndex == 0)
			activeItemIndex = items.Count;
		activeItemIndex--;
	}
}