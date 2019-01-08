using System.Collections;
using UnityEngine;

public class MovementWithOculusTouch : MonoBehaviour
{
    [Header("Oculus Touch input")]
    [Tooltip("Button to move")]
    [SerializeField] private OVRInput.Button buttonY = OVRInput.Button.Start;
    [Tooltip("Controller to follow")]
    [SerializeField] private Transform controllerToFollow;

    [Header("Rotatation multipliers")]
    [SerializeField] private int speed = 10;
    [SerializeField] private int bound = 20;

    private new Transform transform;
    private Vector3 startPos;

    private Coroutine button_down;
    private Coroutine button_up;

    void Start()
    {
        transform = GetComponent<Transform>();
        startPos = Vector3.zero;
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
                startPos = controllerToFollow.position;

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
            Vector3 currentPos = controllerToFollow.position;
            Vector3 diff = currentPos - startPos;
            startPos = currentPos;
            transform.Translate(diff * speed);

            if (OVRInput.GetUp(buttonY))
            {
                StopCoroutine(button_up);

                button_down = StartCoroutine(WaitForButton_Down());
            }

            yield return new WaitForEndOfFrame();
        }
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
