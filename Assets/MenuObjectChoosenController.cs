using UnityEngine;
using System.Collections.Generic;
using UniRx;

public class MenuObjectChoosenController : MonoBehaviour {

	[Header("Menu items")]
	[SerializeField] MenuItemV stopEditing;
	[SerializeField] MenuItemV editModel;
	[SerializeField] MenuItemV deleteObject;

	protected ISubject<Unit> stopEditingSubject = new Subject<Unit>();
	public IObservable<Unit> StopEditingStream { get { return stopEditingSubject; } }

	protected ISubject<Unit> editModelSubject = new Subject<Unit>();
	public IObservable<Unit> EditModelStream { get { return editModelSubject; } }

	protected ISubject<Unit> deleteObjectSubject = new Subject<Unit>();
	public IObservable<Unit> DeleteObjectStream { get { return deleteObjectSubject; } }

	private OVRInput.RawButton selectItemButton = OVRInput.RawButton.RThumbstick;
	private OVRInput.RawButton nextItemButton = OVRInput.RawButton.RThumbstickDown;
	private OVRInput.RawButton prevItemButton = OVRInput.RawButton.RThumbstickUp;

	private List<MenuItemV> items;
	private bool isMenuActive;
	private int activeItemIndex;

	private void Awake()
	{
		items = new List<MenuItemV>
		{
			stopEditing,
			editModel,
			deleteObject,
		};
		SetupMenu();
		gameObject.SetActive(false);
	}

	private void SetupMenu()
	{
		isMenuActive = false;
		activeItemIndex = 0;

		foreach (var item in items)
			item.SetInactive();
		items[activeItemIndex].SetActive();
	}

	public void ResetMenu()
	{
		SetupMenu();
	}

	void Update()
	{
		if (isMenuActive)
		{
			if (OVRInput.Get(selectItemButton))
			{
				isMenuActive = false;
				ItemSelected();
			}
			else
			{
				if (OVRInput.GetDown(prevItemButton))
				{
					items[activeItemIndex].SetInactive();
					DecreaseActiveItemIndex();
					items[activeItemIndex].SetActive();
				}
				else if (OVRInput.GetDown(nextItemButton))
				{
					items[activeItemIndex].SetInactive();
					IncreaseActiveItemIndex();
					items[activeItemIndex].SetActive();
				}
			}
		}
	}

	private void ItemSelected()
	{
		if (activeItemIndex == 0)
			stopEditingSubject.OnNext(Unit.Default);
		else if (activeItemIndex == 1)
			editModelSubject.OnNext(Unit.Default);
		else if (activeItemIndex == 2)
			deleteObjectSubject.OnNext(Unit.Default);
	}


	public void OpenMenu()
	{
		gameObject.SetActive(true);
		isMenuActive = true;
	}

	public void CloseMenu()
	{
		gameObject.SetActive(false);
		isMenuActive = false;
	}


	private void IncreaseActiveItemIndex()
	{
		activeItemIndex++;
		activeItemIndex %= items.Count;
	}

	private void DecreaseActiveItemIndex()
	{
		if (activeItemIndex == 0)
			activeItemIndex = items.Count;
		activeItemIndex--;
	}
}
