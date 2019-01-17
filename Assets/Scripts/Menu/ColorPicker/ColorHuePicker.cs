using UnityEngine;

public class ColorHuePicker : MonoBehaviour
{
	[SerializeField] private ColorIndicator colorIndicator;
	[SerializeField] private ColorSaturationBrightnessPicker colorSaturationBrightnessPicker;

	[SerializeField] private Draggable draggable1;
	[SerializeField] private Draggable draggable2;

	public void SetColor(HSBColor color)
	{
		draggable1.SetDragPoint( new Vector3(color.h, 0, 0));
		draggable2.SetDragPoint( new Vector3(color.h, 0, 0));
	}	

    public void OnDrag(Vector3 point)
    {
		colorIndicator.SetHue(point.x);
		colorSaturationBrightnessPicker.SetHue(point.x);
    }
}
