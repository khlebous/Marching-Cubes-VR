using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubMenuMenuItemV : MenuItemV
{
	[SerializeField] HandMenuController submenu;

	public void Start()
	{
		submenu.SetActive(false);
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
