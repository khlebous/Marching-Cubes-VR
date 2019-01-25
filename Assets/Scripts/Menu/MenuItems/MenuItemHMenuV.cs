using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class MenuItemHMenuV : MenuItemV
{
	[SerializeField] private List<MenuItemV> items;

	[Header("Input")]
	[SerializeField] private OVRInput.RawButton nextItemButton = OVRInput.RawButton.RThumbstickRight;
	[SerializeField] private OVRInput.RawButton prevItemButton = OVRInput.RawButton.RThumbstickLeft;

	private ISubject<int> chosenItemStream = new Subject<int>();
	public IObservable<int> ChosenItemSubject { get { return chosenItemStream; } }

	private int activeItemIndex = 0;
	private bool active;

	public void Awake()
	{
		ResetItem();
	}

	public void ResetItem()
	{
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
			if (OVRInput.GetDown(prevItemButton))
			{
				items[activeItemIndex].SetInactive();
				DecreaseActiveItemIndex();
				ModeChanged();
				items[activeItemIndex].SetActive();

			}
			else if (OVRInput.GetDown(nextItemButton))
			{
				items[activeItemIndex].SetInactive();
				IncreaseActiveItemIndex();
				ModeChanged();
				items[activeItemIndex].SetActive();
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
