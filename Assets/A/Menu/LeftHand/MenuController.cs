using System.Collections;
using UnityEngine;

public class MenuController : MonoBehaviour
{
	[SerializeField] private HandMenuController menuController;

	private Coroutine waitForOpenCoroutine;
	private Coroutine waitForCloseCoroutine;

	void Start()
	{
		menuController.CloseMenu();

		StartWaitForOpen();
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
			if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft))
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
			if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight))
			{
				StopCoroutine(waitForCloseCoroutine);
				menuController.CloseMenu();
				waitForOpenCoroutine = StartCoroutine(WaitForOpenMenu());
			}

			yield return new WaitForEndOfFrame();
		}
	}
}