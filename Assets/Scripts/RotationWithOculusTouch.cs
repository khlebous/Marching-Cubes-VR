using System.Collections;
using UnityEngine;

public class RotationWithOculusTouch : MonoBehaviour
{
	[Header("Oculus Touch input")]
	[Tooltip("Button to rotate around Y axis")]
	[SerializeField] private OVRInput.Button buttonY = OVRInput.Button.One;
	[Tooltip("Button to rotate around X and Z axis")]
	[SerializeField] private OVRInput.Button buttonXZ = OVRInput.Button.Two;
	[Tooltip("Controller to follow")]
	[SerializeField] private OVRInput.Controller controllerToFollow = OVRInput.Controller.RTouch;

	[Header("Rotatation multipliers")]
	[SerializeField] private int speed = 500;
	[SerializeField] private int bound = 20;

	private new Transform transform;
	private Vector3 startPosA;
	private Vector3 startPosB;

	private Coroutine buttonA_down;
	private Coroutine buttonB_down;
	private Coroutine buttonA_up;
	private Coroutine buttonB_up;

	private void Start()
	{
		transform = GetComponent<Transform>();
		startPosA = Vector3.zero;
		startPosB = Vector3.zero;

		StartListening();
	}

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
			if (OVRInput.GetDown(buttonY))
			{
				startPosA = OVRInput.GetLocalControllerPosition(controllerToFollow);

				StopCoroutine(buttonA_down);
				buttonA_up = StartCoroutine(WaitForButtonA_Up());
			}

			yield return new WaitForEndOfFrame();
		}
	}

	private IEnumerator WaitForButtonA_Up()
	{
		while (true)
		{
			Vector3 currentPos = OVRInput.GetLocalControllerPosition(controllerToFollow);
			Vector3 diff = startPosA - currentPos;
			startPosA = currentPos;
			transform.Rotate(new Vector3(0, diff.x * speed, 0), Space.World);

			if (OVRInput.GetUp(buttonY))
			{
				StopCoroutine(buttonA_up);

				buttonA_down = StartCoroutine(WaitForButtonA_Down());
			}

			yield return new WaitForEndOfFrame();
		}
	}

	private IEnumerator WaitForButtonB_Down()
	{
		while (true)
		{
			if (OVRInput.GetDown(buttonXZ))
			{
				startPosB = OVRInput.GetLocalControllerPosition(controllerToFollow);

				StopCoroutine(buttonB_down);
				buttonB_up = StartCoroutine(WaitForButtonB_Up());
			}

			yield return new WaitForEndOfFrame();
		}
	}

	private IEnumerator WaitForButtonB_Up()
	{
		while (true)
		{
			Vector3 currentPos = OVRInput.GetLocalControllerPosition(controllerToFollow);
			Vector3 diff = startPosB - currentPos;
			startPosB = currentPos;
			transform.Rotate(new Vector3(-speed * diff.y, 0, 0), Space.World);

			CorrectAnglesIfNeeded();

			if (OVRInput.GetUp(buttonXZ))
			{
				StopCoroutine(buttonB_up);

				buttonB_down = StartCoroutine(WaitForButtonB_Down());
			}

			yield return new WaitForEndOfFrame();
		}
	}

	private void CorrectAnglesIfNeeded()
	{
		float eulerAnglesX = transform.eulerAngles.x;
		if (CheckAngleWithinBounds(eulerAnglesX))
		{
			float newEulerAngleX = CalculateNearestAngleWithinBounds(eulerAnglesX);
			transform.eulerAngles = new Vector3(newEulerAngleX, transform.eulerAngles.y, transform.eulerAngles.z);
		}

		float eulerAnglesZ = transform.eulerAngles.z;
		if (CheckAngleWithinBounds(eulerAnglesZ))
		{
			float newEulerAngleZ = CalculateNearestAngleWithinBounds(eulerAnglesZ);
			transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, newEulerAngleZ);
		}
	}

	private bool CheckAngleWithinBounds(float angle)
	{
		return angle > bound && angle < 360 - bound;
	}

	private float CalculateNearestAngleWithinBounds(float angle)
	{
		float leftDistance = angle - bound;
		float rightDistance = 360 - angle - bound;
		return leftDistance > rightDistance ? 360 - bound : bound;
	}
}
