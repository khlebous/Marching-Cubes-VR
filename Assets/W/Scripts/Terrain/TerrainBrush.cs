﻿using System;
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
		public Color color;
		public MarchingCubesGPUProject.EditableTerrain terrain;
		[SerializeField] private OVRInput.Button buttonY = OVRInput.Button.Two;

		public TerrainBrushMode mode = TerrainBrushMode.Change;
		public TerrainBrushShape shape = TerrainBrushShape.Wheel;

		private const float _minScale = McConsts.ModelN / 50f;
		private const float _maxScale = McConsts.ModelN / 5f;

		public Matrix4x4 GetToBrushMatrix(Vector3 position)
		{
			var brushPosition = Matrix4x4.Translate(-position);
			var brushRotation = Matrix4x4.Rotate(Quaternion.Inverse(this.transform.rotation));
			var scale = Matrix4x4.Scale(this.transform.lossyScale).inverse;

			var result = scale * brushRotation * brushPosition;
			return result;
		}
		//public Matrix4x4 GetFromBrushMatrix(Vector3 position)
		//{
		//    var scale = Matrix4x4.Scale(this.transform.lossyScale);
		//    var brushRotation = Matrix4x4.Rotate(this.transform.rotation);
		//    var brushPosition = Matrix4x4.Translate(position);

		//    var result = brushPosition * brushRotation * scale;
		//    return result;
		//}

		private Coroutine buttonA_down;
		private Coroutine buttonA_up;


		public void SetActive()
		{
			mode = TerrainBrushMode.Inactive;
			StartListening(TerrainBrushMode.Change);
		}

		public void SetInactive()
		{
			StopListening();
		}

		private void StartListening(TerrainBrushMode terrainMode)
		{
			buttonA_down = StartCoroutine(WaitForButtonA_Down(terrainMode));
		}

		private void StopListening()
		{
			if (null != buttonA_down)
				StopCoroutine(buttonA_down);

			if (null != buttonA_up)
				StopCoroutine(buttonA_up);
		}

		private IEnumerator WaitForButtonA_Down(TerrainBrushMode terrainMode)
		{
			while (true)
			{
				if (OVRInput.GetDown(buttonY, OVRInput.Controller.RTouch))
				{
					StopCoroutine(buttonA_down);
					mode = terrainMode; // TerrainBrushMode.Change;
					terrain.StartShaping();
					buttonA_up = StartCoroutine(WaitForButtonA_Up(terrainMode));
				}
				//else if (OVRInput.GetDown(buttonX))
				//{
				//	StopCoroutine(buttonA_down);
				//	mode = terrainMode; // TerrainBrushMode.ExtremeChange;
				//	terrain.StartShaping();
				//	buttonA_up = StartCoroutine(WaitForButtonA_Up(buttonX));
				//}
				yield return new WaitForEndOfFrame();
			}
		}

		private IEnumerator WaitForButtonA_Up(TerrainBrushMode terrainMode)
		{
			while (true)
			{
				if (OVRInput.GetUp(buttonY))
				{
					StopCoroutine(buttonA_up);
					mode = TerrainBrushMode.Inactive;
					terrain.FinishShaping();

					buttonA_down = StartCoroutine(WaitForButtonA_Down(terrainMode));
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
			buttonA_down = StartCoroutine(WaitForButtonA_Down(TerrainBrushMode.Color));
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

		public void SetModificationType(int modificationType)
		{
			//TODO separate shaping mode and mode
			if (modificationType == 0)
			{
				StopListening();
				mode = TerrainBrushMode.Inactive;
				StartListening(TerrainBrushMode.Change);
			}
			else if (modificationType == 1)
			{
				StopListening();
				mode = TerrainBrushMode.Inactive;
				StartListening(TerrainBrushMode.ExtremeChange);
			}
		}


		public void SetSizeChanged(float newValue)
		{
			var scale = newValue * (_maxScale - _minScale) + _minScale;
			this.transform.localScale = scale * Vector3.one;
		}

	}
}
