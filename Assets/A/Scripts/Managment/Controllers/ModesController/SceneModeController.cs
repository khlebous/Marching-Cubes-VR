using UniRx;
using UnityEngine;

public class SceneModeController : MonoBehaviour, IModeController
{
	[SerializeField] private GameObject sceneGO;
	[SerializeField] private MenuSceneController menuSceneController;

	protected ISubject<Unit> exitToMainModeSubject = new Subject<Unit>();
	public IObservable<Unit> ExitToMainModeStream { get { return exitToMainModeSubject; } }

	private void Start()
	{
		menuSceneController.ModeExitedStream.Subscribe(exitToMainModeSubject.OnNext);
	}

	public void TurnOn()
	{
		Debug.Log("SceneModeController  turn on");
		sceneGO.SetActive(true);
		menuSceneController.SetActive();
	}

	public void TurnOff()
	{
		Debug.Log("SceneModeController  turn off");
		sceneGO.SetActive(false);
		menuSceneController.SetInactive();
	}
}
