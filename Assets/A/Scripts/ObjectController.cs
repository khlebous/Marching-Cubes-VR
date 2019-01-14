using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
	private MovementWithOculusTouch move;
	private RotationWithOculusTouch rot;
	private Shader normalShader;
	private Shader activeShader;


	private void Awake()
	{
		move = GetComponent<MovementWithOculusTouch>();
		rot = GetComponent<RotationWithOculusTouch>();
		normalShader = Shader.Find("MarchingCubesGPUProject/DrawMarchingCubes");
		activeShader = Shader.Find("Standard");
	}

	public void SetActive()
	{
		move.enabled = true;
		rot.enabled = true;

		foreach (var renderer in gameObject.GetComponentsInChildren<Renderer>())
		{
			renderer.material.shader = activeShader;
			renderer.material.color = Color.red;
		}
	}

	public void SetInactive()
	{
		move.enabled = false;
		rot.enabled = false;
		foreach (var renderer in gameObject.GetComponentsInChildren<Renderer>())
			renderer.material.shader = normalShader;
	}

	public void SetControllerToFollow(Transform controllerToFollow)
	{
		move.SetControllerToFollow(controllerToFollow);
		rot.SetControllerToFollow(controllerToFollow);
	}
}
