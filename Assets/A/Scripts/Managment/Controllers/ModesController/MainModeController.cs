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
		mainMenuController.ItemSelectedStream.Subscribe(itemSelectedSubject.OnNext);
	}
	
	public void TurnOnModeWithCurrentSceneGuids(List<Guid> sceneGuids)
	{
		mainMenuController.SetActive(sceneGuids);
	}

	public void TurnOff()
	{
		mainMenuController.SetInactive();
	}
}
