using UnityEngine;
using UniRx;
using System;
using System.IO;

public class ModelModeController : MonoBehaviour
{
	[SerializeField] private GameObject modelContiner;
	[SerializeField] private Assets.MarchingCubesGPU.Scripts.ModelBrush brush;
	[SerializeField] private MenuModelController menuModelController;

	[Header("Other")]
	[SerializeField] private McManager mcManager;
	[SerializeField] private CameraCapture cameraCapture;

	protected ISubject<Unit> modeExitedSubject = new Subject<Unit>();
	public IObservable<Unit> ModeExitedStream { get { return modeExitedSubject; } }

	protected ISubject<McData> modeSavedAndExitedSubject = new Subject<McData>();
	public IObservable<McData> ModeSavedAndExitedStream { get { return modeSavedAndExitedSubject; } }

	Guid sceneGuid;
	MarchingCubesGPUProject.EditableModel model;


	private void Start()
	{
		menuModelController.ExitToSceneModeStream.Subscribe(_ => ExitMode());
		menuModelController.SaveAndExitToSceneModeStream.Subscribe(_ => SaveObjectAndExitMode());
		menuModelController.PhotoRequestStream.Subscribe(_ => TakePhoto());
	}


	public void TurnOnMode(LoadData loadData)
	{
		sceneGuid = loadData.sceneGuid;

		if (loadData.data == null)
			model = mcManager.LoadModel(mcManager.CreateModel());
		else
			model = mcManager.LoadModel(loadData.data);

		model.gameObject.transform.parent = modelContiner.transform;
		model.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);

		modelContiner.SetActive(true);
		menuModelController.UpdatePhoto(PathHelper.GetModelPngPath(model.Guid, sceneGuid));
		menuModelController.ResetMenus();
		brush.SetActive();
	}

	private void ExitMode()
	{
		brush.SetInactive();
		modelContiner.SetActive(false);
		menuModelController.SetInactive();

		model.Destroy();
		model = null;

		string path = PathHelper.GetModelTmpPngPath(sceneGuid);
		File.Delete(path);

		modeExitedSubject.OnNext(Unit.Default);
	}

	private void SaveObjectAndExitMode()
	{
		mcManager.Save(model, sceneGuid);
		brush.SetInactive();
		modelContiner.SetActive(false);
		menuModelController.SetInactive();

		string path = PathHelper.GetModelPngPath(model.Guid, sceneGuid);
		File.Delete(path);
		string tmpPath = PathHelper.GetModelTmpPngPath(sceneGuid);
		File.Move(tmpPath, path);

		modeSavedAndExitedSubject.OnNext(model.GetData());
		model.Destroy();
		model = null;
	}

	private void TakePhoto()
	{
		string path = PathHelper.GetModelTmpPngPath(sceneGuid);
		PathHelper.EnsureDirForFileExists(path);

		cameraCapture.TakeShot(path);
		menuModelController.UpdatePhoto(path);
	}
}
