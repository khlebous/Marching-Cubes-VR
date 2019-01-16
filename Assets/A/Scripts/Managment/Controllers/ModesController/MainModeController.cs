using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class MainModeController : MonoBehaviour
{
	[SerializeField] private MainMenuController mainMenuController;


	protected ISubject<Guid> itemSelectedSubject = new Subject<Guid>();
	public IObservable<Guid> ItemSelectedStream { get { return itemSelectedSubject; } }

	
	private void Start()
	{
		mainMenuController.ItemSelectedStream.Subscribe(ExitModeLoadSceneModeWithGuid);
	}

	public void ExitModeLoadSceneModeWithGuid(Guid guid)
	{
		mainMenuController.SetInactive();
		itemSelectedSubject.OnNext(guid);
	}
	
	public void TurnOnModeWithCurrentSceneGuids(List<Guid> sceneGuids)
	{
		mainMenuController.ResetMenu(sceneGuids);
	}
}
