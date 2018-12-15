using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuItemColorV : MenuItemV
{
	private bool colorIsChoosing = true;

	private ButtonState thumbstick = ButtonState.Normal;
	[SerializeField] private bool active;
	[SerializeField] Draggable color;
	//[SerializeField] Draggable intensity;

	public void Start()
	{
		active = false;
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
				color.Input(ButtonState.Left);
				Debug.Log("Left");

			}
			else if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight))
			{
				color.Input(ButtonState.Right);
				Debug.Log("Right");
			}
			else
			{
				if (thumbstick != ButtonState.Normal)
					thumbstick = ButtonState.Normal;
			}
		}
	}

}
