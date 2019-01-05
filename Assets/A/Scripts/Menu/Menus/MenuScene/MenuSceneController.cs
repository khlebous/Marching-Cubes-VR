﻿using System.Collections;
using UnityEngine;
using UniRx;
using System;

public class MenuSceneController : MonoBehaviour
{
	[SerializeField] private MenuLeftSceneController menuLeftController;
	[SerializeField] private MenuRightSceneController menuRightController;

	protected ISubject<Unit> exitToMainModeSubject = new Subject<Unit>();
	public IObservable<Unit> ExitToMainModeStream { get { return exitToMainModeSubject; } }

	protected ISubject<Unit> saveAndExitToMainModeSubject = new Subject<Unit>();
	public IObservable<Unit> SaveAndExitToMainModeStream { get { return saveAndExitToMainModeSubject; } }

	protected ISubject<Unit> suspendAndExitToTerrainModeSubject = new Subject<Unit>();
	public IObservable<Unit> SuspendAndExitToTerrainModeStream { get { return suspendAndExitToTerrainModeSubject; } }

	protected ISubject<Guid> suspendAndExitToObjectModeSubject = new Subject<Guid>();
	public IObservable<Guid> SuspendAndExitToObjectModeStream { get { return suspendAndExitToObjectModeSubject; } }

	private OVRInput.Button showMenuLeftButton = OVRInput.Button.PrimaryThumbstickRight;
	private OVRInput.Button hideMenuLeftButton = OVRInput.Button.PrimaryThumbstickLeft;
	private OVRInput.Button showMenuRightButton = OVRInput.Button.SecondaryThumbstickLeft;
	private OVRInput.Button hideMenuRightButton = OVRInput.Button.SecondaryThumbstickRight;

	private Coroutine waitForMenuLeftOpenCoroutine;
	private Coroutine waitForMenuLeftCloseCoroutine;
	private Coroutine waitForMenuRightOpenCoroutine;
	private Coroutine waitForMenuRightCloseCoroutine;

	void Start()
	{
		menuLeftController.CloseMenu();
		menuRightController.CloseMenu();

		menuLeftController.SaveAndExitToMainModeStream.Subscribe(_ => SetInactive());
		menuLeftController.SaveAndExitToMainModeStream.Subscribe(saveAndExitToMainModeSubject.OnNext);

		menuLeftController.ExitToMainModeStream.Subscribe(_ => SetInactive());
		menuLeftController.ExitToMainModeStream.Subscribe(exitToMainModeSubject.OnNext);

		menuRightController.ExitToTerrainModeStream.Subscribe(_ => Suspend());
		menuRightController.ExitToTerrainModeStream.Subscribe(suspendAndExitToTerrainModeSubject.OnNext);

		menuRightController.ExitToObjectModeStream.Subscribe(_ => Suspend());
		menuRightController.ExitToObjectModeStream.Subscribe(suspendAndExitToObjectModeSubject.OnNext);

		StartWaitForMenuOpen();
	}

	private void Suspend()
	{
		// TODO same as inactive
		StopListening();
		menuLeftController.CloseMenu();
		menuRightController.CloseMenu();
		gameObject.SetActive(false);
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