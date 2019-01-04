using System;
using UnityEngine;
using UniRx;

public class SceneModeController : MonoBehaviour
{
	[SerializeField] private GameObject sceneContiner;
	[SerializeField] private MenuSceneController menuSceneController;

	protected ISubject<Unit> exitToMainModeSubject = new Subject<Unit>();
	public IObservable<Unit> ExitToMainModeStream { get { return exitToMainModeSubject; } }

	protected ISubject<Unit> exitToTerrainModeSubject = new Subject<Unit>();
	public IObservable<Unit> ExitToTerrainModeStream { get { return exitToTerrainModeSubject; } }

	protected ISubject<Unit> exitToObjectModeSubject = new Subject<Unit>();
	public IObservable<Unit> ExitToObjectModeStream { get { return exitToObjectModeSubject; } }

	private Guid currentGuid;

	private void Start()
	{
		menuSceneController.ExitToMainModeStream.Subscribe(_ => ExitToMainMode());
		menuSceneController.SaveAndExitToMainModeStream.Subscribe(_ => SaveSceneAndExitToMainMode());
		menuSceneController.SuspendAndExitToTerrainModeStream.Subscribe(_ => SuspendAndExitToTerrainMode());
		menuSceneController.SuspendAndExitToObjectModeStream.Subscribe(_ => SuspendAndExitToObjectMode());
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

	private void SuspendAndExitToTerrainMode()
	{
		Debug.Log("terrain sleep");
		sceneContiner.SetActive(false);
		menuSceneController.SetInactive();

		exitToTerrainModeSubject.OnNext(Unit.Default);
	}

	private void SuspendAndExitToObjectMode()
	{
		Debug.Log("terrain sleep");
		sceneContiner.SetActive(false);
		menuSceneController.SetInactive();

		exitToObjectModeSubject.OnNext(Unit.Default);
	}
}
