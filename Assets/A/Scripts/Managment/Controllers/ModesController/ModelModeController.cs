using UnityEngine;
using UniRx;

public class ModelModeController : MonoBehaviour
{
	[SerializeField] private GameObject modelContiner;
	[SerializeField] private Assets.MarchingCubesGPU.Scripts.ModelBrush brush;
	[SerializeField] private MenuModelController menuModelController;

	[Header("Other")]
	[SerializeField] private McManager mcManager;

	protected ISubject<Unit> modeExitedSubject = new Subject<Unit>();
	public IObservable<Unit> ModeExitedStream { get { return modeExitedSubject; } }

	MarchingCubesGPUProject.EditableModel model;

	private void Start()
	{
		menuModelController.ExitToSceneModeStream.Subscribe(_ => ExitMode());
		menuModelController.SaveAndExitToSceneModeStream.Subscribe(_ => SaveObjectAndExitMode());
	}

	public void TurnOnMode()
	{
		Debug.Log("TerrainModeController  turn on");

		model = mcManager.LoadModel(mcManager.CreateModel());
		model.gameObject.transform.parent = modelContiner.transform;
		model.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

		modelContiner.SetActive(true);
		menuModelController.SetActive();
		brush.SetActive();
	}

	private void ExitMode()
	{
		Debug.Log("SceneModeController  turn off");
		brush.SetInactive();
		modelContiner.SetActive(false);
		menuModelController.SetInactive();

		model.Destroy();
		model = null;

		modeExitedSubject.OnNext(Unit.Default);
	}

	private void SaveObjectAndExitMode()
	{
		Debug.Log("TODO save scene");
		ExitMode();
	}
}
