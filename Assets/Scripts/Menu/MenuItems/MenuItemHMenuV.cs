using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class MenuItemHMenuV : MenuItemV
{
	[SerializeField] private List<MenuItemV> items;

	[Header("Input")]
	[SerializeField] private OVRInput.Button nextItemButton = OVRInput.Button.SecondaryThumbstickRight;
	[SerializeField] private OVRInput.Button prevItemButton = OVRInput.Button.SecondaryThumbstickLeft;

	private ISubject<int> chosenItemStream = new Subject<int>();
	public IObservable<int> ChosenItemSubject { get { return chosenItemStream; } }

	private ButtonState thumbstick = ButtonState.Normal;
	private int activeItemIndex = 0;
	private bool active;

	public void Awake()
	{
		ResetItem();
	}

	public void ResetItem()
	{
		thumbstick = ButtonState.Normal;
		activeItemIndex = 0;
		active = false;

		foreach (var item in items)
			item.SetInactive();
		items[activeItemIndex].SetActive();

		ModeChanged();
	}

	public override void SetChosen()
	{
		base.SetChosen();
		active = true;
	}

	public override void SetNormal()
	{
		base.SetNormal();
		active = false;
	}

	void Update()
	{
		if (active)
		{
			if (OVRInput.Get(prevItemButton))
			{
				if (thumbstick == ButtonState.Normal)
				{
					thumbstick = ButtonState.Left;

					items[activeItemIndex].SetInactive();
					DecreaseActiveItemIndex();
					ModeChanged();
					items[activeItemIndex].SetActive();
				}

			}
			else if (OVRInput.Get(nextItemButton))
			{
				if (thumbstick == ButtonState.Normal)
				{
					thumbstick = ButtonState.Right;
					items[activeItemIndex].SetInactive();
					IncreaseActiveItemIndex();
					ModeChanged();
					items[activeItemIndex].SetActive();
				}
			}
			else
			{
				if (thumbstick != ButtonState.Normal)
					thumbstick = ButtonState.Normal;
			}
		}
	}

	private void ModeChanged()
	{
		chosenItemStream.OnNext(activeItemIndex);
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
