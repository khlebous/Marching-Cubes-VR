using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycatTesting : MonoBehaviour
{
	public LineRenderer laserLineRenderer;
	public float maxRayDistance = 20f;
	public LayerMask activeLayers;

	void Start()
	{
		Vector3[] initLaserPositions = new Vector3[2] { Vector3.zero, Vector3.zero };
		laserLineRenderer.SetPositions(initLaserPositions);
		//laserLineRenderer.SetWidth(0.1f, 0.1f);
		laserLineRenderer.startWidth = 0.1f;
		laserLineRenderer.endWidth = 0.01f;
	}

	void Update()
	{
		if (true)
		{
			ShootLaserFromTargetPosition(transform.position, transform.forward, maxRayDistance);
			laserLineRenderer.enabled = true;
		}
		else
		{
			laserLineRenderer.enabled = false;
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
			Debug.Log(raycastHit.collider.gameObject.name);
		}

		laserLineRenderer.SetPosition(0, targetPosition);
		laserLineRenderer.SetPosition(1, endPosition);
		Debug.DrawLine(transform.position, transform.position + direction * maxRayDistance, Color.red);
	}

	//private void FixedUpdate()
	//{
	//	Ray ray = new Ray(transform.position, transform.forward);
	//	RaycastHit[] hits = Physics.RaycastAll(ray, maxRayDistance, activeLayers);

	//	Debug.DrawLine(transform.position, transform.position + transform.forward * maxRayDistance, Color.red);


	//}
}
