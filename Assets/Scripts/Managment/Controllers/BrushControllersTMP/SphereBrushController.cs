using UnityEngine;

public class SphereBrushController : BrushController
{
	protected override void SetBrushMesh()
	{
		gameObject.transform.localScale = new Vector3(2 * Radius, 2 * Radius, 2 * Radius);
	}
}
