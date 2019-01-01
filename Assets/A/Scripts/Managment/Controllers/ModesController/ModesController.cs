using UnityEngine;
using UniRx;
using System;

public class ModesController : MonoBehaviour
{
	[Header("Mode controllers")]
	[SerializeField] private MainModeController mainModeController;
	[SerializeField] private SceneModeController sceneModeController;
	[SerializeField] private TerrainModeController terrainModeController;
	[SerializeField] private ObjectModeController objectModeController;

	[Header("Mangers")]
	[SerializeField] private McManager mcManager;

	private void Start()
	{
		mainModeController.ItemSelectedStream.Subscribe(LoadSceneWithGuid);

		sceneModeController.ExitToMainModeStream.Subscribe(_ => TurnOnMainModeFromSceneMode());
		sceneModeController.ExitToMainModeStream.Subscribe(_ => SaveSceneAndTurnOnMainMode());
	}

	private void TurnOnMainModeFromSceneMode()
	{
		sceneModeController.ExitMode();
		mainModeController.TurnOnModeWithCurrentSceneGuids(mcManager.GetAllSceneGuids());
	}

	private void SaveSceneAndTurnOnMainMode()
	{
		sceneModeController.SaveSceneAndExitMode();
		mainModeController.TurnOnModeWithCurrentSceneGuids(mcManager.GetAllSceneGuids());
	}

	private void LoadSceneWithGuid(Guid sceneGuid)
	{
		mainModeController.TurnOff();
		sceneModeController.TurnOn(sceneGuid);
	}

	
}
