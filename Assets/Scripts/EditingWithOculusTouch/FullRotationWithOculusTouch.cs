using System.Collections;
using UnityEngine;

public class FullRotationWithOculusTouch : MonoBehaviour
{
	[Header("Oculus Touch input")]
	[Tooltip("Button to rotate")]
	[SerializeField]
	private OVRInput.RawButton buttonY = OVRInput.RawButton.X;
	[Tooltip("Controller to follow")]
	[SerializeField]
	private Transform controllerToFollow;

	private new Transform transform;
	private Quaternion startControllerInverseRotation;
	private Quaternion startObjRotation;

	private Coroutine button_down;
	private Coroutine button_up;

	private void Start()
	{
		transform = GetComponent<Transform>();
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
				startControllerInverseRotation = Quaternion.Inverse(controllerToFollow.rotation);
				startObjRotation = transform.rotation;

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
			var currentPos = controllerToFollow.position;
			transform.rotation = controllerToFollow.rotation * startControllerInverseRotation * startObjRotation;

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
