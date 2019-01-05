using UnityEngine;
using UniRx;
using System;
using System.Collections.Generic;

public class ModelMenuItemV : MenuItemV
{
	[SerializeField] private ModelMenuV modelMenu;

	[Header("Input")]
	[SerializeField]
	private OVRInput.Button selectButton = OVRInput.Button.SecondaryThumbstick;

	private ISubject<Guid> modelToAddSelectedSubject = new Subject<Guid>();
	public IObservable<Guid> ModelToAddSelectedStream { get { return modelToAddSelectedSubject; } }

	private ButtonState thumbstick = ButtonState.Normal;
	private bool active;

	public void Start()
	{
		modelMenu.SetInactive();
		modelMenu.ItemSelectedStream.Subscribe(ItemSelected);
	}

	private void ItemSelected(Guid guid)
	{
		modelToAddSelectedSubject.OnNext(guid);
	}

	public override void SetChoosen()
	{
		//highlight.SetState(2);
		base.SetChoosen();
		active = true;
	}

	public override void SetNormal()
	{
		//highlight.SetState(1);
		base.SetNormal();
		active = false;
	}

	public void SetupMenu(List<Guid> objGuids)
	{
		modelMenu.SetupMenu(objGuids);
	}

	void Update()
	{
		if (active)
		{
			if (OVRInput.Get(selectButton))
			{
				active = false;
				modelMenu.SetActive();
			}
		}
	}
}