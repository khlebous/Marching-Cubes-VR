using UnityEngine;
using System;
using UniRx;
using System.Collections;
using System.Collections.Generic;

public class MainMenuController : MonoBehaviour
{
	[Header("Menu items")]
	[SerializeField] private MenuItemV loadSceneItem;
	[SerializeField] private MenuItemV deleteSceneItem;
	[SerializeField] private MenuItemV quitItem;

	[Header("Other")]
	[SerializeField] private McManager mcManager;

	public int ActiveSceneIndex { get; private set; }
	public int MaxItemIndex { get; private set; }
	public List<Guid> ScenesGuids { get; private set; }

	protected ISubject<Unit> itemChangedSubject = new Subject<Unit>();
	public IObservable<Unit> ItemChangedStream { get { return itemChangedSubject; } }

	protected ISubject<Unit> menuEnabledSubject = new Subject<Unit>();
	public IObservable<Unit> MenuEnabledStream { get { return menuEnabledSubject; } }

	protected ISubject<Guid> itemSelectedSubject = new Subject<Guid>();
	public IObservable<Guid> ItemSelectedStream { get { return itemSelectedSubject; } }

	private OVRInput.RawButton selectItemButton = OVRInput.RawButton.LThumbstick;
	private OVRInput.RawButton prevSceneButton = OVRInput.RawButton.LThumbstickLeft;
	private OVRInput.RawButton nextSceneButton = OVRInput.RawButton.LThumbstickRight;
	private OVRInput.RawButton prevItemButton = OVRInput.RawButton.LThumbstickUp;
	private OVRInput.RawButton nextItemButton = OVRInput.RawButton.LThumbstickDown;

	private List<MenuItemV> items;
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
			if (OVRInput.Get(selectItemButton))
			{
				isMenuActive = false;
				ItemSelected();
			}
			else
			{
				if (OVRInput.GetDown(prevSceneButton))
				{
						DecreaseActiveSceneIndex();
						itemChangedSubject.OnNext(Unit.Default);
				}
				else if (OVRInput.GetDown(nextSceneButton))
				{
						IncreaseActiveSceneIndex();
						itemChangedSubject.OnNext(Unit.Default);
				}
				else if (OVRInput.GetDown(prevItemButton))
				{
						items[activeItemIndex].SetInactive();
						DecreaseActiveItemIndex();
						items[activeItemIndex].SetActive();
				}
				else if (OVRInput.GetDown(nextItemButton))
				{
						items[activeItemIndex].SetInactive();
						IncreaseActiveItemIndex();
						items[activeItemIndex].SetActive();
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
