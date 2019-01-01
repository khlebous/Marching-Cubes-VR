using UniRx;
using UnityEngine;
using System;
public class MainModeController : MonoBehaviour, IModeController
{
	[SerializeField] private MainMenuController mainMenuController;

	protected ISubject<Guid> itemSelectedSubject = new Subject<Guid>();
	public IObservable<Guid> ItemSelectedStream { get { return itemSelectedSubject; } }

	private void Start()
	{
		mainMenuController.ItemSelectedStream.Subscribe(itemSelectedSubject.OnNext);
	}

	public void TurnOn()
	{
		mainMenuController.SetActive();
	}

	public void TurnOff()
	{
		mainMenuController.SetInactive();
	}
}
