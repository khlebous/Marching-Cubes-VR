using System.Collections;
using UnityEngine;
using UniRx;

public class MenuSceneController : MonoBehaviour
{
	[SerializeField] private MenuLeftSceneController menuLeftController;
	[SerializeField] private MenuRightSceneController menuRightController;

	[Header("Input")]
	[SerializeField] private OVRInput.Button showMenuLeftButton = OVRInput.Button.PrimaryThumbstickRight;
	[SerializeField] private OVRInput.Button hideMenuLeftButton = OVRInput.Button.PrimaryThumbstickLeft;
	[SerializeField] private OVRInput.Button showMenuRightButton = OVRInput.Button.SecondaryThumbstickLeft;
	[SerializeField] private OVRInput.Button hideMenuRightButton = OVRInput.Button.SecondaryThumbstickRight;

	[Header("Other")]
	[SerializeField] private GameObject sceneGO;

	protected ISubject<Unit> modeExitedSubject = new Subject<Unit>();
	public IObservable<Unit> ModeExitedStream { get { return modeExitedSubject; } }

	private Coroutine waitForMenuLeftOpenCoroutine;
	private Coroutine waitForMenuLeftCloseCoroutine;
	private Coroutine waitForMenuRightOpenCoroutine;
	private Coroutine waitForMenuRightCloseCoroutine;

	void Start()
	{
		menuLeftController.CloseMenu();
		menuRightController.CloseMenu();

		menuLeftController.ThubstickClickedStream.Subscribe(_ => SetInactive());
		menuLeftController.ThubstickClickedStream.Subscribe(modeExitedSubject.OnNext);

		StartWaitForMenuOpen();
	}

	public void SetInactive()
	{
		StopListening();
		menuLeftController.CloseMenu();
		menuRightController.CloseMenu();
		gameObject.SetActive(false);
	}

	public void SetActive()
	{
		menuLeftController.CloseMenu();
		menuRightController.CloseMenu();
		gameObject.SetActive(true);
		sceneGO.SetActive(true);
		StartWaitForMenuOpen();
	}

	private void StartWaitForMenuOpen()
	{
		StopListening();
		waitForMenuLeftOpenCoroutine = StartCoroutine(WaitForOpenMenuLeft());
		waitForMenuRightOpenCoroutine = StartCoroutine(WaitForOpenMenuRight());
	}

	public void StartWaitForMenuRightClose()
	{
		StopListening();
		waitForMenuRightCloseCoroutine = StartCoroutine(WaitForCloseMenuRight());
	}

	public void StartWaitForMenuLeftClose()
	{
		StopListening();
		waitForMenuLeftCloseCoroutine = StartCoroutine(WaitForCloseMenuLeft());
	}

	private void StopListening()
	{
		if (null != waitForMenuLeftOpenCoroutine)
			StopCoroutine(waitForMenuLeftOpenCoroutine);

		if (null != waitForMenuLeftCloseCoroutine)
			StopCoroutine(waitForMenuLeftCloseCoroutine);

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
}
