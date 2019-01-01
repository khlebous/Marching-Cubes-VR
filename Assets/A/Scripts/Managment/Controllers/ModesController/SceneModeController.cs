using UniRx;
using UnityEngine;
using System;

public class SceneModeController : MonoBehaviour
{
	[SerializeField] private GameObject sceneContiner;
	[SerializeField] private MenuSceneController menuSceneController;

	protected ISubject<Unit> exitToMainModeSubject = new Subject<Unit>();
	public IObservable<Unit> ExitToMainModeStream { get { return exitToMainModeSubject; } }

	protected ISubject<Unit> saveAndExitToMainModeSubject = new Subject<Unit>();
	public IObservable<Unit> SaveAndExitToMainModeStream { get { return saveAndExitToMainModeSubject; } }

	private void Start()
	{
		menuSceneController.ExitToMainModeStream.Subscribe(exitToMainModeSubject.OnNext);
		menuSceneController.SaveAndExitToMainModeStream.Subscribe(saveAndExitToMainModeSubject.OnNext);
	}

	public void TurnOn(Guid guid)
	{
		Debug.Log("SceneModeController  turn on");
		Debug.Log("TODO load scene: "+ guid.ToString());
		sceneContiner.SetActive(true);
		menuSceneController.SetActive();
	}

	public void ExitMode()
	{
		Debug.Log("SceneModeController  turn off");
		sceneContiner.SetActive(false);
		menuSceneController.SetInactive();
	}

	internal void SaveSceneAndExitMode()
	{
		Debug.Log("TODO save scene");
		ExitMode();
	}
}
