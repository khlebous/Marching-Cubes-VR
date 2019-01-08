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

	private void TurnOnTerrainModeFromSceneMode(LoadData data)
	{
		terrainModeController.TurnOnMode(data);
	}

	private void TurnOnObjectModeFromSceneMode(LoadData loadData)
	{
		objectModeController.TurnOnMode(loadData);
	}

	// Exit terrain mode
	private void TurnOnSceneModeFromTerrainMode()
	{
		sceneModeController.TurnOnCurrentMode(); 
	}

	private void UpdateAndTurnOnSceneModeFromTerrainMode(McData terrainData)
	{
		sceneModeController.TurnOnCurrentModeWithTerrainUpdate(terrainData);
	}

	private void UpdateAndTurnOnSceneModeFromObjectMode(McData terrainData)
	{
		sceneModeController.TurnOnCurrentModeWithObjectUpdate(terrainData);
	}

	// Exit obj mode
	private void TurnOnSceneModeFromObjectMode() // TODO same as exit terrain?
	{
		sceneModeController.TurnOnCurrentMode();
	}
}
