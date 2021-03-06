﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;

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

	private ISubject<Guid> modelToAddSelectedSubject = new Subject<Guid>();
	public IObservable<Guid> ModelToAddSelectedStream { get { return modelToAddSelectedSubject; } }

	private ISubject<Guid> modelToEditSelectedSubject = new Subject<Guid>();
	public IObservable<Guid> ModelToEditSelectedStream { get { return modelToEditSelectedSubject; } }

	private ISubject<Guid> modelToDeleteSelectedSubject = new Subject<Guid>();
	public IObservable<Guid> ModelToDeleteSelectedStream { get { return modelToDeleteSelectedSubject; } }

	protected ISubject<Unit> photoRequesSubject = new Subject<Unit>();
	public IObservable<Unit> PhotoRequestStream { get { return photoRequesSubject; } }

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

		menuLeftController.SaveAndExitToMainModeStream.Subscribe(_ => SetInactive());
		menuLeftController.SaveAndExitToMainModeStream.Subscribe(saveAndExitToMainModeSubject.OnNext);

		menuLeftController.ExitToMainModeStream.Subscribe(_ => SetInactive());
		menuLeftController.ExitToMainModeStream.Subscribe(exitToMainModeSubject.OnNext);

		menuLeftController.PhotoRequestStream.Subscribe(photoRequesSubject.OnNext);

		menuRightController.ExitToTerrainModeStream.Subscribe(_ => Suspend());
		menuRightController.ExitToTerrainModeStream.Subscribe(suspendAndExitToTerrainModeSubject.OnNext);

		menuRightController.ExitToObjectModeStream.Subscribe(_ => Suspend());
		menuRightController.ExitToObjectModeStream.Subscribe(suspendAndExitToObjectModeSubject.OnNext);

		menuRightController.ModelToAddSelectedStream.Subscribe(modelToAddSelectedSubject.OnNext);
		menuRightController.ModelToEditSelectedStream.Subscribe(modelToEditSelectedSubject.OnNext);
		menuRightController.ModelToDeleteSelectedStream.Subscribe(modelToDeleteSelectedSubject.OnNext);
		menuRightController.ItemSelectedStream.Subscribe(_ => StopWaitForMenuRightClose());

		StartWaitForMenuOpen();
	}

	private void StartWaitForMenuOpen()
	{
		StopListening();
		waitForMenuLeftOpenCoroutine = StartCoroutine(WaitForOpenMenuLeft());
		waitForMenuRightOpenCoroutine = StartCoroutine(WaitForOpenMenuRight());
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

	public void ResetMenus()
	{
		menuLeftController.CloseMenu();
		menuRightController.CloseMenu();
		menuLeftController.ResetMenu();
		menuRightController.ResetMenu();

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

	private void Suspend()
	{
		// TODO same as inactive
		StopListening();
		menuLeftController.CloseMenu();
		menuRightController.CloseMenu();
		gameObject.SetActive(false);
	}

	public void UpdateModelsGuids(Guid sceneGuid, List<Guid> modelGuids)
	{
		menuRightController.UpdateModelsGuids(sceneGuid, modelGuids);
	}


	public void StartWaitForMenuRightClose()
	{
		StopListening();
		waitForMenuRightCloseCoroutine = StartCoroutine(WaitForCloseMenuRight());
	}

	public void StopWaitForMenuRightClose()
	{
		if (null != waitForMenuRightCloseCoroutine)
			StopCoroutine(waitForMenuRightCloseCoroutine);
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


	public void UpdatePhoto(string path)
	{
		menuLeftController.UpdatePhoto(path);
	}
}
