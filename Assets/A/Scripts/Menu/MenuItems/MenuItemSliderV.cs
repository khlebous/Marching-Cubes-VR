using UnityEngine;
using UnityEngine.UI;

public class MenuItemSliderV : MenuItemV
{
	[SerializeField] private Slider slider;

	private ButtonState thumbstick = ButtonState.Normal;

	[SerializeField] float sliderStep = 0.1f;
	[SerializeField] private bool active;

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
				if (thumbstick == ButtonState.Normal)
				{
					thumbstick = ButtonState.Left;
					Debug.Log("Left");

					slider.value -= sliderStep;
				}

			}
			else if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight))
			{
				if (thumbstick == ButtonState.Normal)
				{
					thumbstick = ButtonState.Right;
					Debug.Log("Right");

					slider.value += sliderStep;

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
