using UnityEngine;
using UniRx;
using System;

public class TerrainModeController : MonoBehaviour
{
	[SerializeField] private GameObject terrainContiner;
	[SerializeField] private MenuTerrainController menuTerrainController;

	[Header("Other")]
	[SerializeField] private McManager mcManager;

	protected ISubject<Unit> modeExitedSubject = new Subject<Unit>();
	public IObservable<Unit> ModeExitedStream { get { return modeExitedSubject; } }

	protected ISubject<Unit> modeSavedAndExitedSubject = new Subject<Unit>();
	public IObservable<Unit> ModeSavedAndExitedStream { get { return modeSavedAndExitedSubject; } }

	MarchingCubesGPUProject.EditableTerrain terrain;
	Guid sceneGuid;

	private void Start()
	{
		menuTerrainController.ExitToSceneModeStream.Subscribe(_ => ExitMode());
		menuTerrainController.SaveAndExitToSceneModeStream.Subscribe(_ => SaveTerrainAndExitMode());
	}

	public void TurnOnMode(TerrainLoadData loadData)
	{
		this.sceneGuid = loadData.sceneGuid;

		terrain = mcManager.LoadTerrain(loadData.data);
		terrain.gameObject.transform.parent = terrainContiner.transform;
		terrain.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

		terrainContiner.SetActive(true);
		menuTerrainController.SetActive();
	}

	private void ExitMode()
	{
		terrainContiner.SetActive(false);
		menuTerrainController.SetInactive();

		terrain.Destroy();
		terrain = null;

		modeExitedSubject.OnNext(Unit.Default);
	}

	private void SaveTerrainAndExitMode()
	{
		mcManager.Save(terrain, sceneGuid);

		terrainContiner.SetActive(false);
		menuTerrainController.SetInactive();

		terrain.Destroy();
		terrain = null;

		modeSavedAndExitedSubject.OnNext(Unit.Default);
	}
}