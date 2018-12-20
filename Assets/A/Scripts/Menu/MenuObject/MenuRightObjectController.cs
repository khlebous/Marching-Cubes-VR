using System.Collections;
using UnityEngine;
using UniRx;
using System.Collections.Generic;

public class MenuRightObjectController : MonoBehaviour
{
	[SerializeField] private Assets.MarchingCubesGPU.Scripts.ModelBrush brush;

	[Header("Input")]
	[SerializeField] private OVRInput.Button selectItemButton = OVRInput.Button.SecondaryThumbstick;
	[SerializeField] private OVRInput.Button nextItemButton = OVRInput.Button.SecondaryThumbstickDown;
	[SerializeField] private OVRInput.Button prevItemButton = OVRInput.Button.SecondaryThumbstickUp;

	[Header("Menu items")]
	[SerializeField] MenuItemHMenuV modeItem;
	[SerializeField] MenuItemHMenuV brushShapeItem;
	[SerializeField] MenuItemSliderV brushSizeItem;
	[SerializeField] MenuItemColorV brushColorItem;

	private ISubject<bool> itemIsActiveStream = new Subject<bool>();
	public IObservable<bool> ItemIsActiveSubject { get { return itemIsActiveStream; } }

	private ButtonState currThumbstickState = ButtonState.Normal;
	private bool isMenuActive;
	private int activeItemIndex = 0;

	private List<MenuItemV> items;

	public void OpenMenu()
	{
		Debug.Log("open menu");
		gameObject.SetActive(true);
		isMenuActive = true;
	}

	public void CloseMenu()
	{
		Debug.Log("close menu");
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
		itemIsActiveStream.OnNext(false);
	}

	void Update()
	{
		if (isMenuActive)
		{
			if (OVRInput.Get(selectItemButton))
			{
				isMenuActive = false;
				items[activeItemIndex].SetChoosen();
				itemIsActiveStream.OnNext(true);
			}
			else
			{
				if (OVRInput.Get(prevItemButton))
				{
					if (currThumbstickState == ButtonState.Normal)
					{
						currThumbstickState = ButtonState.Up;
						Debug.Log("Up");
						items[activeItemIndex].SetInactive();
						DecreaseActiveItemIndex();
						Debug.Log("new curr index: " + activeItemIndex);
						items[activeItemIndex].SetActive();
					}
				}
				else if (OVRInput.Get(nextItemButton))
				{
					if (currThumbstickState == ButtonState.Normal)
					{
						currThumbstickState = ButtonState.Down;
						Debug.Log("Down");
						items[activeItemIndex].SetInactive();
						IncreaseActiveItemIndex();
						Debug.Log("new curr index: " + activeItemIndex);
						items[activeItemIndex].SetActive();
					}
				}
				else
				{
					if (currThumbstickState != ButtonState.Normal)
					{
						currThumbstickState = ButtonState.Normal;
						Debug.Log("Normal");
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
		activeItemIndex %= items.Count;
	}
}
