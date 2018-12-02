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
		[SerializeField] private MarchingCubesGPUProject.Terrain terrain;
		[SerializeField] private OVRInput.Button buttonY = OVRInput.Button.Four;

		public TerrainBrushMode mode = TerrainBrushMode.Change;
        public TerrainBrushShape shape = TerrainBrushShape.Wheel;
		
        public Matrix4x4 GetToBrushMatrix(Vector3 position)
        {
            var brushPosition = Matrix4x4.Translate(-position);
            var brushRotation = Matrix4x4.Rotate(Quaternion.Inverse(this.transform.rotation));
            var scale = Matrix4x4.Scale(this.transform.lossyScale).inverse;

            var result = scale * brushRotation * brushPosition;
            return result;
        }
        public Matrix4x4 GetFromBrushMatrix(Vector3 position)
        {
            var scale = Matrix4x4.Scale(this.transform.lossyScale);
            var brushRotation = Matrix4x4.Rotate(this.transform.rotation);
            var brushPosition = Matrix4x4.Translate(position);

            var result = brushPosition * brushRotation * scale;
            return result;
        }

		private Coroutine buttonA_down;
		private Coroutine buttonA_up;

		void Start()
		{
			StartListening();
		}

		private void StartListening()
		{
			buttonA_down = StartCoroutine(WaitForButtonA_Down());
		}

		private void StopListening()
		{
			if (null != buttonA_down)
				StopCoroutine(buttonA_down);

			if (null != buttonA_up)
				StopCoroutine(buttonA_up);
		}

		private IEnumerator WaitForButtonA_Down()
		{
			while (true)
			{
				if (OVRInput.GetDown(buttonY))
				{
					StopCoroutine(buttonA_down);
					Debug.Log("change");
					mode = TerrainBrushMode.Change;
					terrain.StartShaping();
					buttonA_up = StartCoroutine(WaitForButtonA_Up());
				}

				yield return new WaitForEndOfFrame();
			}
		}

		private IEnumerator WaitForButtonA_Up()
		{
			while (true)
			{
				if (OVRInput.GetUp(buttonY))
				{
					StopCoroutine(buttonA_up);
					Debug.Log("inactive");
					mode = TerrainBrushMode.Inactive;
					terrain.FinishShaping();
					buttonA_down = StartCoroutine(WaitForButtonA_Down());
				}

				yield return new WaitForEndOfFrame();
			}
		}
	}   
}
