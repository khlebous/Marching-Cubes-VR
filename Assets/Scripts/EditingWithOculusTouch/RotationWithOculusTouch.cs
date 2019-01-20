using System.Collections;
using UnityEngine;

public class RotationWithOculusTouch : MonoBehaviour
{
    [Header("Oculus Touch input")]
    [Tooltip("Button to rotate")]
    [SerializeField] private OVRInput.RawButton buttonY = OVRInput.RawButton.X;
    [Tooltip("Controller to follow")]
	[SerializeField] private Transform controllerToFollow;

    [Header("Rotatation multipliers")]
    [SerializeField] private int speed = 10;

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
            var toObjVector = this.transform.position - lastControllerPosition;
            var changePosVec = controllerToFollow.transform.position - lastControllerPosition;

            var distVec = Vector3.ProjectOnPlane(changePosVec, toObjVector);
            distVec.y = 0;

            var rotation = transform.rotation.eulerAngles;
            rotation.y += speed * distVec.magnitude;

            transform.rotation = Quaternion.Euler(rotation);

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
