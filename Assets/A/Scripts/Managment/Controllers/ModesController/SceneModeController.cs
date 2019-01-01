using UniRx;
using UnityEngine;
using System;

public class SceneModeController : MonoBehaviour/*, IModeController*/
{
	[SerializeField] private GameObject sceneContiner;
	[SerializeField] private MenuSceneController menuSceneController;

	protected ISubject<Unit> exitToMainModeSubject = new Subject<Unit>();
	public IObservable<Unit> ExitToMainModeStream { get { return exitToMainModeSubject; } }

	private void Start()
	{
		menuSceneController.ModeExitedStream.Subscribe(exitToMainModeSubject.OnNext);
	}

	public void TurnOn(Guid guid)
	{
		Debug.Log("SceneModeController  turn on");
		sceneContiner.SetActive(true);
		menuSceneController.SetActive();
	}

	public void TurnOff()
	{
		Debug.Log("SceneModeController  turn off");
		sceneContiner.SetActive(false);
		menuSceneController.SetInactive();
	}
}
