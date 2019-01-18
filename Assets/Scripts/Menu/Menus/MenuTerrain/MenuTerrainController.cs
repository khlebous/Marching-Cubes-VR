using UnityEngine;
using System.Collections;
using UniRx;

public class MenuTerrainController : MonoBehaviour
{
	[SerializeField] private MenuLeftTerrainController menuLeftController;
	[SerializeField] private MenuRightTerrainController menuRightController;

	protected ISubject<Unit> exitToSceneModeSubject = new Subject<Unit>();
	public IObservable<Unit> ExitToSceneModeStream { get { return exitToSceneModeSubject; } }

	protected ISubject<Unit> saveAndExitToSceneModeSubject = new Subject<Unit>();
	public IObservable<Unit> SaveAndExitToSceneModeStream { get { return saveAndExitToSceneModeSubject; } }

	private OVRInput.RawButton showMenuLeftButton = OVRInput.RawButton.LThumbstickRight;
	private OVRInput.RawButton hideMenuLeftButton = OVRInput.RawButton.LThumbstickLeft;
	private OVRInput.RawButton showMenuRightButton = OVRInput.RawButton.RThumbstickLeft;
	private OVRInput.RawButton hideMenuRightButton = OVRInput.RawButton.RThumbstickRight;

	private Coroutine waitForMenuLeftOpenCoroutine;
	private Coroutine waitForMenuLeftCloseCoroutine;
	private Coroutine waitForMenuRightOpenCoroutine;
	private Coroutine waitForMenuRightCloseCoroutine;

	void Start()
	{
		menuLeftController.CloseMenu();
		menuRightController.CloseMenu();

		menuLeftController.SaveAndExitToSceneModeStream.Subscribe(_ => SetInactive());
		menuLeftController.SaveAndExitToSceneModeStream.Subscribe(saveAndExitToSceneModeSubject.OnNext);

		menuLeftController.ExitToSceneModeStream.Subscribe(_ => SetInactive());
		menuLeftController.ExitToSceneModeStream.Subscribe(exitToSceneModeSubject.OnNext);

		menuRightController.ItemIsActiveStream.Subscribe(_ => StopListeningForRight());
		menuRightController.ItemNotActiveStream.Subscribe(_ => StartListeningForRight());

		StartWaitForMenuOpen();
	}

	private void StartWaitForMenuOpen()
	{
		StopListening();
		waitForMenuLeftOpenCoroutine = StartCoroutine(WaitForOpenMenuLeft());
		waitForMenuRightOpenCoroutine = StartCoroutine(WaitForOpenMenuRight());
	}

	private void StartListeningForRight()
	{
		waitForMenuRightCloseCoroutine = StartCoroutine(WaitForCloseMenuRight());
	}

	private void StopListening()
	{
		StopListeningForLeft();
		StopListeningForRight();
	}

	private void StopListeningForLeft()
	{
		if (null != waitForMenuLeftOpenCoroutine)
			StopCoroutine(waitForMenuLeftOpenCoroutine);

		if (null != waitForMenuLeftCloseCoroutine)
			StopCoroutine(waitForMenuLeftCloseCoroutine);
	}

	private void StopListeningForRight()
	{
		if (null != waitForMenuRightOpenCoroutine)
			StopCoroutine(waitForMenuRightOpenCoroutine);

		if (null != waitForMenuRightCloseCoroutine)
			StopCoroutine(waitForMenuRightCloseCoroutine);
	}


	private IEnumerator WaitForOpenMenuLeft()
	{
		while (true)
		{
			if (OVRInput.Get(showMenuLeftButton))
			{
				StopCoroutine(waitForMenuLeftOpenCoroutine);
				menuLeftController.OpenMenu();
				waitForMenuLeftCloseCoroutine = StartCoroutine(WaitForCloseMenuLeft());
			}

			yield return new WaitForEndOfFrame();
		}
	}

	private IEnumerator WaitForCloseMenuLeft()
	{
		while (true)
		{
			if (OVRInput.Get(hideMenuLeftButton))
			{
				StopCoroutine(waitForMenuLeftCloseCoroutine);
				menuLeftController.CloseMenu();
				waitForMenuLeftOpenCoroutine = StartCoroutine(WaitForOpenMenuLeft());
			}

			yield return new WaitForEndOfFrame();
		}
	}

	private IEnumerator WaitForOpenMenuRight()
	{
		while (true)
		{
			if (OVRInput.Get(showMenuRightButton))
			{
				StopCoroutine(waitForMenuRightOpenCoroutine);
				menuRightController.OpenMenu();
				waitForMenuRightCloseCoroutine = StartCoroutine(WaitForCloseMenuRight());
			}

			yield return new WaitForEndOfFrame();
		}
	}

	private IEnumerator WaitForCloseMenuRight()
	{
		while (true)
		{
			if (OVRInput.Get(hideMenuRightButton))
			{
				StopCoroutine(waitForMenuRightCloseCoroutine);
				menuRightController.CloseMenu();
				waitForMenuRightOpenCoroutine = StartCoroutine(WaitForOpenMenuRight());
			}

			yield return new WaitForEndOfFrame();
		}
	}


	public void SetActive()
	{
		menuLeftController.CloseMenu();
		menuRightController.CloseMenu();
		gameObject.SetActive(true);
		StartWaitForMenuOpen();
	}

	public void SetInactive()
	{
		StopListening();
		menuLeftController.CloseMenu();
		menuRightController.CloseMenu();
		gameObject.SetActive(false);
	}

	public void ResetMenus()
	{
		gameObject.SetActive(true);

		menuLeftController.CloseMenu();
		menuRightController.CloseMenu();
		menuLeftController.ResetMenu();
		menuRightController.ResetMenu();

		StartWaitForMenuOpen();
	}
}
