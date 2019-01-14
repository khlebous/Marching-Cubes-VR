using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
	private MovementWithOculusTouch move;
	private Shader normalShader;
	private Shader activeShader;


	private void Awake()
	{
		move = GetComponent<MovementWithOculusTouch>();
		normalShader = Shader.Find("MarchingCubesGPUProject/DrawMarchingCubes");
		activeShader = Shader.Find("Standard");
	}

	public void SetActive()
	{
		move.enabled = true;
		
		foreach(var renderer in gameObject.GetComponentsInChildren<Renderer>())
		{
			renderer.material.shader = activeShader;
			renderer.material.color = Color.red;
		}
	}

	public void SetInactive()
	{
		move.enabled = false;
		foreach (var renderer in gameObject.GetComponentsInChildren<Renderer>())
		{
			renderer.material.shader = normalShader;
		}
	}

	public void SetControllerToFollow(Transform controllerToFollow)
	{
		move.SetControllerToFollow(controllerToFollow);
	}
}
