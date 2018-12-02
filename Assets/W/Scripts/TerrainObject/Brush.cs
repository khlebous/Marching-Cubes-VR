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

		void Start()
		{
			StartListening();
		}

		private Coroutine buttonA_down;
		private Coroutine buttonA_up;

		private Coroutine buttonB_down;
		private Coroutine buttonB_up;

		private void StartListening()
		{
			buttonA_down = StartCoroutine(WaitForButtonA_Down());
			buttonB_down = StartCoroutine(WaitForButtonB_Down());
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

		private IEnumerator WaitForButtonA_Down()
		{
			while (true)
			{
				if (OVRInput.GetDown(buttonX))
				{
					StopCoroutine(buttonA_down);
					mode = BrushMode.Create;
					buttonA_up = StartCoroutine(WaitForButtonA_Up());
				}

				yield return new WaitForEndOfFrame();
			}
		}

		private IEnumerator WaitForButtonA_Up()
		{
			while (true)
			{
				if (OVRInput.GetUp(buttonX))
				{
					StopCoroutine(buttonA_up);
					mode = BrushMode.Inactive;
					buttonA_down = StartCoroutine(WaitForButtonA_Down());
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
