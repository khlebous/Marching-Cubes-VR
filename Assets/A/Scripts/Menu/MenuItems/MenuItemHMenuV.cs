using System.Collections.Generic;
using UnityEngine;

public class MenuItemHMenuV : MenuItemV
{
	[SerializeField] public List<MenuItemV> items;

	private int activeItemIndex = 0;
	private buttonState thumbstick = buttonState.Normal;
	[SerializeField] private bool active;

	public void Start()
	{
		active = false;

		foreach (var item in items)
			item.SetInactive();
		items[activeItemIndex].SetActive();
	}

	public override void SetActive()
	{
		base.SetActive();
		active = true;
	}

	public override void SetInactive()
	{
		base.SetInactive();
		active = false;
	}

	void Update()
	{
		if (active)
		{

			if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft))
			{
				if (thumbstick == buttonState.Normal)
				{
					thumbstick = buttonState.Left;
					Debug.Log("Left");

					items[activeItemIndex].SetInactive();
					if (activeItemIndex == 0)
						activeItemIndex = items.Count;
					activeItemIndex--;
					activeItemIndex %= items.Count;
					items[activeItemIndex].SetActive();
				}

			}
			else if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight))
			{
				if (thumbstick == buttonState.Normal)
				{
					thumbstick = buttonState.Right;
					Debug.Log("Right");

					items[activeItemIndex].SetInactive();
					activeItemIndex++;
					activeItemIndex %= items.Count;
					items[activeItemIndex].SetActive();
				}
			}
			else
			{
				if (thumbstick != buttonState.Normal)
					thumbstick = buttonState.Normal;
			}
		}
	}
}

