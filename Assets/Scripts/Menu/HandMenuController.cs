using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class HandMenuController : MonoBehaviour
{
	[SerializeField] private List<MenuItemV> items;

	// TODO not used?
	private ISubject<bool> itemIsActiveStream = new Subject<bool>();
	public IObservable<bool> ItemIsActiveSubject { get { return itemIsActiveStream; } }

	private ButtonState currThumbstickState = ButtonState.Normal;
	private bool isMenuActive;
	private int activeItemIndex = 0;

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
		foreach (var item in items)
		{
			item.SetInactive();
			item.ThubstickClickedStream.Subscribe(_ => SetMenuActive());
		}
		items[activeItemIndex].SetActive();
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
			if (OVRInput.Get(OVRInput.Button.PrimaryThumbstick))
			{
				Debug.Log("Thumbstic clickedClicked");

				isMenuActive = false;
				items[activeItemIndex].SetChosen();
				itemIsActiveStream.OnNext(true);
			}
			else
			{
				if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickUp))
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
				else if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickDown))
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
