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

    [SerializeField] private Transform oculusBase;

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
		model.transform.localPosition = new Vector3(0, 0, 0);
        model.transform.localRotation = Quaternion.Euler(Vector3.zero);
		model.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);

        modelContiner.transform.rotation = oculusBase.rotation;
        var positionOffset = oculusBase.forward * McConsts.ModelN * model.transform.lossyScale.x + oculusBase.up;
        modelContiner.transform.position = oculusBase.position + positionOffset;

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

		string tmpPath = PathHelper.GetModelTmpPngPath(sceneGuid);
		if (File.Exists(tmpPath))
			File.Delete(tmpPath);

		modeExitedSubject.OnNext(Unit.Default);
	}

	private void SaveObjectAndExitMode()
	{
		mcManager.Save(model, sceneGuid);
		brush.SetInactive();
		modelContiner.SetActive(false);
		menuModelController.SetInactive();

		string path = PathHelper.GetModelPngPath(model.Guid, sceneGuid);
		string tmpPath = PathHelper.GetModelTmpPngPath(sceneGuid);

		if (File.Exists(tmpPath))
		{
			if (File.Exists(path))
				File.Delete(path);
			File.Move(tmpPath, path);
		}

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
