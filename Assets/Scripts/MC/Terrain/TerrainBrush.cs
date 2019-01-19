using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.MarchingCubesGPU.Scripts
{
	public enum TerrainBrushMode
	{
		ExtremeChange = 4,
		Color = 3,
		Change = 2,
		Inactive = 0
	}

	public enum TerrainBrushShape
	{
		Wheel = 1,
		Rectangle = 2
	}

	public class TerrainBrush : MonoBehaviour
	{
		public MarchingCubesGPUProject.EditableTerrain terrain;
		public Color color;
		public TerrainBrushMode mode = TerrainBrushMode.Change;
		public TerrainBrushShape shape = TerrainBrushShape.Wheel;

		private OVRInput.RawButton buttonB = OVRInput.RawButton.B;
		private OVRInput.RawButton buttonA = OVRInput.RawButton.A;

		private const float _minScale = 1;
		private const float _maxScale = McConsts.ModelN / 2f;

        public Transform cylinderMesh;

		public Matrix4x4 GetToBrushMatrix(Vector3 position)
		{
			var brushPosition = Matrix4x4.Translate(-position);
			var brushRotation = Matrix4x4.Rotate(Quaternion.Inverse(this.transform.rotation));
			var scale = Matrix4x4.Scale(this.transform.lossyScale).inverse;

			var result = scale * brushRotation * brushPosition;
			return result;
		}

		private Coroutine buttonB_down;
		private Coroutine buttonB_up;

		private Coroutine buttonA_down;
		private Coroutine buttonA_up;

		public void SetActive()
		{
			mode = TerrainBrushMode.Inactive;
            this.gameObject.SetActive(true);
			StartListening(TerrainBrushMode.Change);
        }

		public void SetInactive()
		{
			StopListening();
            this.gameObject.SetActive(false);
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

		private void StartListening(TerrainBrushMode terrainMode)
		{
			buttonB_down = StartCoroutine(WaitForButtonB_Down(terrainMode));
			if (terrainMode != TerrainBrushMode.Color)
				buttonA_down = StartCoroutine(WaitForButtonA_Down()); // automaticaly extreme change
		}

		private IEnumerator WaitForButtonB_Down(TerrainBrushMode terrainMode)
		{
			while (true)
			{
				if (OVRInput.GetDown(buttonB))
				{
					StopCoroutine(buttonB_down);
					mode = terrainMode;
					terrain.StartShaping();
					buttonB_up = StartCoroutine(WaitForButtonB_Up(terrainMode));
				}

				yield return new WaitForEndOfFrame();
			}
		}

		private IEnumerator WaitForButtonB_Up(TerrainBrushMode terrainMode)
		{
			while (true)
			{
				if (OVRInput.GetUp(buttonB))
				{
					StopCoroutine(buttonB_up);
					mode = TerrainBrushMode.Inactive;
					terrain.FinishShaping();
					buttonB_down = StartCoroutine(WaitForButtonB_Down(terrainMode));
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
					mode = TerrainBrushMode.ExtremeChange;
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
					mode = TerrainBrushMode.Inactive;
					buttonA_down = StartCoroutine(WaitForButtonA_Down());
				}

				yield return new WaitForEndOfFrame();
			}
		}

		private void SetChangeMode()
		{
			StopListening();
			mode = TerrainBrushMode.Inactive;
			StartListening(TerrainBrushMode.Change);
		}

		private void SetColorMode()
		{
			StopListening();
			mode = TerrainBrushMode.Inactive;
			buttonB_down = StartCoroutine(WaitForButtonB_Down(TerrainBrushMode.Color));
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
				shape = TerrainBrushShape.Wheel;
			else if (newShape == 1)
				shape = TerrainBrushShape.Rectangle;
		}

		public void SetSizeChanged(float newValue)
		{
			var scale = newValue * (_maxScale - _minScale) + _minScale;
			this.transform.localScale = scale * Vector3.one;
		}
	}
}
