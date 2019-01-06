using System;
using UnityEngine;
using UniRx;

public class SceneModeController : MonoBehaviour
{
	[SerializeField] private GameObject sceneContiner;
	[SerializeField] private MenuSceneController menuSceneController;

	[Header("Other")]
	[SerializeField] private McManager mcManager;

	protected ISubject<Unit> exitToMainModeSubject = new Subject<Unit>();
	public IObservable<Unit> ExitToMainModeStream { get { return exitToMainModeSubject; } }

	protected ISubject<TerrainLoadData> exitToTerrainModeSubject = new Subject<TerrainLoadData>();
	public IObservable<TerrainLoadData> ExitToTerrainModeStream { get { return exitToTerrainModeSubject; } }

	protected ISubject<Unit> exitToObjectModeSubject = new Subject<Unit>();
	public IObservable<Unit> ExitToObjectModeStream { get { return exitToObjectModeSubject; } }

	//private Guid currentGuid;
	private EditableScene scene;

	private void Start()
	{
		menuSceneController.ExitToMainModeStream.Subscribe(_ => ExitToMainMode());
		menuSceneController.SaveAndExitToMainModeStream.Subscribe(_ => SaveSceneAndExitToMainMode());
		menuSceneController.SuspendAndExitToTerrainModeStream.Subscribe(_ => SuspendAndExitToTerrainMode());
		menuSceneController.SuspendAndExitToObjectModeStream.Subscribe(_ => SuspendAndExitToObjectMode());
	}

	public void TurnOnModeWith(Guid guid)
	{
		scene = mcManager.LoadScene(guid);
		scene.gameObject.transform.parent = sceneContiner.transform;

		sceneContiner.SetActive(true);
		menuSceneController.SetActive();
	}

	private void ExitToMainMode()
	{
		sceneContiner.SetActive(false);
		menuSceneController.SetInactive();

		scene.Destroy();
		scene = null;

		exitToMainModeSubject.OnNext(Unit.Default);
	}

	private void SaveSceneAndExitToMainMode()
	{
		mcManager.Save(scene);
		ExitToMainMode();
	}

	public void TurnOnCurrentMode()
	{
		sceneContiner.SetActive(true);
		menuSceneController.SetActive();
	}

	private void SuspendAndExitToTerrainMode()
	{
		sceneContiner.SetActive(false);
		menuSceneController.SetInactive();

		exitToTerrainModeSubject.OnNext
			(new TerrainLoadData(scene.Guid, scene.Terrain.Data));
	}

	public void TurnOnCurrentModeWithUpdate(McData data)
	{
		//scene.Update(new McGameObjData(data), mcManager.
		//mcManager.UpdateScene(McData );
		//TurnOnCurrentMode();
	}

	private void SuspendAndExitToObjectMode()
	{
		sceneContiner.SetActive(false);
		menuSceneController.SetInactive();

		exitToObjectModeSubject.OnNext(Unit.Default);
	}
}
