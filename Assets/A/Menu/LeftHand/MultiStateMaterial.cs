using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class MultiStateMaterial : MonoBehaviour
{
	[SerializeField] private List<Material> materials;
	private MeshRenderer meshRenderer;

	public void Start()
	{
		//meshRenderer = GetComponent<MeshRenderer>();
	}

	public void SetState(int state)
	{
		if (meshRenderer == null)
			meshRenderer = GetComponent<MeshRenderer>();
		meshRenderer.material = materials[state];
	}
}
