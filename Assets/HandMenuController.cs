using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandMenuController : MonoBehaviour
{
	void Update()
	{
		Vector2 tmp = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
		if (Mathf.Abs(tmp.x) > Mathf.Abs(tmp.y))
		{
			if (tmp.x > 0.5)
				Debug.Log("right");
			else if (tmp.x <= -1)
				Debug.Log("left");
		}
		else
		{
			if (tmp.y == 1 )
				Debug.Log("up");
			else if (tmp.y == -1)
				Debug.Log("down");
		}
	}
}
