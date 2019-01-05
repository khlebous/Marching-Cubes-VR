using UnityEngine;
using UnityEngine.UI;
using UniRx;

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

	// TODO not in update but coroutines
	void Update()
	{
		if (active)
		{
			if (OVRInput.Get(decreaseValueButton))
			{
				slider.value -= sliderStep;
				SliderValueChanged();
			}
			else if (OVRInput.Get(increaseButton))
			{
				slider.value += sliderStep;
				SliderValueChanged();
			}
		}
	}

	private void SliderValueChanged()
	{
		valueChangedSubject.OnNext(slider.value);
	}

	private void Start()
	{
		valueChangedSubject.OnNext(slider.value);
	}
}
