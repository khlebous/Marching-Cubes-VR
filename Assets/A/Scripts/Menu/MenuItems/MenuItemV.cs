using System.Collections;
using UnityEngine;

using UniRx;
using System;

public class MenuItemV : MonoBehaviour
{
	[SerializeField] MultiStateMaterial highlight;

	protected ISubject<Unit> thubstickClickedSubject = new Subject<Unit>();
	public IObservable<Unit> ThubstickClickedStream { get { return thubstickClickedSubject; } }

	private Coroutine waitForEndEditing;

	public virtual void SetInactive()
	{
		if (highlight != null)
			highlight.SetState(0);
	}

	public virtual void SetActive()
	{
		highlight.SetState(1);
	}

	public virtual void SetChoosen()
	{
		Debug.Log("start editing item");

		highlight.SetState(2);
		waitForEndEditing = StartCoroutine(WaitForEndEditing());
	}

	private IEnumerator WaitForEndEditing()
	{
		yield return 0;

		while (true)
		{
			if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick))
			{
				Debug.Log("end edithing item");
				SetUnChoosen();
			}

			yield return new WaitForEndOfFrame();
		}
	}


	public virtual void SetUnChoosen()
	{
		if (waitForEndEditing != null)
		{
			Debug.Log("stop courutine");
			StopCoroutine(waitForEndEditing);
		}

		Debug.Log("unchoosen");
		highlight.SetState(1);
		thubstickClickedSubject.OnNext(Unit.Default);
	}

}
