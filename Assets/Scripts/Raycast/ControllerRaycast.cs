using UnityEngine;
using System.Collections;
using UniRx;

public class ControllerRaycast : MonoBehaviour
{
	[SerializeField] private float maxRayDistance = 20f;
	[SerializeField] private OVRInput.RawButton button = OVRInput.RawButton.B;

	protected ISubject<ObjectController> objectSelectedSubject = new Subject<ObjectController>();
	public IObservable<ObjectController> ObjectSelectedStream { get { return objectSelectedSubject; } }

	private LineRenderer lineRenderer;
	private Coroutine button_down;
	private Coroutine button_up;

	void Awake()
	{
		lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.SetPositions(new Vector3[2] { Vector3.zero, Vector3.zero });
		lineRenderer.startWidth = 0.1f;
		lineRenderer.endWidth = 0.01f;
	}

	private void OnEnable()
	{
		StartListening();
	}

	private void OnDisable()
	{
		StopListening();
	}

	public void SetActive(bool value)
	{
		gameObject.SetActive(value);
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
			if (OVRInput.GetDown(button))
			{
				lineRenderer.enabled = true;
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
			var raycastHit = ShootLaserFromTargetPosition
				(transform.position, transform.forward, maxRayDistance);

			if (OVRInput.GetUp(button))
			{
				if (raycastHit.collider != null)
				{
					lineRenderer.enabled = false;
					ObjectController objController = raycastHit.collider.gameObject.transform
						.GetComponentInParent<ObjectController>();
					if (objController != null)
						objectSelectedSubject.OnNext(objController);

					StopListening();
					button_down = StartCoroutine(WaitForButton_Down());
				}
			}

			yield return new WaitForEndOfFrame();
		}
	}

	RaycastHit ShootLaserFromTargetPosition(Vector3 targetPosition, Vector3 direction, float length)
	{
		Ray ray = new Ray(targetPosition, direction);
		RaycastHit raycastHit;
		Vector3 endPosition = targetPosition + (length * direction);

		if (Physics.Raycast(ray, out raycastHit, length))
			endPosition = raycastHit.point;

		lineRenderer.SetPosition(0, targetPosition);
		lineRenderer.SetPosition(1, endPosition);
		Debug.DrawLine(transform.position, transform.position + direction * maxRayDistance, Color.red);

		return raycastHit;
	}
}
