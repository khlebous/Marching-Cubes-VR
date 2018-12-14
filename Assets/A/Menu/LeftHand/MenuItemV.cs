using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using System;

public class MenuItemV : MonoBehaviour
{
	[SerializeField] MultiStateMaterial highlight;

	protected ISubject<bool> thubstickClickedSubject = new Subject<bool>();
	public IObservable<bool> ThubstickClickedStream { get { return thubstickClickedSubject; } }

	//public void Start()
	//{
	//    submenu.SetActive(false);
	//    submenu.ThubstickClickedStream.Subscribe(thubstickClickedSubject.OnNext);
	//}

	private Coroutine WaitForSubmit;


	public void OnClick()
	{
		// highlight submenu
	}

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
		Debug.Log("set choosen");

		highlight.SetState(2);
		WaitForSubmit = StartCoroutine(WaitForButtonA_Down());
	}

	private IEnumerator WaitForButtonA_Down()
	{
		yield return new WaitForSeconds(0.5f);

		while (true)
		{
			if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick))
			{
				Debug.Log("thubm");
				SetUnChoosen();
			}

			yield return new WaitForEndOfFrame();
		}
	}


	public virtual void SetUnChoosen()
	{
		if (WaitForSubmit != null)
		{
			Debug.Log("stop courutine");
			StopCoroutine(WaitForSubmit);
		}

		Debug.Log("unchoosen");
		highlight.SetState(1);
		thubstickClickedSubject.OnNext(true);
	}

}
