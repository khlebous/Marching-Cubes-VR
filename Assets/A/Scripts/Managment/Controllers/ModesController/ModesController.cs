using UnityEngine;
using UniRx;
using System;

public class ModesController : MonoBehaviour
{
	[Header("Mode controllers")]
	[SerializeField] private MainModeController mainModeController;
	[SerializeField] private SceneModeController sceneModeController;
	[SerializeField] private TerrainModeController terrainModeController;
	[SerializeField] private ModelModeController objectModeController;

	[Header("Mangers")]
	[SerializeField] private McManager mcManager;

	private void Start()
	{
		mainModeController.ItemSelectedStream.Subscribe(LoadSceneWithGuid);
		sceneModeController.ExitToMainModeStream.Subscribe(_ => TurnOnMainModeFromSceneMode());
		sceneModeController.ExitToTerrainModeStream.Subscribe(_ => TurnOnTerrainModeFromSceneMode());
		sceneModeController.ExitToObjectModeStream.Subscribe(_ => TurnOnObjectModeFromSceneMode());
		terrainModeController.ModeExitedStream.Subscribe(_ => TurnOnSceneModeFromTerrainMode());
		objectModeController.ModeExitedStream.Subscribe(_ => TurnOnSceneModeFromObjectMode());

		LoadMainMode();
	}

	private void LoadMainMode()
	{
		mainModeController.TurnOnModeWithCurrentSceneGuids(mcManager.GetAllSceneGuids());
	}

	// Exit main menu mode
	private void LoadSceneWithGuid(Guid sceneGuid)
	{
		sceneModeController.TurnOnModeWith(sceneGuid);
	}
	// Exit scene mode
	private void TurnOnMainModeFromSceneMode()
	{
		mainModeController.TurnOnModeWithCurrentSceneGuids(mcManager.GetAllSceneGuids());
	}

	private void TurnOnTerrainModeFromSceneMode()
	{
		terrainModeController.TurnOnMode();
	}

	private void TurnOnObjectModeFromSceneMode()
	{
		objectModeController.TurnOnMode();
	}

	// Exit terrain mode
	private void TurnOnSceneModeFromTerrainMode()
	{
		sceneModeController.TurnOnCurrentMode(); 
	}

	// Exit obj mode
	private void TurnOnSceneModeFromObjectMode() // TODO same as exit terrain?
	{
		sceneModeController.TurnOnCurrentMode();
	}
}
