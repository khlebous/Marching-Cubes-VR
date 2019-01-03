using UnityEngine;
using UniRx;
using System.Collections.Generic;

public class MenuLeftObjectController : MonoBehaviour
{
	[Header("Menu items")]
	[SerializeField] private MenuItemV saveExitItem;
	[SerializeField] private MenuItemV dontSaveExitItem;

	protected ISubject<Unit> exitToSceneModeSubject = new Subject<Unit>();
	public IObservable<Unit> ExitToSceneModeStream { get { return exitToSceneModeSubject; } }

	protected ISubject<Unit> saveAndExitToSceneModeSubject = new Subject<Unit>();
	public IObservable<Unit> SaveAndExitToSceneModeStream { get { return saveAndExitToSceneModeSubject; } }

	private OVRInput.Button prevItemButton = OVRInput.Button.PrimaryThumbstickUp;
	private OVRInput.Button nextItemButton = OVRInput.Button.PrimaryThumbstickDown;
	private OVRInput.Button selectItemButton = OVRInput.Button.PrimaryThumbstick;

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
			item.SetInactive();

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
			saveAndExitToSceneModeSubject.OnNext(Unit.Default);
		else if (activeItemIndex == 1)
			exitToSceneModeSubject.OnNext(Unit.Default);
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
