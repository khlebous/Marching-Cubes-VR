using System.Collections;
using UnityEngine;

public class MovementWithOculusTouch : MonoBehaviour
{
	[Header("Oculus Touch input")]
	[Tooltip("Button to move")]
	[SerializeField] private OVRInput.Button buttonY = OVRInput.Button.Four;

	[Header("Rotatation multipliers")]
	[SerializeField] private int speed = 500;
	[SerializeField] private int bound = 20;

	private new Transform transform;
	private Vector3 startPosA;

	private Coroutine buttonA_down;
	private Coroutine buttonA_up;

	void Start ()
	{
		transform = GetComponent<Transform>();
		startPosA = Vector3.zero;

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
				startPosA = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);

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
			//Debug.Log("qwe");

			Vector3 currentPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
			Vector3 diff = startPosA - currentPos;
			startPosA = currentPos;
			transform.Translate(-diff*speed);

			if (OVRInput.GetUp(buttonY))
			{
				StopCoroutine(buttonA_up);

				buttonA_down = StartCoroutine(WaitForButtonA_Down());
			}

			yield return new WaitForEndOfFrame();
		}
	}
}
