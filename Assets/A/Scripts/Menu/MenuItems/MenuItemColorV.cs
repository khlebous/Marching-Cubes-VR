using UnityEngine;
using UniRx;

public class MenuItemColorV : MenuItemV
{
	[Header("Colors")]
	[SerializeField] ColorIndicator colorIndicator;
	[SerializeField] Draggable colorHuePicker;
	[SerializeField] Draggable intensity;

	[Header("Input")]
	[SerializeField] private OVRInput.Button decreaseValueButton = OVRInput.Button.SecondaryThumbstickLeft;
	[SerializeField] private OVRInput.Button increaseValueButton = OVRInput.Button.SecondaryThumbstickRight;
	[SerializeField] private OVRInput.Controller controller = OVRInput.Controller.RTouch;

	protected ISubject<Color> colorChangedSubject = new Subject<Color>();
	public IObservable<Color> ColorChangedStream { get { return colorChangedSubject; } }

	private bool active;

	public void Start()
	{
		active = false;
	}

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

	void Update()
	{
		if (active)
		{
			if (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
			{
				colorHuePicker.Input(ButtonState.Right);
				ColorChanged();

			}
			else if (OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
			{
				colorHuePicker.Input(ButtonState.Left);
				ColorChanged();
			}
			else if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickLeft))
			{
				intensity.Input(ButtonState.Left);
				ColorChanged();

			}
			else if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickRight))
			{
				intensity.Input(ButtonState.Right);
				ColorChanged();
			}
			else if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickUp))
			{
				intensity.Input(ButtonState.Up);
				ColorChanged();

			}
			else if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickDown))
			{
				intensity.Input(ButtonState.Down);
				ColorChanged();
			}
		}
	}

	private void ColorChanged()
	{
		colorChangedSubject.OnNext(colorIndicator.GetColor());
	}
}
