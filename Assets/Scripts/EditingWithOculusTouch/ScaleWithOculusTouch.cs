using System.Collections;
using UnityEngine;

public class ScaleWithOculusTouch : MonoBehaviour
{
    [Header("Oculus Touch input")]
    [Tooltip("Button to scale")]
    [SerializeField] private OVRInput.RawButton buttonY = OVRInput.RawButton.Y;
    [Tooltip("Controller to follow")]
    private Transform controllerToFollow;

    [Header("Scale multipliers")]
    [SerializeField] private float multiplier = 2f;

    private new Transform transform;
    private Vector3 LastControllerPosition;

    private Coroutine button_down;
    private Coroutine button_up;

    private void Start()
    {
        transform = GetComponent<Transform>();
        LastControllerPosition = Vector3.zero;
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
                LastControllerPosition = controllerToFollow.position;
				StopListening();
                button_up = StartCoroutine(WaitForButton_Up());
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator WaitForButton_Up()
    {
        while (true)
        {
            var startDist = Vector3.Distance(transform.position, LastControllerPosition);
            var currtDist = Vector3.Distance(transform.position, controllerToFollow.transform.position);

            transform.localScale += Vector3.one * multiplier * (currtDist - startDist);
			LastControllerPosition = controllerToFollow.position;

			if (OVRInput.GetUp(buttonY))
            {
				StopListening();
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
