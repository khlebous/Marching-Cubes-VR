using System;
using UnityEngine;
using UniRx;

public class SceneModeController : MonoBehaviour
{
	[SerializeField] private GameObject sceneContiner;
	[SerializeField] private MenuSceneController menuSceneController;

	protected ISubject<Unit> exitToMainModeSubject = new Subject<Unit>();
	public IObservable<Unit> ExitToMainModeStream { get { return exitToMainModeSubject; } }

	private Guid currentGuid;

	private void Start()
	{
		menuSceneController.ExitToMainModeStream.Subscribe(_ => ExitToMainMode());
		menuSceneController.SaveAndExitToMainModeStream.Subscribe(_ => SaveSceneAndExitToMainMode());
	}

	public void TurnOnModeWith(Guid guid)
	{
		Debug.Log("SceneModeController  turn on");
		Debug.Log("TODO load scene: "+ guid.ToString());
		sceneContiner.SetActive(true);
		menuSceneController.SetActive();
	}

	private void ExitToMainMode()
	{
		Debug.Log("SceneModeController  turn off");
		sceneContiner.SetActive(false);
		menuSceneController.SetInactive();

		currentGuid = Guid.Empty; //?

		exitToMainModeSubject.OnNext(Unit.Default);
	}

	private void SaveSceneAndExitToMainMode()
	{
		Debug.Log("TODO save scene");
		ExitToMainMode();
	}

	public void TurnOnCurrentMode()
	{
		TurnOnModeWith(currentGuid);
		Debug.Log("TODO load current");
	}
}
