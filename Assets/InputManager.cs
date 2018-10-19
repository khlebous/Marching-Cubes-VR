using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
	[SerializeField] GameObject obj;
	[SerializeField] int scale;
	[SerializeField] int speed;
	[SerializeField] int limit;

	Vector3 startPos;
	bool startedXZ;
	bool startedY;
	OVRInput.Button button = OVRInput.Button.One;
	OVRInput.Button button2 = OVRInput.Button.Two;

	void Update()
	{
		if (OVRInput.GetUp(button))
		{
			startedXZ = false;
			Debug.Log("up");
		}
		else if (startedXZ)
		{
			Vector3 currentPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
			Vector3 diff = startPos - currentPos;
			startPos = currentPos;
			obj.transform.Rotate(new Vector3(0, diff.x * scale, 0), Space.World);

		}
		else if (OVRInput.GetDown(button))
		{
			startPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
			Debug.Log("start pos: " + startPos);
			startedXZ = true;
		}

		if (OVRInput.GetUp(button2))
		{
			startedY = false;
			Debug.Log("up2");
		}
		else if (startedY)
		{
			Vector3 currentPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
			Vector3 diff = startPos - currentPos;
			startPos = currentPos;
			obj.transform.Rotate(new Vector3(-scale * diff.y, 0, 0), Space.World);


			if (obj.transform.eulerAngles.x > limit && obj.transform.eulerAngles.x < 360 - limit)
			{
				float a = obj.transform.eulerAngles.x - limit;
				float b = 360 - obj.transform.eulerAngles.x - limit;
				float c = a > b ? 360 - limit : limit;

				obj.transform.eulerAngles = new Vector3(c, obj.transform.eulerAngles.y, obj.transform.eulerAngles.z);
			}

			if (obj.transform.eulerAngles.z > limit && obj.transform.eulerAngles.z < 360 - limit)
			{
				float a = obj.transform.eulerAngles.z- limit;
				float b = 360 - obj.transform.eulerAngles.z - limit;
				float c = a > b ? 360 - limit : limit;

				obj.transform.eulerAngles = new Vector3( obj.transform.eulerAngles.x, obj.transform.eulerAngles.y,c);
			}
		}
		else if (OVRInput.GetDown(button2))
		{
			startPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
			Debug.Log("start pos: " + startPos);
			startedY = true;
		}
	}
}
