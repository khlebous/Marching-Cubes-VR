using UnityEngine;
using System;
using UniRx;
using System.Collections;
using System.Collections.Generic;

public class MainMenuController : MonoBehaviour
{

	public int ActiveItemIndex { get; private set; }
	public int MaxItemIndex { get; private set; }
	public List<Guid> ScenesGuids { get; private set; }

	protected ISubject<Unit> itemChangedSubject = new Subject<Unit>();
	public IObservable<Unit> ItemChangedStream { get { return itemChangedSubject; } }

	protected ISubject<Unit> menuEnabledSubject = new Subject<Unit>();
	public IObservable<Unit> MenuEnabledStream { get { return menuEnabledSubject; } }

	protected ISubject<Guid> itemSelectedSubject = new Subject<Guid>();
	public IObservable<Guid> ItemSelectedStream { get { return itemSelectedSubject; } }

	private OVRInput.Button selectItemButton = OVRInput.Button.PrimaryThumbstick;
	private OVRInput.Button prevItemButton = OVRInput.Button.PrimaryThumbstickLeft;
	private OVRInput.Button nextItemButton = OVRInput.Button.PrimaryThumbstickRight;
	private OVRInput.Controller controller = OVRInput.Controller.LTouch;

	private ButtonState currThumbstickState;
	private bool isMenuActive;

	public void SetActive(List<Guid> sceneGuids)
	{
		SetupMenu(sceneGuids);
		menuEnabledSubject.OnNext(Unit.Default);
		currThumbstickState = ButtonState.Normal;
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
		ActiveItemIndex = 0;
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
				if (OVRInput.Get(prevItemButton, controller))
				{
					if (currThumbstickState == ButtonState.Normal)
					{
						currThumbstickState = ButtonState.Up;
						DecreaseActiveItemIndex();
						itemChangedSubject.OnNext(Unit.Default);
					}
				}
				else if (OVRInput.Get(nextItemButton, controller))
				{
					if (currThumbstickState == ButtonState.Normal)
					{
						currThumbstickState = ButtonState.Down;
						IncreaseActiveItemIndex();
						itemChangedSubject.OnNext(Unit.Default);
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
		if (ActiveItemIndex == 0)
			itemSelectedSubject.OnNext(Guid.Empty);
		else
			itemSelectedSubject.OnNext(ScenesGuids[ActiveItemIndex - 1]);
	}

	 
	private void IncreaseActiveItemIndex()
	{
		ActiveItemIndex++;
		ActiveItemIndex %= MaxItemIndex;
	}

	private void DecreaseActiveItemIndex()
	{
		if (ActiveItemIndex == 0)
			ActiveItemIndex = MaxItemIndex;
		ActiveItemIndex--;
	}
}
