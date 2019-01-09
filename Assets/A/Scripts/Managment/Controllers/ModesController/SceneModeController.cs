using System;
using UnityEngine;
using UniRx;
using System.Collections.Generic;
using System.Linq;

public class SceneModeController : MonoBehaviour
{
	[SerializeField] private GameObject sceneContiner;
	[SerializeField] private MenuSceneController menuSceneController;

	[Header("Other")]
	[SerializeField] private McManager mcManager;

	protected ISubject<Unit> exitToMainModeSubject = new Subject<Unit>();
	public IObservable<Unit> ExitToMainModeStream { get { return exitToMainModeSubject; } }

	protected ISubject<LoadData> exitToTerrainModeSubject = new Subject<LoadData>();
	public IObservable<LoadData> ExitToTerrainModeStream { get { return exitToTerrainModeSubject; } }

	protected ISubject<LoadData> exitToObjectModeSubject = new Subject<LoadData>();
	public IObservable<LoadData> ExitToObjectModeStream { get { return exitToObjectModeSubject; } }

	private EditableScene scene;

	private void Start()
	{
		menuSceneController.ExitToMainModeStream.Subscribe(_ => ExitToMainMode());
		menuSceneController.SaveAndExitToMainModeStream.Subscribe(_ => SaveSceneAndExitToMainMode());
		menuSceneController.SuspendAndExitToTerrainModeStream.Subscribe(_ => SuspendAndExitToTerrainMode());
		menuSceneController.SuspendAndExitToObjectModeStream.Subscribe(_ => SuspendAndExitToObjectMode());
		menuSceneController.ModelToAddSelectedStream.Subscribe(AddModelToScene);
		menuSceneController.ModelToEditSelectedStream.Subscribe(SuspendAndExitToObjectMode);
		menuSceneController.ModelToDeleteSelectedStream.Subscribe(DeleteModelFromModelsList);

	}

	private void AddModelToScene(Guid modelGuid)
	{
		scene.InstantiateModel(modelGuid);
	}

	private void DeleteModelFromModelsList(Guid modelGuid)
	{
		scene.DeleteModel(modelGuid);
		mcManager.DeleteModel(modelGuid, scene.Guid);
		ModelsListChanged();
	}

	public void TurnOnModeWith(Guid guid)
	{
		scene = mcManager.LoadScene(guid);
		scene.gameObject.transform.parent = sceneContiner.transform;
		scene.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

		sceneContiner.SetActive(true);
		menuSceneController.SetActive();

		ModelsListChanged();
	}

	private void ModelsListChanged()
	{
		List<Guid> modelGuids = scene.Models.Keys.ToList();
		menuSceneController.UpdateModelsGuids(modelGuids);
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
			(new LoadData(scene.Guid, scene.Terrain.Data));
	}

	public void TurnOnCurrentModeWithTerrainUpdate(McData data)
	{
		scene.SetOrUpdateTerrain(new McGameObjData(data, mcManager.LoadTerrainMeshes(data)));
		TurnOnCurrentMode();
	}

	public void TurnOnCurrentModeWithObjectUpdate(McData data)
	{
		scene.SetOrUpdateModel(new McGameObjData(data, mcManager.LoadModelMeshes(data)));
		ModelsListChanged();
		TurnOnCurrentMode();
	}

	private void SuspendAndExitToObjectMode()
	{
		sceneContiner.SetActive(false);
		menuSceneController.SetInactive();

		exitToObjectModeSubject.OnNext(new LoadData(scene.Guid, null));
	}

	private void SuspendAndExitToObjectMode(Guid guid)
	{
		sceneContiner.SetActive(false);
		menuSceneController.SetInactive();

		exitToObjectModeSubject.OnNext(new LoadData(scene.Guid, scene.Models[guid].Data));
	}
}
