using UnityEngine;
using UniRx;

public class MenuItemColorV : MenuItemV
{
	[Header("Colors")]
	[SerializeField] ColorIndicator colorIndicator;
	[SerializeField] Draggable colorHuePicker;
	[SerializeField] Draggable intensity;

	protected ISubject<Color> colorChangedSubject = new Subject<Color>();
	public IObservable<Color> ColorChangedStream { get { return colorChangedSubject; } }

	private bool active;

	public void Start()
	{
		ResetItem();
	}

	public void ResetItem()
	{
		active = false;
		ColorChanged();
	}

	public override void SetChoosen()
	{
		base.SetChoosen();
		active = true;
	}

	public override void SetNormal()
	{
		ColorChanged();
		base.SetNormal();
		active = false;
	}

	void Update()
	{
		if (active)
		{
			if (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
			{
				colorHuePicker.Input(ButtonState.Right);
			}
			else if (OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
			{
				colorHuePicker.Input(ButtonState.Left);
			}
			else if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickLeft))
			{
				intensity.Input(ButtonState.Left);

			}
			else if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickRight))
			{
				intensity.Input(ButtonState.Right);
			}
			else if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickUp))
			{
				intensity.Input(ButtonState.Up);

			}
			else if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickDown))
			{
				intensity.Input(ButtonState.Down);
			}
		}
	}

	private void ColorChanged()
	{
		colorChangedSubject.OnNext(colorIndicator.GetColor());
	}
}
