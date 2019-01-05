using UnityEngine;
using UniRx;
using System.Collections;
using System.Collections.Generic;

public class MenuRightModelController : MonoBehaviour
{
	// TODO Should be higher?
	[SerializeField] private Assets.MarchingCubesGPU.Scripts.ModelBrush brush;

	[Header("Menu items")]
	[SerializeField] MenuItemHMenuV modeItem;
	[SerializeField] MenuItemHMenuV brushShapeItem;
	[SerializeField] MenuItemSliderV brushSizeItem;
	[SerializeField] MenuItemColorV brushColorItem;

	private ISubject<Unit> itemIsActiveSubject = new Subject<Unit>();
	public IObservable<Unit> ItemIsActiveStream { get { return itemIsActiveSubject; } }

	private ISubject<Unit> itemNotActiveSubject = new Subject<Unit>();
	public IObservable<Unit> ItemNotActiveStream { get { return itemNotActiveSubject; } }

	private OVRInput.Button selectItemButton = OVRInput.Button.SecondaryThumbstick;
	private OVRInput.Button nextItemButton = OVRInput.Button.SecondaryThumbstickDown;
	private OVRInput.Button prevItemButton = OVRInput.Button.SecondaryThumbstickUp;

	private ButtonState currThumbstickState = ButtonState.Normal;
	private bool isMenuActive;
	private int activeItemIndex = 0;

	private List<MenuItemV> items;

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

	private void Start()
	{
		items = new List<MenuItemV>
		{
			modeItem,
			brushShapeItem,
			brushSizeItem,
			brushColorItem
		};

		foreach (var item in items)
		{
			item.SetInactive();
			item.ThubstickClickedStream.Subscribe(_ => SetMenuActive());
		}
		items[activeItemIndex].SetActive();

		modeItem.ChoosenItemSubject.Subscribe(brush.SetMode);
		brushShapeItem.ChoosenItemSubject.Subscribe(brush.SetShape);
	    brushSizeItem.ValueChangedStream.Subscribe(brush.SetSizeChanged);
		brushColorItem.ColorChangedStream.Subscribe(brush.SetColor);
	}

	public void SetMenuActive()
	{
		StartCoroutine(WaitNextFrame());
	}

	IEnumerator WaitNextFrame()
	{
		yield return new WaitForSeconds(0.5f);

		isMenuActive = true;
		itemNotActiveSubject.OnNext(Unit.Default);
	}

	void Update()
	{
		if (isMenuActive)
		{
			if (OVRInput.Get(selectItemButton))
			{
				isMenuActive = false;
				items[activeItemIndex].SetChoosen();
				itemIsActiveSubject.OnNext(Unit.Default);
			}
			else
			{
				if (OVRInput.Get(prevItemButton))
				{
					if (currThumbstickState == ButtonState.Normal)
					{
						currThumbstickState = ButtonState.Up;
						items[activeItemIndex].SetInactive();
						DecreaseActiveItemIndex();
						items[activeItemIndex].SetActive();
					}
				}
				else if (OVRInput.Get(nextItemButton))
				{
					if (currThumbstickState == ButtonState.Normal)
					{
						currThumbstickState = ButtonState.Down;
						items[activeItemIndex].SetInactive();
						IncreaseActiveItemIndex();
						items[activeItemIndex].SetActive();
					}
				}
				else
				{
					if (currThumbstickState != ButtonState.Normal)
					{
						currThumbstickState = ButtonState.Normal;
					}
				}
			}
		}
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
