using UnityEngine;
using UniRx;
using System;

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
		menuModelController.UpdatePhoto(GetFullPath());
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

		modeExitedSubject.OnNext(Unit.Default);
	}

	private void SaveObjectAndExitMode()
	{
		brush.SetInactive();
		modelContiner.SetActive(false);
		menuModelController.SetInactive();

		mcManager.Save(model, sceneGuid);

		modeSavedAndExitedSubject.OnNext(model.GetData());
		model.Destroy();
		model = null;
	}

	private void TakePhoto()
	{
		string fullPath = GetFullPath();
		var fileInfo = new System.IO.FileInfo(fullPath);
		fileInfo.Directory.Create();

		cameraCapture.TakeShot(fullPath);
		menuModelController.UpdatePhoto(GetFullPath());
	}

	private string GetFullPath()
	{
		return Application.dataPath + "/Resources/" + sceneGuid.ToString()
			+ "/Models/" + model.Guid.ToString() + ".png";
	}
}
