using UniRx;
using UnityEngine;

public class ControllerRaycast : MonoBehaviour
{
	public LineRenderer laserLineRenderer;
	public float maxRayDistance = 20f;
	public LayerMask activeLayers;
	private bool rayEnabled = true;

	protected ISubject<ObjectController> objectSelectedSubject = new Subject<ObjectController>();
	public IObservable<ObjectController> ObjectSelectedStream { get { return objectSelectedSubject; } }

	void Start()
	{
		Vector3[] initLaserPositions = new Vector3[2] { Vector3.zero, Vector3.zero };
		laserLineRenderer.SetPositions(initLaserPositions);
		laserLineRenderer.startWidth = 0.1f;
		laserLineRenderer.endWidth = 0.01f;

		SetEnable(false);
	}

	public void SetEnable(bool enabled)
	{
		rayEnabled = enabled;
		laserLineRenderer.enabled = enabled;
	}

	void Update()
	{
		if (rayEnabled)
		{
			ShootLaserFromTargetPosition(transform.position, transform.forward, maxRayDistance);
		}
	}

	void ShootLaserFromTargetPosition(Vector3 targetPosition, Vector3 direction, float length)
	{
		Ray ray = new Ray(targetPosition, direction);
		RaycastHit raycastHit;
		Vector3 endPosition = targetPosition + (length * direction);

		if (Physics.Raycast(ray, out raycastHit, length))
		{
			endPosition = raycastHit.point;

			if (OVRInput.Get(OVRInput.RawButton.A))
			{
				ObjectController objController = raycastHit.collider.gameObject.transform
					.GetComponentInParent<ObjectController>();
				if (objController != null)
					objectSelectedSubject.OnNext(objController);
			}
		}

		laserLineRenderer.SetPosition(0, targetPosition);
		laserLineRenderer.SetPosition(1, endPosition);
		Debug.DrawLine(transform.position, transform.position + direction * maxRayDistance, Color.red);
	}
}
