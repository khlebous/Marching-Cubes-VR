using UnityEngine;
using UniRx;
using System;

public class TerrainModeController : MonoBehaviour
{
	[SerializeField] private GameObject terrainContiner;
	[SerializeField] private MenuTerrainController menuTerrainController;

	[Header("Other")]
	[SerializeField] private McManager mcManager;
	[SerializeField] private Assets.MarchingCubesGPU.Scripts.TerrainBrush terrainBrush;

	protected ISubject<Unit> modeExitedSubject = new Subject<Unit>();
	public IObservable<Unit> ModeExitedStream { get { return modeExitedSubject; } }

	protected ISubject<McData> modeSavedAndExitedSubject = new Subject<McData>();
	public IObservable<McData> ModeSavedAndExitedStream { get { return modeSavedAndExitedSubject; } }

	Guid sceneGuid;
	MarchingCubesGPUProject.EditableTerrain terrain;


	private void Start()
	{
		menuTerrainController.ExitToSceneModeStream.Subscribe(_ => ExitMode());
		menuTerrainController.SaveAndExitToSceneModeStream.Subscribe(_ => SaveTerrainAndExitMode());
	}


	public void TurnOnMode(LoadData loadData)
	{
		this.sceneGuid = loadData.sceneGuid;

		terrain = mcManager.LoadTerrain(loadData.data);
		terrain.gameObject.transform.parent = terrainContiner.transform;
		terrain.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		
		terrainContiner.SetActive(true);
		menuTerrainController.ResetMenus();
		terrainBrush.SetActive();
	}

	private void ExitMode()
	{
		terrainBrush.SetInactive();
		terrainContiner.SetActive(false);
		menuTerrainController.SetInactive();

		terrain.Destroy();
		terrain = null;

		modeExitedSubject.OnNext(Unit.Default);
	}

	private void SaveTerrainAndExitMode()
	{
		terrainBrush.SetInactive();
		mcManager.Save(terrain, sceneGuid);

		terrainContiner.SetActive(false);
		menuTerrainController.SetInactive();

		modeSavedAndExitedSubject.OnNext(terrain.GetData());
		terrain.Destroy();
		terrain = null;
	}
}