using UnityEngine;
using UniRx;

public class ObjectModeController : MonoBehaviour
{
	[SerializeField] private GameObject objectContiner;
	[SerializeField] private MenuObjectController menuObjectController;

	protected ISubject<Unit> modeExitedSubject = new Subject<Unit>();
	public IObservable<Unit> ModeExitedStream { get { return modeExitedSubject; } }

	private void Start()
	{
		menuObjectController.ExitToSceneModeStream.Subscribe(_ => ExitMode());
		menuObjectController.SaveAndExitToSceneModeStream.Subscribe(_ => SaveObjectAndExitMode());
	}

	public void TurnOnMode()
	{
		Debug.Log("TerrainModeController  turn on");
		Debug.Log("TODO smth: ");
		objectContiner.SetActive(true);
		menuObjectController.SetActive();
	}

	private void ExitMode()
	{
		Debug.Log("SceneModeController  turn off");
		objectContiner.SetActive(false);
		menuObjectController.SetInactive();

		modeExitedSubject.OnNext(Unit.Default);
	}

	private void SaveObjectAndExitMode()
	{
		Debug.Log("TODO save scene");
		ExitMode();
	}
}
