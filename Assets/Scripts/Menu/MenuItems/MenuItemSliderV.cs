using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;

public class MenuItemSliderV : MenuItemV
{
	[Header("Slider")]
	[SerializeField] private Slider slider;
	[SerializeField] float sliderStep = 0.01f;

	[Header("Input")]
	[SerializeField] private OVRInput.Button decreaseValueButton = OVRInput.Button.SecondaryThumbstickLeft;
	[SerializeField] private OVRInput.Button increaseButton = OVRInput.Button.SecondaryThumbstickRight;

	protected ISubject<float> valueChangedSubject = new Subject<float>();
	public IObservable<float> ValueChangedStream { get { return valueChangedSubject; } }

	private bool active;

	private void Start()
	{
		ResetItem();
	}

	public void ResetItem()
	{
		slider.value = 0.2f;
		SliderValueChanged();
		active = false;
	}

	public override void SetChosen()
	{
		base.SetChosen();
		active = true;
	}

	public override void SetNormal()
	{
		SliderValueChanged();
		base.SetNormal();
		active = false;
	}

	// TODO not in update but coroutines
	void Update()
	{
		if (active)
		{
			if (OVRInput.Get(decreaseValueButton))
			{
				slider.value -= sliderStep;
			}
			else if (OVRInput.Get(increaseButton))
			{
				slider.value += sliderStep;
			}
		}
	}

	private void SliderValueChanged()
	{
		valueChangedSubject.OnNext(slider.value);
	}
}
