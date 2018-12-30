using UnityEngine;

public class ObjectModeController : MonoBehaviour, IModeController
{
	public void TurnOn()
	{
		Debug.Log("ObjectModeController  turn on");
	}

	public void TurnOff()
	{
		Debug.Log("ObjectModeController  turn off");
	}
}
