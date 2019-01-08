using UnityEngine;

public class ColorSaturationBrightnessPicker : MonoBehaviour
{
	public Material backgroundMaterial;

	[SerializeField] private ColorIndicator colorIndicator;

	[SerializeField] private Draggable draggable1;
	[SerializeField] private Draggable draggable2;

	public void SetColor(HSBColor color)
	{
		backgroundMaterial.SetColor("_Color", new HSBColor(color.h, 1, 1).ToColor());

		draggable1.SetDragPoint( new Vector3(color.s, color.b, 0));
		draggable2.SetDragPoint(new Vector3(color.s, color.b, 0));
	}

	public void OnDrag(Vector3 point)
	{
		colorIndicator.SetSaturationBrightness( new Vector2(point.x, point.y));
	}

	public void SetHue(float hue)
	{
		backgroundMaterial.SetColor("_Color", new HSBColor(hue, 1, 1).ToColor());
	}
}
