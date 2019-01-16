using UnityEngine;
using UniRx;
using System.Collections;
using System.Collections.Generic;

public class MenuRightTerrainController : MonoBehaviour
{
	// TODO Should be higher?
	[SerializeField] private Assets.MarchingCubesGPU.Scripts.TerrainBrush brush;

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

	private List<MenuItemV> items;
	private ButtonState currThumbstickState;
	private bool isMenuActive;
	private int activeItemIndex;


	private void Awake()
	{
		items = new List<MenuItemV>
		{
			modeItem,
			brushShapeItem,
			brushSizeItem,
			brushColorItem
		};
		SetupMenu();
		gameObject.SetActive(false);

		brushSizeItem.ValueChangedStream.Subscribe(brush.SetSizeChanged);
	}

	private void SetupMenu()
	{
		currThumbstickState = ButtonState.Normal;
		isMenuActive = false;
		activeItemIndex = 0;

		foreach (var item in items)
			item.SetInactive();
		items[activeItemIndex].SetActive();
	}

	public void ResetMenu()
	{
		modeItem.ResetItem();
		brushShapeItem.ResetItem();
		brushSizeItem.ResetItem();
		brushColorItem.ResetItem();

		SetupMenu();
	}


	private void Start()
	{
		modeItem.ChoosenItemSubject.Subscribe(brush.SetMode);
		brushShapeItem.ChoosenItemSubject.Subscribe(brush.SetShape);
		brushColorItem.ColorChangedStream.Subscribe(brush.SetColor);

		foreach (var item in items)
			item.ThubstickClickedStream.Subscribe
				(_ => StartCoroutine(WaitNextFrameAndSetMenuActive()));
	}

	IEnumerator WaitNextFrameAndSetMenuActive()
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
