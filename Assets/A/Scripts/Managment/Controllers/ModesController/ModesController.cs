using UnityEngine;
using UniRx;
using System;

public class ModesController : MonoBehaviour
{
	[SerializeField] private MainModeController mainModeController;
	[SerializeField] private SceneModeController sceneModeController;
	[SerializeField] private TerrainModeController terrainModeController;
	[SerializeField] private ObjectModeController objectModeController;

	private void Start()
	{
		mainModeController.ItemSelectedStream.Subscribe(TurnOnSceneMode);
		sceneModeController.ExitToMainModeStream.Subscribe(_ => TurnOnMainMode());
	}

	private void TurnOnMainMode()
	{
		mainModeController.TurnOn();
		sceneModeController.TurnOff();
		terrainModeController.TurnOff();
		objectModeController.TurnOff();
	}

	private void TurnOnSceneMode(Guid sceneGuid)
	{
		mainModeController.TurnOff();
		sceneModeController.TurnOn(sceneGuid);
		//terrainModeController.TurnOff();
		//objectModeController.TurnOff();
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
