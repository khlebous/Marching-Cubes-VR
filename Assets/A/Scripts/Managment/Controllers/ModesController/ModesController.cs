using UnityEngine;
using UniRx;

public class ModesController : MonoBehaviour
{
	[SerializeField] private MainModeController mainModeController;
	[SerializeField] private SceneModeController sceneModeController;
	[SerializeField] private TerrainModeController terrainModeController;
	[SerializeField] private ObjectModeController objectModeController;

	private void Start()
	{
		mainModeController.ItemSelectedStream.Subscribe(_ => TurnOnSceneMode());
		sceneModeController.ExitToMainModeStream.Subscribe(_ => TurnOnMainMode());
	}

	private void TurnOnMainMode()
	{
		mainModeController.TurnOn();
		sceneModeController.TurnOff();
		terrainModeController.TurnOff();
		objectModeController.TurnOff();
	}

	private void TurnOnSceneMode()
	{
		mainModeController.TurnOff();
		sceneModeController.TurnOn();
		terrainModeController.TurnOff();
		objectModeController.TurnOff();
	}

	private void TurnOnTerrainMode()
	{
		mainModeController.TurnOff();
		sceneModeController.TurnOff();
		terrainModeController.TurnOn();
		objectModeController.TurnOff();
	}

	private void TurnOnObjectMode()
	{
		mainModeController.TurnOff();
		sceneModeController.TurnOff();
		terrainModeController.TurnOff();
		objectModeController.TurnOn();
	}
}
