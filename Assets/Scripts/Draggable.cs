using UnityEngine;

public class Draggable : MonoBehaviour
{
	public bool fixX;
	public bool fixY;
	public Transform thumb;
	bool dragging;

	public void Input(ButtonState state)
	{
		Debug.Log("deaggable get input: " + state);
		if (state == ButtonState.Left)
		{
			if (thumb.localPosition.x > -100)
				SetThumbPosition(thumb.localPosition - new Vector3(1, 0, 0));
			else
				SetThumbPosition(thumb.localPosition);
		}
		else if (state == ButtonState.Right)
		{
			if (thumb.localPosition.x < 100)
				SetThumbPosition(thumb.localPosition + new Vector3(1, 0, 0));
			else
				SetThumbPosition(thumb.localPosition);
		}
	}

	// TODO remove
	void SetDragPoint(Vector3 point)
	{
		point = (Vector3.one - point) * GetComponent<Collider>().bounds.size.x + GetComponent<Collider>().bounds.min;
		SetThumbPosition(point);
	}

	void SetThumbPosition(Vector3 point)
	{
		thumb.localPosition = new Vector3(fixX ? thumb.localPosition.x : point.x, fixY ? thumb.localPosition.y : point.y, thumb.localPosition.z);
		SendMessage("OnDrag", Vector3.one - (thumb.position - GetComponent<Collider>().bounds.min) / GetComponent<Collider>().bounds.size.x);
	}

}
