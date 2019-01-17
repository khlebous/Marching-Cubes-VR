using UnityEngine;

public class Draggable : MonoBehaviour
{
	[SerializeField] private float minBound;
	[SerializeField] private float maxBound;

	[SerializeField] private bool fixX;
	[SerializeField] private bool fixY;

	[SerializeField] private Transform thumb;

	[SerializeField] private ColorHuePicker colorHuePicker;
	[SerializeField] private ColorSaturationBrightnessPicker colorSaturationBrightnessPicker;
	private bool dragging;

	public void Input(ButtonState state)
	{
		if (!fixX && !fixY)
		{
			if (state == ButtonState.Left)
			{
				if (thumb.localPosition.x > minBound)
					SetThumbPosition(thumb.localPosition - new Vector3(1, 0, 0));
				else
					SetThumbPosition(thumb.localPosition);
			}
			else if (state == ButtonState.Right)
			{
				if (thumb.localPosition.x < maxBound)
					SetThumbPosition(thumb.localPosition + new Vector3(1, 0, 0));
				else
					SetThumbPosition(thumb.localPosition);
			}
			else if (state == ButtonState.Down)
			{
				if (thumb.localPosition.y > minBound)
					SetThumbPosition(thumb.localPosition - new Vector3(0, 1, 0));
				else
					SetThumbPosition(thumb.localPosition);
			}
			else if (state == ButtonState.Up)
			{
				if (thumb.localPosition.y < maxBound)
					SetThumbPosition(thumb.localPosition + new Vector3(0, 1, 0));
				else
					SetThumbPosition(thumb.localPosition);
			}
		}
		else
		{
			if (state == ButtonState.Left)
			{
				if (thumb.localPosition.x > minBound)
					SetThumbPosition(thumb.localPosition - new Vector3(1, 0, 0));
				else
					SetThumbPosition(thumb.localPosition);
			}
			else if (state == ButtonState.Right)
			{
				if (thumb.localPosition.x < maxBound)
					SetThumbPosition(thumb.localPosition + new Vector3(1, 0, 0));
				else
					SetThumbPosition(thumb.localPosition);
			}
		}

	}

	public void SetDragPoint(Vector3 point)
	{
		point = (Vector3.one - point) * GetComponent<Collider>().bounds.size.x + GetComponent<Collider>().bounds.min;
		SetThumbPosition(point);
	}

	public void SetThumbPosition(Vector3 point)
	{
		thumb.localPosition = new Vector3(fixX ? thumb.localPosition.x : point.x, fixY ? thumb.localPosition.y : point.y, thumb.localPosition.z);
		var point2 = Vector3.one - (thumb.position - GetComponent<Collider>().bounds.min) / GetComponent<Collider>().bounds.size.x;

		if (!fixX && !fixY)
			colorSaturationBrightnessPicker.OnDrag(point2);
		else
			colorHuePicker.OnDrag(point2);
	}
}
