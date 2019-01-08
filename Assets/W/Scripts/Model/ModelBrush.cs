﻿using System.Collections;
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

	public class ModelBrush : MonoBehaviour
	{
		[Header("Input")]
		[SerializeField] private OVRInput.Button buttonB = OVRInput.Button.Two;
		[SerializeField] private OVRInput.Button buttonA = OVRInput.Button.One;
		[SerializeField] private OVRInput.Controller controller = OVRInput.Controller.RTouch;

		[Header("Other")]
		public Color color;

		public BrushMode mode = BrushMode.Inactive;
		public BrushShape shape = BrushShape.Sphere;

		private const float _minScale = McConsts.ModelN / 400f;
		private const float _maxScale = McConsts.ModelN / 25f;

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


		// TODO A remove comments (ja to zrobie jak sprawdze ze dziala -A)
		private Coroutine buttonA_down;
		private Coroutine buttonA_up;

		private Coroutine buttonB_down;
		private Coroutine buttonB_up;

		public void SetActive()
		{
			mode = BrushMode.Inactive;
			StartListening(BrushMode.Create);
		}

		public void SetInactive()
		{
			StopListening();
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

		private void StartListening(BrushMode brushMode)
		{
			buttonA_down = StartCoroutine(WaitForButtonB_Down(brushMode));
			if (brushMode != BrushMode.Color)
				buttonB_down = StartCoroutine(WaitForButtonA_Down()); // automaticaly delele
		}

		private IEnumerator WaitForButtonB_Down(BrushMode brushMode)
		{
			while (true)
			{
				if (OVRInput.GetDown(buttonB, controller))
				{
					StopCoroutine(buttonB_down);
					mode = brushMode;
					buttonB_up = StartCoroutine(WaitForButtonB_Up(brushMode));
				}

				yield return new WaitForEndOfFrame();
			}
		}

		private IEnumerator WaitForButtonB_Up(BrushMode brushMode)
		{
			while (true)
			{
				if (OVRInput.GetUp(buttonB))
				{
					StopCoroutine(buttonB_up);
					mode = BrushMode.Inactive;
					buttonB_down = StartCoroutine(WaitForButtonB_Down(brushMode));
				}

				yield return new WaitForEndOfFrame();
			}
		}

		private IEnumerator WaitForButtonA_Down()
		{
			while (true)
			{
				if (OVRInput.GetDown(buttonA))
				{
					StopCoroutine(buttonA_down);
					mode = BrushMode.Remove;
					buttonA_up = StartCoroutine(WaitForButtonA_Up());
				}

				yield return new WaitForEndOfFrame();
			}
		}

		private IEnumerator WaitForButtonA_Up()
		{
			while (true)
			{
				if (OVRInput.GetUp(buttonA))
				{
					StopCoroutine(buttonA_up);
					mode = BrushMode.Inactive;
					buttonA_down = StartCoroutine(WaitForButtonA_Down());
				}

				yield return new WaitForEndOfFrame();
			}
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
			buttonA_down = StartCoroutine(WaitForButtonB_Down(BrushMode.Color));
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
		}

		public void SetShape(int newShape)
		{
			if (newShape == 0)
				shape = BrushShape.Sphere;
			else if (newShape == 1)
				shape = BrushShape.Cuboid;
		}

		public void SetSizeChanged(float newValue)
		{
			var scale = newValue * (_maxScale - _minScale) + _minScale;
			this.transform.localScale = scale * Vector3.one;
		}
	}
}
