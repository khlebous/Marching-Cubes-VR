using System.Collections;
using UnityEngine;

public class RotationWithOculusTouch : MonoBehaviour
{
    [Header("Oculus Touch input")]
    [Tooltip("Button to rotate")]
    [SerializeField] private OVRInput.Button buttonY = OVRInput.Button.Three;
    [Tooltip("Controller to follow")]
    private Transform controllerToFollow;

    [Header("Rotatation multipliers")]
    [SerializeField] private int bound = 10;

    private new Transform transform;
    private Vector3 startControllerPosition;
    private Vector3 startObjRotation;

    private Coroutine button_down;
    private Coroutine button_up;

    private void Start()
    {
        transform = GetComponent<Transform>();
        startControllerPosition = Vector3.zero;
        startObjRotation = Vector3.zero;
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
                startControllerPosition = controllerToFollow.position;
                startObjRotation = transform.rotation.eulerAngles;

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
            var rotation = RotationHelper.GetRotation(transform.position, startObjRotation, startControllerPosition, currentPos);
            transform.rotation = Quaternion.Euler(rotation);

            if (OVRInput.GetUp(buttonY))
            {
                StopCoroutine(button_up);

                button_down = StartCoroutine(WaitForButton_Down());
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
