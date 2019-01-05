﻿using UnityEngine;
using UniRx;

public class ModelModeController : MonoBehaviour
{
	[SerializeField] private GameObject modelContiner;
	[SerializeField] private Assets.MarchingCubesGPU.Scripts.ModelBrush brush;
	[SerializeField] private MenuModelController menuModelController;

	protected ISubject<Unit> modeExitedSubject = new Subject<Unit>();
	public IObservable<Unit> ModeExitedStream { get { return modeExitedSubject; } }

	private void Start()
	{
		menuModelController.ExitToSceneModeStream.Subscribe(_ => ExitMode());
		menuModelController.SaveAndExitToSceneModeStream.Subscribe(_ => SaveObjectAndExitMode());
	}

	public void TurnOnMode()
	{
		Debug.Log("TerrainModeController  turn on");
		Debug.Log("TODO smth: ");
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

		modeExitedSubject.OnNext(Unit.Default);
	}

	private void SaveObjectAndExitMode()
	{
		Debug.Log("TODO save scene");
		ExitMode();
	}
}
