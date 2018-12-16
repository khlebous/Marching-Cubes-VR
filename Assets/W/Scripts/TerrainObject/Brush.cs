using System.Collections;
using UnityEngine;

namespace Assets.MarchingCubesGPU.Scripts
{

	public enum BrushMode
	{
		Color = 3,
		Remove = 2,
		Create = 1,
		Inactive = 0
	}

	public enum BrushShape
	{
		Sphere = 1,
		Cuboid = 2
	}

	public class Brush : MonoBehaviour
	{
		[SerializeField] private OVRInput.Button buttonX = OVRInput.Button.Three;
		[SerializeField] private OVRInput.Button buttonY = OVRInput.Button.Four;
		public Color color;

		public BrushMode mode = BrushMode.Inactive;
		public BrushShape shape = BrushShape.Sphere;

		public Matrix4x4 GetToBrushMatrix()
		{
			var brushPosition = Matrix4x4.Translate(-this.transform.position);
			var brushRotation = Matrix4x4.Rotate(Quaternion.Inverse(this.transform.rotation));
			var scale = Matrix4x4.Scale(this.transform.lossyScale).inverse;

			var result = scale * brushRotation * brushPosition;
			return result;
		}
		//public Matrix4x4 GetFromBrushMatrix()
		//{
		//	var scale = Matrix4x4.Scale(this.transform.lossyScale);
		//	var brushRotation = Matrix4x4.Rotate(this.transform.rotation);
		//	var brushPosition = Matrix4x4.Translate(this.transform.position);

		//	var result = brushPosition * brushRotation * scale;
		//	return result;
		//}

		private Coroutine buttonA_down;
		private Coroutine buttonA_up;

		private Coroutine buttonB_down;
		private Coroutine buttonB_up;

		private void Start()
		{
			StartListening(BrushMode.Create);
		}

		private void StartListening(BrushMode brushMode)
		{
			buttonA_down = StartCoroutine(WaitForButtonA_Down(brushMode));
			buttonB_down = StartCoroutine(WaitForButtonB_Down()); // automaticalu delele
		}

		private void SetChangeMode()
		{
			StopListening();
			mode = BrushMode.Inactive;
			StartListening(BrushMode.Create);
		}

		private void SetColorMode()
		{
			StopListening();
			mode = BrushMode.Inactive;
			buttonA_down = StartCoroutine(WaitForButtonA_Down(BrushMode.Color));
		}

		public void SetColor(Color color)
		{
			this.color = color;
		}

		public void SetMode(int newMode)
		{
			if (newMode == 0)
				SetChangeMode();
			else if (newMode == 1)
				SetColorMode();

			Debug.Log("new mode: " + newMode);
			
		}

		public void SetShape(int newShape)
		{
			if (newShape == 0)
				shape = BrushShape.Sphere;
			else if (newShape == 1)
				shape = BrushShape.Cuboid;

			Debug.Log("new brush shape: " + newShape);
		}

		public void SetSizeChanged(float newValue)
		{
			Debug.Log("new brush size: " + newValue);
		
			// TODO Wojtek
			// value jest w przedziale 0-1
		}

		private void StopListening()
		{
			if (null != buttonA_down)
				StopCoroutine(buttonA_down);
			if (null != buttonB_down)
				StopCoroutine(buttonB_down);

			if (null != buttonA_up)
				StopCoroutine(buttonA_up);
			if (null != buttonB_up)
				StopCoroutine(buttonB_up);
		}

		private IEnumerator WaitForButtonA_Down(BrushMode brushMode)
		{
			while (true)
			{
				if (OVRInput.GetDown(buttonX))
				{
					StopCoroutine(buttonA_down);
					mode = brushMode;
					buttonA_up = StartCoroutine(WaitForButtonA_Up(brushMode));
				}

				yield return new WaitForEndOfFrame();
			}
		}

		private IEnumerator WaitForButtonA_Up(BrushMode brushMode)
		{
			while (true)
			{
				if (OVRInput.GetUp(buttonX))
				{
					StopCoroutine(buttonA_up);
					mode = BrushMode.Inactive;
					buttonA_down = StartCoroutine(WaitForButtonA_Down(brushMode));
				}

				yield return new WaitForEndOfFrame();
			}
		}

		private IEnumerator WaitForButtonB_Down()
		{
			while (true)
			{
				if (OVRInput.GetDown(buttonY))
				{
					StopCoroutine(buttonB_down);
					mode = BrushMode.Remove;
					buttonB_up = StartCoroutine(WaitForButtonB_Up());
				}

				yield return new WaitForEndOfFrame();
			}
		}

		private IEnumerator WaitForButtonB_Up()
		{
			while (true)
			{
				if (OVRInput.GetUp(buttonY))
				{
					StopCoroutine(buttonB_up);
					mode = BrushMode.Inactive;
					buttonB_down = StartCoroutine(WaitForButtonB_Down());
				}

				yield return new WaitForEndOfFrame();
			}
		}
	}

}
