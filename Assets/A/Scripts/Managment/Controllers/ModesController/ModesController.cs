using System;
using UnityEngine;
using UniRx;

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
		sceneModeController.ExitToTerrainModeStream.Subscribe(TurnOnTerrainModeFromSceneMode);
		sceneModeController.ExitToObjectModeStream.Subscribe(TurnOnObjectModeFromSceneMode);

		terrainModeController.ModeExitedStream.Subscribe(_ => TurnOnSceneModeFromTerrainMode());
		terrainModeController.ModeSavedAndExitedStream.Subscribe(UpdateAndTurnOnSceneModeFromTerrainMode);

		objectModeController.ModeExitedStream.Subscribe(_ => TurnOnSceneModeFromObjectMode());
		objectModeController.ModeSavedAndExitedStream.Subscribe(UpdateAndTurnOnSceneModeFromObjectMode);

		LoadMainMode();
	}

	private void LoadMainMode()
	{
		mainModeController.TurnOnModeWithCurrentSceneGuids(mcManager.GetAllSceneGuids());
	}


	private void LoadSceneWithGuid(Guid sceneGuid)
	{
		sceneModeController.TurnOnModeWith(sceneGuid);
	}


	private void TurnOnMainModeFromSceneMode()
	{
		mainModeController.TurnOnModeWithCurrentSceneGuids(mcManager.GetAllSceneGuids());
	}

	private void TurnOnTerrainModeFromSceneMode(LoadData terrainData)
	{
		terrainModeController.TurnOnMode(terrainData);
	}

	private void TurnOnObjectModeFromSceneMode(LoadData objectData)
	{
		objectModeController.TurnOnMode(objectData);
	}


	private void TurnOnSceneModeFromTerrainMode()
	{
		sceneModeController.TurnOnCurrentMode(); 
	}

	private void UpdateAndTurnOnSceneModeFromTerrainMode(McData terrainData)
	{
		sceneModeController.TurnOnCurrentModeWithTerrainUpdate(terrainData);
	}


	private void TurnOnSceneModeFromObjectMode() // TODO same as exit terrain?
	{
		sceneModeController.TurnOnCurrentMode();
	}

	private void UpdateAndTurnOnSceneModeFromObjectMode(McData terrainData)
	{
		sceneModeController.TurnOnCurrentModeWithObjectUpdate(terrainData);
	}
}
