using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuItemColorV : MenuItemV
{
	private bool colorIsChoosing = true;

	private buttonState thumbstick = buttonState.Normal;
	[SerializeField] private bool active;
	[SerializeField] Draggable color;
	//[SerializeField] Draggable intensity;

	public void Start()
	{
		active = false;
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
				color.Input(buttonState.Left);
				Debug.Log("Left");

			}
			else if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight))
			{
				color.Input(buttonState.Right);
				Debug.Log("Right");
			}
			else
			{
				if (thumbstick != buttonState.Normal)
					thumbstick = buttonState.Normal;
			}
		}
	}

}
