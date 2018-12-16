using UnityEngine;
using UniRx;

public class MenuItemColorV : MenuItemV
{
	[SerializeField] Draggable colorHuePicker;
	[SerializeField] ColorIndicator colorIndicator;
	// TODO [SerializeField] Draggable intensity;

	[Header("Input")]
	[SerializeField] private OVRInput.Button decreaseValueButton = OVRInput.Button.SecondaryThumbstickLeft;
	[SerializeField] private OVRInput.Button increaseValueButton = OVRInput.Button.SecondaryThumbstickRight;

	protected ISubject<Color> colorChangedSubject = new Subject<Color>();
	public IObservable<Color> ColorChangedStream { get { return colorChangedSubject; } }

	private bool colorIsChoosing = true;

	private ButtonState thumbstick = ButtonState.Normal;
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

	public override void SetUnChoosen()
	{
		base.SetUnChoosen();
		active = false;
	}

	void Update()
	{
		if (active)
		{
			if (OVRInput.Get(decreaseValueButton))
			{
				colorHuePicker.Input(ButtonState.Left);
				ColorChanged();

			}
			else if (OVRInput.Get(increaseValueButton))
			{
				colorHuePicker.Input(ButtonState.Right);
				ColorChanged();
			}
		}
	}

	private void ColorChanged()
	{
		colorChangedSubject.OnNext(colorIndicator.GetColor());
	}

}
