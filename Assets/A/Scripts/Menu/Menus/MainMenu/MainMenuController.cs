using System.Collections;
using UniRx;
using UnityEngine;
using System;
using System.Collections.Generic;

public class MainMenuController : MonoBehaviour
{
	[Header("Input")]
	[SerializeField] private OVRInput.Button selectItemButton = OVRInput.Button.PrimaryThumbstick;
	[SerializeField] private OVRInput.Button nextItemButton = OVRInput.Button.PrimaryThumbstickRight;
	[SerializeField] private OVRInput.Button prevItemButton = OVRInput.Button.PrimaryThumbstickLeft;

	public int ActiveItemIndex { get; private set; } 
	public int MaxItemIndex { get; private set; }
	public List<Guid> ScenesGuids;

	protected ISubject<Unit> itemChangedSubject = new Subject<Unit>();
	public IObservable<Unit> ItemChangedStream { get { return itemChangedSubject; } }

	protected ISubject<Unit> menuEnabledSubject = new Subject<Unit>();
	public IObservable<Unit> MenuEnabledStream { get { return menuEnabledSubject; } }

	protected ISubject<Guid> itemSelectedSubject = new Subject<Guid>();
	public IObservable<Guid> ItemSelectedStream { get { return itemSelectedSubject; } }

	private ButtonState currThumbstickState = ButtonState.Normal;
	private bool isMenuActive;

	private void Start()
	{
		SetInactive();
	}

	public void SetActive(List<Guid> sceneGuids)
	{
		SetupMenu(sceneGuids);
		menuEnabledSubject.OnNext(Unit.Default);

		StartCoroutine(WaitNextFrame());
	}

	private void SetupMenu(List<Guid> sceneGuids)
	{
		ActiveItemIndex = 0;
		ScenesGuids = sceneGuids;
		MaxItemIndex = ScenesGuids.Count + 1;
	}

	IEnumerator WaitNextFrame()
	{
		yield return new WaitForSeconds(0.5f);

		isMenuActive = true;
	}

	public void SetInactive()
	{
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
						DecreaseActiveItemIndex();
						itemChangedSubject.OnNext(Unit.Default);
					}
				}
				else if (OVRInput.Get(nextItemButton))
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
		itemSelectedSubject.OnNext(ScenesGuids[ActiveItemIndex]);
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
