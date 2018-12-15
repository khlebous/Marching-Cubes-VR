//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//using UniRx;
//using System;

//public class SubMenuMenuItemV : MenuItemV
//{
//	[SerializeField] HandMenuController submenu;

//	protected ISubject<Unit> submenuActiveSubject = new Subject<Unit>();
//	public IObservable<Unit> SubmenuActiveStream { get { return submenuActiveSubject; } }


//	public void Start()
//	{
//		submenu.SetActive(false);
//	}

//	public override void SetUnChoosen()
//	{
//		base.SetUnChoosen();
//		thubstickClickedSubject.OnNext(true);
//	}


//	public override void SetInactive()
//	{
//		base.SetInactive();
//	}

//	public override void SetChoosen()
//	{
//		Debug.Log("set choosen2");
//		base.SetChoosen();
//		submenuActiveSubject.OnNext(Unit.Default);

//		submenu.SetActive(true);
//	}
//}
