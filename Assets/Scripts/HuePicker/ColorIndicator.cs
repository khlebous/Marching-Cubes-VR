using UnityEngine;

public class ColorIndicator : MonoBehaviour
{
	HSBColor color;
	[SerializeField] private ColorHuePicker colorHuePicker;
	[SerializeField] private ColorSaturationBrightnessPicker colorSaturationBrightnessPicker;

	void Start()
	{
		color = HSBColor.FromColor(GetComponent<Renderer>().sharedMaterial.GetColor("_Color"));

		colorHuePicker.SetColor(color);
		colorSaturationBrightnessPicker.SetColor(color);
	}

	void ApplyColor()
	{
		GetComponent<Renderer>().sharedMaterial.SetColor("_Color", color.ToColor());
		//transform.parent.BroadcastMessage("OnColorChange", color, SendMessageOptions.DontRequireReceiver);
	}

	public void SetHue(float hue)
	{
		color.h = hue;
		ApplyColor();
	}

	public void SetSaturationBrightness(Vector2 sb)
	{
		color.s = sb.x;
		color.b = sb.y;
		ApplyColor();
	}

	public Color GetColor()
	{
		return HSBColor.ToColor(color);
	}
}
