﻿using System;
using UnityEngine;
using UniRx;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.IO;

public class SceneModeController : MonoBehaviour
{
	[SerializeField] private GameObject sceneContiner;
	[SerializeField] private Transform controllerToFollow;
	[SerializeField] private MenuSceneController menuSceneController;
	[SerializeField] private MenuObjectChosenController menuObjectChoosen;

	[Header("Other")]
	[SerializeField] private McManager mcManager;
	[SerializeField] private ControllerRaycast controllerRaycast;
	[SerializeField] private CameraCapture cameraCapture;

    [SerializeField] private Transform oculusBase;

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
		menuSceneController.PhotoRequestStream.Subscribe(_ => TakePhoto());

		menuObjectChoosen.StopEditingStream.Subscribe(_ => StopEditnigObject());
		menuObjectChoosen.EditModelStream.Subscribe(_ => EditModel());
		menuObjectChoosen.DeleteObjectStream.Subscribe(_ => DeleteObject());

		controllerRaycast.ObjectSelectedStream.Subscribe(SetObjectSelected);
	}

	private void DeleteObject()
	{
		menuObjectChoosen.CloseMenu();

		sceneContiner.GetComponent<MovementWithOculusTouch>().enabled = true;
		sceneContiner.GetComponent<RotationWithOculusTouch>().enabled = true;
		sceneContiner.GetComponent<ScaleWithOculusTouch>().enabled = true;
		menuSceneController.SetActive();
		controllerRaycast.SetActive(true);

		if (selectedObject != null)
		{
			scene.DeleteModelFromTerrain(selectedObject.gameObject);
			selectedObject = null;
		}
	}

	private void EditModel()
	{
		Guid guid = scene.ModelsOnTerrain.FirstOrDefault
			(x => x.GameObject == selectedObject.gameObject).ModelGuid;
		SetObjectNormal(selectedObject);
		menuObjectChoosen.CloseMenu();
		SuspendAndExitToObjectMode(guid);
	}

	private void StopEditnigObject()
	{
		menuObjectChoosen.CloseMenu();

		SetObjectNormal(selectedObject);
		sceneContiner.GetComponent<MovementWithOculusTouch>().enabled = true;
		sceneContiner.GetComponent<RotationWithOculusTouch>().enabled = true;
		sceneContiner.GetComponent<ScaleWithOculusTouch>().enabled = true;
		menuSceneController.SetActive();
		controllerRaycast.SetActive(true);
	}

	public void TurnOnCurrentMode()
	{
		sceneContiner.SetActive(true);
		menuSceneController.ResetMenus();
		controllerRaycast.SetActive(true);
	}

	public void TurnOnModeWith(Guid guid)
	{
		scene = mcManager.LoadScene(guid);
		scene.gameObject.transform.parent = sceneContiner.transform;
		scene.transform.localPosition = new Vector3(0, 0, 0);
		scene.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        sceneContiner.transform.rotation = oculusBase.rotation;
        var positionOffset = oculusBase.forward * McConsts.TerrN * scene.transform.lossyScale.x - oculusBase.up;
        sceneContiner.transform.position = oculusBase.position + positionOffset;

        menuSceneController.UpdatePhoto(PathHelper.GetScenePngPath(scene.Guid));
		TurnOnCurrentMode();
		ModelsListChanged();
	}

	public void TurnOnCurrentModeWithObjectUpdate(McData data)
	{
		scene.SetOrUpdateModel(new McGameObjData(data, mcManager.LoadModelMeshes(data)));
		ModelsListChanged();
		TurnOnCurrentMode();
		controllerRaycast.SetActive(true);
	}

	public void TurnOnCurrentModeWithTerrainUpdate(McData data)
	{
		scene.SetOrUpdateTerrain(new McGameObjData(data, mcManager.LoadTerrainMeshes(data)));
		TurnOnCurrentMode();
		controllerRaycast.SetActive(true);
	}


	private void ExitToMainMode()
	{
		sceneContiner.SetActive(false);
		menuSceneController.SetInactive();
		controllerRaycast.SetActive(false);

		string tmpPath = PathHelper.GetSceneTmpPngPath(scene.Guid);
		if (File.Exists(tmpPath))
			File.Delete(tmpPath);

		exitToMainModeSubject.OnNext(Unit.Default);
		scene.Destroy();
		scene = null;
	}

	private void SaveSceneAndExitToMainMode()
	{
		mcManager.Save(scene);
		sceneContiner.SetActive(false);
		menuSceneController.SetInactive();
		controllerRaycast.SetActive(false);

		string path = PathHelper.GetScenePngPath(scene.Guid);
		string tmpPath = PathHelper.GetSceneTmpPngPath(scene.Guid);

		if (File.Exists(tmpPath))
		{
			if (File.Exists(path))
				File.Delete(path);
			File.Move(tmpPath, path);
		}

		scene.Destroy();
		scene = null;
		exitToMainModeSubject.OnNext(Unit.Default);
	}


	private void SuspendAndExitToTerrainMode()
	{
		sceneContiner.SetActive(false);
		menuSceneController.SetInactive();
		controllerRaycast.SetActive(false);

		exitToTerrainModeSubject.OnNext
			(new LoadData(scene.Guid, scene.Terrain.Data));
	}

	private void SuspendAndExitToObjectMode()
	{
		sceneContiner.SetActive(false);
		menuSceneController.SetInactive();
		controllerRaycast.SetActive(false);

		exitToObjectModeSubject.OnNext(new LoadData(scene.Guid, null));
	}

	private void SuspendAndExitToObjectMode(Guid guid)
	{
		sceneContiner.SetActive(false);
		menuSceneController.SetInactive();
		controllerRaycast.SetActive(false);

		exitToObjectModeSubject.OnNext(new LoadData(scene.Guid, scene.Models[guid].Data));
	}


	private void SetObjectSelected(ObjectController go)
	{
		menuSceneController.SetInactive();
		sceneContiner.GetComponent<MovementWithOculusTouch>().enabled = false;
		sceneContiner.GetComponent<RotationWithOculusTouch>().enabled = false;
		sceneContiner.GetComponent<ScaleWithOculusTouch>().enabled = false;
		controllerRaycast.SetActive(false);

		if (selectedObject != null)
			SetObjectNormal(selectedObject);
		selectedObject = go;
		go.SetControllerToFollow(controllerToFollow);
		go.SetActive();

		menuObjectChoosen.OpenMenu();
	}

	private void SetObjectNormal(ObjectController go)
	{
		go.SetInactive();
		selectedObject = null;
	}


	private void ModelsListChanged()
	{
		List<Guid> modelGuids = scene.Models.Keys.ToList();
		menuSceneController.UpdateModelsGuids(scene.Guid, modelGuids);
	}

	private void AddModelToScene(Guid modelGuid)
	{
		GameObject newObject = scene.InstantiateModel(modelGuid);

        newObject.transform.rotation = oculusBase.rotation;
        var positionOffset = oculusBase.forward * McConsts.TerrN  * scene.transform.lossyScale.x + oculusBase.up;
        newObject.transform.position = oculusBase.position + positionOffset;

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

	private void TakePhoto()
	{
		string path = PathHelper.GetSceneTmpPngPath(scene.Guid);
		PathHelper.EnsureDirForFileExists(path);
		cameraCapture.TakeShot(path);
		menuSceneController.UpdatePhoto(path);
	}
}
