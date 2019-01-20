using System.Collections;
using UnityEngine;

public class RotationWithOculusTouch : MonoBehaviour
{
	[Header("Oculus Touch input")]
	[Tooltip("Button to rotate")]
	[SerializeField]
	private OVRInput.RawButton buttonY = OVRInput.RawButton.X;
	[Tooltip("Controller to follow")]
	[SerializeField]
	private Transform controllerToFollow;

	[Header("Rotatation multipliers")]
	[SerializeField]
	private float speed = 10;

	private new Transform transform;
	private Vector3 lastControllerPosition;

	private Coroutine button_down;
	private Coroutine button_up;

	private void Start()
	{
		transform = GetComponent<Transform>();
		lastControllerPosition = Vector3.zero;
	}

	private void StartListening()
	{
		button_down = StartCoroutine(WaitForButton_Down());
	}

	private void StopListening()
	{
		if (null != button_down)
			StopCoroutine(button_down);

		if (null != button_up)
			StopCoroutine(button_up);
	}

	private IEnumerator WaitForButton_Down()
	{
		while (true)
		{
			if (OVRInput.GetDown(buttonY))
			{
				lastControllerPosition = controllerToFollow.position;
				StopCoroutine(button_down);
				button_up = StartCoroutine(WaitForButton_Up());
			}

			yield return new WaitForEndOfFrame();
		}
	}

	private IEnumerator WaitForButton_Up()
	{
		while (true)
		{
			var virtualObj = controllerToFollow.transform.position + (this.transform.position - controllerToFollow.transform.position).normalized / speed;

			var currToObj = virtualObj - controllerToFollow.transform.position;
			currToObj.y = 0;

			var lastToObj = virtualObj - lastControllerPosition;
			lastToObj.y = 0;

			var rotationChange = Quaternion.FromToRotation(lastToObj, currToObj);
			transform.rotation = rotationChange * transform.rotation;

			lastControllerPosition = controllerToFollow.transform.position;

			if (OVRInput.GetUp(buttonY))
			{
				StopCoroutine(button_up);

				button_down = StartCoroutine(WaitForButton_Down());
			}

			yield return new WaitForEndOfFrame();
		}
	}


	public void SetControllerToFollow(Transform toFollow)
	{
		controllerToFollow = toFollow;
	}

	private void OnEnable()
	{
		StartListening();
	}

	private void OnDisable()
	{
		StopListening();
	}
}
