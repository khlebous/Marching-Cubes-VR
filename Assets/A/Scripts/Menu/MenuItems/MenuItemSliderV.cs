using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuItemSliderV : MenuItemV
{
	[SerializeField] private Slider slider;

	private buttonState thumbstick = buttonState.Normal;

	[SerializeField] float sliderStep = 0.1f;
	[SerializeField] private bool active;

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
				if (thumbstick == buttonState.Normal)
				{
					thumbstick = buttonState.Left;
					Debug.Log("Left");

					slider.value -= sliderStep;
				}

			}
			else if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight))
			{
				if (thumbstick == buttonState.Normal)
				{
					thumbstick = buttonState.Right;
					Debug.Log("Right");

					slider.value += sliderStep;

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
