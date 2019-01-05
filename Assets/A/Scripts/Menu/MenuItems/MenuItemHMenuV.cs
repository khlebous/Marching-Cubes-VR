﻿using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class MenuItemHMenuV : MenuItemV
{
	[SerializeField] private List<MenuItemV> items;

	[Header("Input")]
	[SerializeField] private OVRInput.Button nextItemButton = OVRInput.Button.SecondaryThumbstickRight;
	[SerializeField] private OVRInput.Button prevItemButton = OVRInput.Button.SecondaryThumbstickLeft;

	private ISubject<int> choosenItemStream = new Subject<int>();
	public IObservable<int> ChoosenItemSubject { get { return choosenItemStream; } }

	private ButtonState thumbstick = ButtonState.Normal;
	private int activeItemIndex = 0;
	private bool active;

	public void Start()
	{
		foreach (var item in items)
			item.SetInactive();
		items[activeItemIndex].SetActive();
	}

	public override void SetChoosen()
	{
		base.SetChoosen();
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
		choosenItemStream.OnNext(activeItemIndex);
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
