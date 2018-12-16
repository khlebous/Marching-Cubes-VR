using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuItemColorV : MenuItemV
{
	private bool colorIsChoosing = true;

	private ButtonState thumbstick = ButtonState.Normal;
	[SerializeField] private bool active;
	[SerializeField] Draggable colorHuePicker;
	
	// TODO 
	// [SerializeField] Draggable intensity;

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
				colorHuePicker.Input(ButtonState.Left);
				Debug.Log("Left");

			}
			else if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight))
			{
				colorHuePicker.Input(ButtonState.Right);
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
