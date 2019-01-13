using System;
using UnityEngine;
using UniRx;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class SceneModeController : MonoBehaviour
{
	[SerializeField] private GameObject sceneContiner;
	[SerializeField] private MenuSceneController menuSceneController;
	[SerializeField] private Transform controllerToFollow;

	[Header("Other")]
	[SerializeField] private McManager mcManager;
	[SerializeField] private ControllerRaycast controllerRaycast;

	protected ISubject<Unit> exitToMainModeSubject = new Subject<Unit>();
	public IObservable<Unit> ExitToMainModeStream { get { return exitToMainModeSubject; } }

	protected ISubject<LoadData> exitToTerrainModeSubject = new Subject<LoadData>();
	public IObservable<LoadData> ExitToTerrainModeStream { get { return exitToTerrainModeSubject; } }

	protected ISubject<LoadData> exitToObjectModeSubject = new Subject<LoadData>();
	public IObservable<LoadData> ExitToObjectModeStream { get { return exitToObjectModeSubject; } }

	private EditableScene scene;
	private ObjectController selectedObject;
	private Coroutine waitForMenuLeftOpenCoroutine;

	private void Start()
	{
		menuSceneController.ExitToMainModeStream.Subscribe(_ => ExitToMainMode());
		menuSceneController.SaveAndExitToMainModeStream.Subscribe(_ => SaveSceneAndExitToMainMode());
		menuSceneController.SuspendAndExitToTerrainModeStream.Subscribe(_ => SuspendAndExitToTerrainMode());
		menuSceneController.SuspendAndExitToObjectModeStream.Subscribe(_ => SuspendAndExitToObjectMode());
		menuSceneController.ModelToAddSelectedStream.Subscribe(AddModelToScene);
		menuSceneController.ModelToEditSelectedStream.Subscribe(SuspendAndExitToObjectMode);
		menuSceneController.ModelToDeleteSelectedStream.Subscribe(DeleteModelFromModelsList);

		controllerRaycast.ObjectSelectedStream.Subscribe(SetObjectSelected);
	}


	public void TurnOnCurrentMode()
	{
		sceneContiner.SetActive(true);
		menuSceneController.SetActive();
		controllerRaycast.SetEnable(true);
	}

	public void TurnOnModeWith(Guid guid)
	{
		scene = mcManager.LoadScene(guid);
		scene.gameObject.transform.parent = sceneContiner.transform;
		scene.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

		sceneContiner.SetActive(true);
		menuSceneController.SetActive();
		controllerRaycast.SetEnable(true);

		ModelsListChanged();
	}

	public void TurnOnCurrentModeWithObjectUpdate(McData data)
	{
		scene.SetOrUpdateModel(new McGameObjData(data, mcManager.LoadModelMeshes(data)));
		ModelsListChanged();
		TurnOnCurrentMode();
		controllerRaycast.SetEnable(true);
	}

	public void TurnOnCurrentModeWithTerrainUpdate(McData data)
	{
		scene.SetOrUpdateTerrain(new McGameObjData(data, mcManager.LoadTerrainMeshes(data)));
		TurnOnCurrentMode();
		controllerRaycast.SetEnable(true);
	}


	private void ExitToMainMode()
	{
		sceneContiner.SetActive(false);
		menuSceneController.SetInactive();
		controllerRaycast.SetEnable(false);

		scene.Destroy();
		scene = null;

		exitToMainModeSubject.OnNext(Unit.Default);
	}

	private void SaveSceneAndExitToMainMode()
	{
		mcManager.Save(scene);
		ExitToMainMode();
		controllerRaycast.SetEnable(false);
	}


	private void SuspendAndExitToTerrainMode()
	{
		sceneContiner.SetActive(false);
		menuSceneController.SetInactive();
		controllerRaycast.SetEnable(false);

		exitToTerrainModeSubject.OnNext
			(new LoadData(scene.Guid, scene.Terrain.Data));
	}

	private void SuspendAndExitToObjectMode()
	{
		sceneContiner.SetActive(false);
		menuSceneController.SetInactive();
		controllerRaycast.SetEnable(false);

		exitToObjectModeSubject.OnNext(new LoadData(scene.Guid, null));
	}

	private void SuspendAndExitToObjectMode(Guid guid)
	{
		sceneContiner.SetActive(false);
		menuSceneController.SetInactive();
		controllerRaycast.SetEnable(false);

		exitToObjectModeSubject.OnNext(new LoadData(scene.Guid, scene.Models[guid].Data));
	}


	private void SetObjectSelected(ObjectController go)
	{
		menuSceneController.SetInactive();
		sceneContiner.GetComponent<MovementWithOculusTouch>().enabled = false;
		controllerRaycast.SetEnable(false);

		if (selectedObject != null)
			SetObjectNormal(selectedObject);
		selectedObject = go;
		go.SetActive();

		waitForMenuLeftOpenCoroutine = StartCoroutine(WaitForNewObjectMovementEnd());
	}

	private IEnumerator WaitForNewObjectMovementEnd()
	{
		while (true)
		{
			if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.LTouch))
			{
				StopCoroutine(waitForMenuLeftOpenCoroutine);

				SetObjectNormal(selectedObject);
				sceneContiner.GetComponent<MovementWithOculusTouch>().enabled = true;
				menuSceneController.SetActive();
				controllerRaycast.SetEnable(true);
			}

			yield return new WaitForEndOfFrame();
		}
	}

	private void SetObjectNormal(ObjectController go)
	{
		go.SetInactive();
		selectedObject = null;
	}


	private void ModelsListChanged()
	{
		List<Guid> modelGuids = scene.Models.Keys.ToList();
		menuSceneController.UpdateModelsGuids(modelGuids);
	}

	private void AddModelToScene(Guid modelGuid)
	{
		GameObject newObject = scene.InstantiateModel(modelGuid);

		ObjectController objectController = newObject.GetComponent<ObjectController>();
		objectController.SetControllerToFollow(controllerToFollow);
		SetObjectSelected(objectController);
	}

	private void DeleteModelFromModelsList(Guid modelGuid)
	{
		scene.DeleteModel(modelGuid);
		mcManager.DeleteModel(modelGuid, scene.Guid);
		ModelsListChanged();
	}
}
