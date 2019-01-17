using UnityEngine;

public class FlatBrushController : BrushController
{
	protected override void SetBrushMesh()
	{
		Vector3[] newVert = new Vector3[4];
		newVert[0] = new Vector3(-Radius, -Radius, 0);
		newVert[1] = new Vector3(Radius, Radius, 0);
		newVert[2] = new Vector3(Radius, -Radius, 0);
		newVert[3] = new Vector3(-Radius, Radius, 0);
		GetComponent<MeshFilter>().mesh.vertices = newVert;
	}
}
