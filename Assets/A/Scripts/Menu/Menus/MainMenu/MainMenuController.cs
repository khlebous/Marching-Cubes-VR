using UnityEngine;
using System;
using UniRx;
using System.Collections;
using System.Collections.Generic;

public class MainMenuController : MonoBehaviour
{
	[Header("Menu items")]
	[SerializeField]
	private MenuItemV loadSceneItem;
	[SerializeField] private MenuItemV deleteSceneItem;
	[SerializeField] private MenuItemV quitItem;

	[Header("Other")]
	[SerializeField]
	private McManager mcManager;

	public int ActiveSceneIndex { get; private set; }
	public int MaxItemIndex { get; private set; }
	public List<Guid> ScenesGuids { get; private set; }

	protected ISubject<Unit> itemChangedSubject = new Subject<Unit>();
	public IObservable<Unit> ItemChangedStream { get { return itemChangedSubject; } }

	protected ISubject<Unit> menuEnabledSubject = new Subject<Unit>();
	public IObservable<Unit> MenuEnabledStream { get { return menuEnabledSubject; } }

	protected ISubject<Guid> itemSelectedSubject = new Subject<Guid>();
	public IObservable<Guid> ItemSelectedStream { get { return itemSelectedSubject; } }

	private OVRInput.Button selectItemButton = OVRInput.Button.PrimaryThumbstick;
	private OVRInput.Button prevSceneButton = OVRInput.Button.PrimaryThumbstickLeft;
	private OVRInput.Button nextSceneButton = OVRInput.Button.PrimaryThumbstickRight;
	private OVRInput.Button prevItemButton = OVRInput.Button.PrimaryThumbstickUp;
	private OVRInput.Button nextItemButton = OVRInput.Button.PrimaryThumbstickDown;
	private OVRInput.Controller controller = OVRInput.Controller.LTouch;

	private List<MenuItemV> items;
	private ButtonState currThumbstickState;
	private bool isMenuActive;
	private int activeItemIndex;

	private void Awake()
	{
		items = new List<MenuItemV>
		{
			loadSceneItem,
			deleteSceneItem,
			quitItem
		};
		SetupMenu();
	}

	private void SetupMenu()
	{
		currThumbstickState = ButtonState.Normal;
		isMenuActive = false;
		activeItemIndex = 0;

		foreach (var item in items)
			item.SetInactive();
		items[activeItemIndex].SetActive();
	}

	public void ResetMenu(List<Guid> sceneGuids)
	{
		SetupMenu();

		SetupMenu(sceneGuids);
		menuEnabledSubject.OnNext(Unit.Default);

		gameObject.SetActive(true);
		StartCoroutine(WaitNextFrameAndSetMenuActive());
	}

	public void SetInactive()
	{
		gameObject.SetActive(false);
		isMenuActive = false;
	}

	private void SetupMenu(List<Guid> sceneGuids)
	{
		ActiveSceneIndex = 0;
		ScenesGuids = sceneGuids;
		MaxItemIndex = ScenesGuids.Count + 1;
	}

	IEnumerator WaitNextFrameAndSetMenuActive()
	{
		yield return new WaitForSeconds(0.5f);

		isMenuActive = true;
	}


	void Update()
	{
		if (isMenuActive)
		{
			if (OVRInput.Get(selectItemButton, controller))
			{
				isMenuActive = false;
				ItemSelected();
			}
			else
			{
				if (OVRInput.Get(prevSceneButton, controller))
				{
					if (currThumbstickState == ButtonState.Normal)
					{
						currThumbstickState = ButtonState.Left;
						DecreaseActiveSceneIndex();
						itemChangedSubject.OnNext(Unit.Default);
					}
				}
				else if (OVRInput.Get(nextSceneButton, controller))
				{
					if (currThumbstickState == ButtonState.Normal)
					{
						currThumbstickState = ButtonState.Right;
						IncreaseActiveSceneIndex();
						itemChangedSubject.OnNext(Unit.Default);
					}
				}
				else if (OVRInput.Get(prevItemButton, controller))
				{
					if (currThumbstickState == ButtonState.Normal)
					{
						currThumbstickState = ButtonState.Up;
						items[activeItemIndex].SetInactive();
						DecreaseActiveItemIndex();
						items[activeItemIndex].SetActive();
					}
				}
				else if (OVRInput.Get(nextItemButton, controller))
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
			if (ActiveSceneIndex == 0)
				itemSelectedSubject.OnNext(Guid.Empty);
			else
				itemSelectedSubject.OnNext(ScenesGuids[ActiveSceneIndex - 1]);
		}
		else if (activeItemIndex == 1)
		{
			if (ActiveSceneIndex == 0)
			{
				StartCoroutine(WaitNextFrameAndSetMenuActive());
				return;
			}

			mcManager.DeleteScene(ScenesGuids[ActiveSceneIndex - 1]);
			SetupMenu(mcManager.GetAllSceneGuids());
			itemChangedSubject.OnNext(Unit.Default);
			StartCoroutine(WaitNextFrameAndSetMenuActive());
		}
		else if (activeItemIndex == 2)
		{
			Application.Quit();
		}
	}


	private void IncreaseActiveSceneIndex()
	{
		ActiveSceneIndex++;
		ActiveSceneIndex %= MaxItemIndex;
	}

	private void DecreaseActiveSceneIndex()
	{
		if (ActiveSceneIndex == 0)
			ActiveSceneIndex = MaxItemIndex;
		ActiveSceneIndex--;
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
