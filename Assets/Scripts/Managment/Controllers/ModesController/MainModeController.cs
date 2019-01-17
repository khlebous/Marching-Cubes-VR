using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class MainModeController : MonoBehaviour
{
	[SerializeField] private GameObject mainModeContiner;
	[SerializeField] private MainMenuController mainMenuController;

	protected ISubject<Guid> itemSelectedSubject = new Subject<Guid>();
	public IObservable<Guid> ItemSelectedStream { get { return itemSelectedSubject; } }

	
	private void Start()
	{
		mainMenuController.ItemSelectedStream.Subscribe(ExitModeLoadSceneModeWithGuid);
	}

	public void ExitModeLoadSceneModeWithGuid(Guid guid)
	{
		mainModeContiner.SetActive(false);
		mainMenuController.SetInactive();
		itemSelectedSubject.OnNext(guid);
	}
	
	public void TurnOnModeWithCurrentSceneGuids(List<Guid> sceneGuids)
	{
		mainModeContiner.SetActive(true);
		mainMenuController.ResetMenu(sceneGuids);
	}
}
