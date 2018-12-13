using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using System;

public class SubMenuMenuItemV : MenuItemV
{
	[SerializeField] HandMenuController submenu;
	private ISubject<bool> thubstickClickedSubject = new Subject<bool>();
	public IObservable<bool> ThubstickClickedStream { get { return thubstickClickedSubject; } }

	public void Start()
	{
		submenu.SetActive(false);
		submenu.ThubstickClickedStream.Subscribe(thubstickClickedSubject.OnNext);
	}

	public override void SetUnChoosen()
	{
		base.SetUnChoosen();

		thubstickClickedSubject.OnNext(true);
	}


	public override void SetInactive()
	{
		base.SetInactive();
	}

	public override void SetChoosen()
	{
		base.SetChoosen();

		submenu.SetActive(true);
	}
}
