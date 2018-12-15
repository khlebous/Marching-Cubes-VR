using System.Collections.Generic;
using UnityEngine;

public class MenuItemHMenuV : MenuItemV
{
	[SerializeField] private List<MenuItemV> items;
	[SerializeField] private Assets.MarchingCubesGPU.Scripts.Brush brush;

	private int activeItemIndex = 0;
	private ButtonState thumbstick = ButtonState.Normal;
	[SerializeField] private bool active;

	public void Start()
	{
		active = false;

		foreach (var item in items)
			item.SetInactive();
		items[activeItemIndex].SetActive();
	}

	public override void SetChoosen()
	{
		base.SetChoosen();
		active = true;
	}

	public override void SetUnChoosen()
	{
		base.SetUnChoosen();
		active = false;
	}

	void Update()
	{
		if (active)
		{

			if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft))
			{
				if (thumbstick == ButtonState.Normal)
				{
					thumbstick = ButtonState.Left;
					Debug.Log("Left");

					items[activeItemIndex].SetInactive();
					if (activeItemIndex == 0)
						activeItemIndex = items.Count;
					activeItemIndex--;
					activeItemIndex %= items.Count;
					items[activeItemIndex].SetActive();
					if (activeItemIndex == 0)
						brush.SetChangeMode();
					else
						brush.SetColorMode();
				}

			}
			else if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight))
			{
				if (thumbstick == ButtonState.Normal)
				{
					thumbstick = ButtonState.Right;
					Debug.Log("Right");

					items[activeItemIndex].SetInactive();
					activeItemIndex++;
					activeItemIndex %= items.Count;
					items[activeItemIndex].SetActive();
					if (activeItemIndex == 0)
						brush.SetChangeMode();
					else
						brush.SetColorMode();
				}
			}
			else
			{
				if (thumbstick != ButtonState.Normal)
					thumbstick = ButtonState.Normal;
			}
		}
	}
}

