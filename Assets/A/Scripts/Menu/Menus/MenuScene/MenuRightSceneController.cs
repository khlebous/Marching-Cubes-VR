using UnityEngine;
using UniRx;
using System.Collections.Generic;
using System;

public class MenuRightSceneController : MonoBehaviour
{
	[Header("Menu items")]
	[SerializeField] private MenuItemV terrainMode;
	[SerializeField] private MenuItemV newModel;
	[SerializeField] private ModelMenuItemV addModelFromList;
	[SerializeField] private MenuItemV editModel;
	[SerializeField] private MenuItemV deleteModel;

	[Header("Other")]
	[SerializeField] private McManager mcManager;

	protected ISubject<Unit> exitToTerrainModeSubject = new Subject<Unit>();
	public IObservable<Unit> ExitToTerrainModeStream { get { return exitToTerrainModeSubject; } }

	protected ISubject<Guid> exitToObjectModeSubject = new Subject<Guid>();
	public IObservable<Guid> ExitToObjectModeStream { get { return exitToObjectModeSubject; } }

	private ISubject<Guid> modelToAddSelectedSubject = new Subject<Guid>();
	public IObservable<Guid> ModelToAddSelectedStream { get { return modelToAddSelectedSubject; } }

	protected ISubject<Unit> itemSelectedSubject = new Subject<Unit>();
	public IObservable<Unit> ItemSelectedStream { get { return itemSelectedSubject; } }

	private OVRInput.Button prevItemButton = OVRInput.Button.SecondaryThumbstickUp;
	private OVRInput.Button nextItemButton = OVRInput.Button.SecondaryThumbstickDown;
	private OVRInput.Button selectItemButton = OVRInput.Button.SecondaryThumbstick;

	private ButtonState currThumbstickState = ButtonState.Normal;
	private bool isMenuActive;
	private int activeItemIndex = 0;

	private List<MenuItemV> items;

	private void Start()
	{
		items = new List<MenuItemV>
		{
			terrainMode,
			newModel,
			addModelFromList,
			editModel,
			deleteModel
		};

		foreach (var item in items)
			item.SetInactive();

		items[activeItemIndex].SetActive();

		addModelFromList.ModelToAddSelectedStream.Subscribe(modelToAddSelectedSubject.OnNext);

		CloseMenu();
	}

	public void OpenMenu()
	{
		gameObject.SetActive(true);
		isMenuActive = true;

		//mcManager.getAllObjectGuids
		List<Guid> guids = new List<Guid>
		{
			Guid.NewGuid(),
			Guid.NewGuid(),
			Guid.NewGuid(),
			Guid.NewGuid()
		};

		addModelFromList.SetupMenu(guids);
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
		switch (activeItemIndex)
		{
			case 0: // Edycja terenu
				exitToTerrainModeSubject.OnNext(Unit.Default);
				break;
			case 1: // Nowy obiekt
				exitToObjectModeSubject.OnNext(Guid.Empty);
				break;
			case 2: // addModelFromList
				addModelFromList.SetChoosen();
				itemSelectedSubject.OnNext(Unit.Default);
					// TODO Show models menu
				break;
			case 3: // Edit model
					// TODO  Show models menu
				exitToObjectModeSubject.OnNext(Guid.NewGuid());
				break;
			case 4: // Delete Model
					// TODO Show models menu
					// TODO delete model
				break;
			default:
				break;
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
