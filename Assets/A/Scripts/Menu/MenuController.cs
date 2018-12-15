using System.Collections;
using UnityEngine;

using UniRx;

public class MenuController : MonoBehaviour
{
	[SerializeField] private HandMenuController menuController;

	[Header("Input")]
	[SerializeField] private OVRInput.Button showMenuButton = OVRInput.Button.SecondaryThumbstickLeft;
	[SerializeField] private OVRInput.Button hideMenuButton = OVRInput.Button.SecondaryThumbstickRight;

	private Coroutine waitForOpenCoroutine;
	private Coroutine waitForCloseCoroutine;

	void Start()
	{
		menuController.ItemIsActiveSubject.Subscribe(SubItem);
		menuController.CloseMenu();
		StartWaitForOpen();
	}

	private void SubItem(bool value)
	{
		if (value)
			StopListening();
		else
			StartWaitForClose();
	}

	private void StartWaitForOpen()
	{
		StopListening();
		waitForOpenCoroutine = StartCoroutine(WaitForOpenMenu());
	}

	public void StartWaitForClose()
	{
		StopListening();
		waitForCloseCoroutine = StartCoroutine(WaitForCloseMenu());
	}

	private void StopListening()
	{
		if (null != waitForOpenCoroutine)
			StopCoroutine(waitForOpenCoroutine);

		if (null != waitForCloseCoroutine)
			StopCoroutine(waitForCloseCoroutine);
	}

	private IEnumerator WaitForOpenMenu()
	{
		while (true)
		{
			if (OVRInput.Get(showMenuButton))
			{
				StopCoroutine(waitForOpenCoroutine);
				menuController.OpenMenu();
				waitForCloseCoroutine = StartCoroutine(WaitForCloseMenu());
			}

			yield return new WaitForEndOfFrame();
		}
	}

	private IEnumerator WaitForCloseMenu()
	{
		while (true)
		{
			if (OVRInput.Get(hideMenuButton))
			{
				StopCoroutine(waitForCloseCoroutine);
				menuController.CloseMenu();
				waitForOpenCoroutine = StartCoroutine(WaitForOpenMenu());
			}

			yield return new WaitForEndOfFrame();
		}
	}
}