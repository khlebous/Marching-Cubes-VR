using UniRx;
using UnityEngine;

public class MainModeController : MonoBehaviour, IModeController
{
	[SerializeField] private MainMenuController mainMenuController;

	protected ISubject<Unit> itemSelectedSubject = new Subject<Unit>();
	public IObservable<Unit> ItemSelectedStream { get { return itemSelectedSubject; } }

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
