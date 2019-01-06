using UnityEngine;
using UniRx;
using System;
using System.Collections.Generic;

public class ModelMenuItemV : MenuItemV
{
	[SerializeField] private ModelMenuV modelMenu;

	[Header("Input")]
	[SerializeField] private OVRInput.Button selectButton = OVRInput.Button.SecondaryThumbstick;

	private ISubject<Guid> modelSelectedSubject = new Subject<Guid>();
	public IObservable<Guid> ModelSelectedStream { get { return modelSelectedSubject; } }

	private ButtonState thumbstick = ButtonState.Normal;
	private bool active;

	public void Start()
	{
		modelMenu.SetInactive();
		modelMenu.ItemSelectedStream.Subscribe(modelSelectedSubject.OnNext);
	}

	public override void SetChoosen()
	{
		base.SetChoosen();
		active = true;
	}

	public override void SetNormal()
	{
		base.SetNormal();
		active = false;
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

	public void SetModelsGuids(List<Guid> modelsGuids)
	{
		modelMenu.SetModelsGuids(modelsGuids);
	}
}