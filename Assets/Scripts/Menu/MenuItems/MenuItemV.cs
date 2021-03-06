﻿using UnityEngine;
using UniRx;
using System.Collections;

public class MenuItemV : MonoBehaviour
{
	[SerializeField] protected MultiStateMaterial highlight;

	[Header("Input")]
	[SerializeField] private OVRInput.RawButton stopEditingItemButton = OVRInput.RawButton.RThumbstick;

	protected ISubject<Unit> thubstickClickedSubject = new Subject<Unit>();
	public IObservable<Unit> ThubstickClickedStream { get { return thubstickClickedSubject; } }

	private Coroutine waitForEndEditing;

	public virtual void SetActive()
	{
		highlight.SetState(1);
	}

	public virtual void SetInactive()
	{
		highlight.SetState(0);
	}

	// TODO choosable items, this logic should not be here
	public virtual void SetChosen()
	{
		highlight.SetState(2);
		waitForEndEditing = StartCoroutine(WaitForEndEditing());
	}

	private IEnumerator WaitForEndEditing()
	{
		yield return null;
		while (true)
		{
			if (OVRInput.GetDown(stopEditingItemButton))
			{
				if (waitForEndEditing != null)
					StopCoroutine(waitForEndEditing);
				SetNormal();
			}

			yield return new WaitForEndOfFrame();
		}
	}

	public virtual void SetNormal()
	{
		highlight.SetState(1);
		thubstickClickedSubject.OnNext(Unit.Default);
	}
}
