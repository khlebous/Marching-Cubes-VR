using UnityEngine;

public class TerrainModeController : MonoBehaviour, IModeController
{
	public void TurnOn()
	{
		Debug.Log("TerrainModeController turn on");
	}

	public void TurnOff()
	{
		Debug.Log("TerrainModeController turn off");
	}
}
