using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using System;

public enum buttonState
{
	Left, Up, Right, Down, Normal
}

public class HandMenuController : MonoBehaviour
{
	[SerializeField] private List<MenuItemV> items;

	// TODO will not be serialized (only to debug)
	[SerializeField] private bool active;

	private int activeItemIndex = 0;
	private bool activeItemChoosen;

	private buttonState thumbstick = buttonState.Normal;

	public void OpenMenu()
	{
		Debug.Log("open menu");
		gameObject.SetActive(true);
		active = true;
	}

	public void CloseMenu()
	{
		Debug.Log("close menu");
		gameObject.SetActive(false);
		active = false;
	}

	private void Start()
	{
		foreach (var item in items)
		{
			item.SetInactive();
			item.ThubstickClickedStream.Subscribe(SetActive);
		}
		items[activeItemIndex].SetActive();
	}

	public void SetActive(bool active)
	{
		StartCoroutine(WaitNextFrame(active));
	}

	IEnumerator WaitNextFrame(bool active)
	{
		yield return new WaitForSeconds(0.5f);
		activeItemChoosen = false;
		this.active = true;

		//items[activeItemIndex].SetUnChoosen();
		//thubstickClickedSubject.OnNext(true);
	}

	private ISubject<bool> thubstickClickedSubject = new Subject<bool>();
	public IObservable<bool> ThubstickClickedStream { get { return thubstickClickedSubject; } }


	void Update()
	{
		if (active)
		{
			if (OVRInput.Get(OVRInput.Button.PrimaryThumbstick))
			{
				Debug.Log("Thumbstic clickedClicked");

				if (!activeItemChoosen) // chyba nie rzeba tego if
				{
					active = false;
					activeItemChoosen = true;
					items[activeItemIndex].SetChoosen();
				}
			}
			else if (!activeItemChoosen)
			{

				if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft))
				{
					if (thumbstick == buttonState.Normal)
					{
						thumbstick = buttonState.Left;
						Debug.Log("Left");
					}

				}
				else if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight))
				{
					if (thumbstick == buttonState.Normal)
					{
						thumbstick = buttonState.Right;
						Debug.Log("Right");
					}
				}
				else if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickUp))
				{
					if (thumbstick == buttonState.Normal)
					{
						thumbstick = buttonState.Up;
						Debug.Log("Up");
						items[activeItemIndex].SetInactive();
						if (activeItemIndex == 0)
							activeItemIndex = items.Count;
						activeItemIndex--;
						activeItemIndex %= items.Count;
						Debug.Log("new curr index: " + activeItemIndex);
						items[activeItemIndex].SetActive();
					}
				}
				else if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickDown))
				{
					if (thumbstick == buttonState.Normal)
					{
						thumbstick = buttonState.Down;
						Debug.Log("Down");
						items[activeItemIndex].SetInactive();
						activeItemIndex++;
						activeItemIndex %= items.Count;
						Debug.Log("new curr index: " + activeItemIndex);
						items[activeItemIndex].SetActive();
					}
				}
				else
				{
					if (thumbstick != buttonState.Normal)
					{
						thumbstick = buttonState.Normal;
						Debug.Log("Normal");
					}
				}
			}
		}

		Vector2 tmp = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
		//Debug.Log(tmp);
		if (Mathf.Abs(tmp.x) > Mathf.Abs(tmp.y))
		{
			if (tmp.x > 0.9999)
				Debug.Log("right");
			else if (tmp.x < -0.5)
				Debug.Log("left");
		}
		else
		{
			if (tmp.y > 0.5)
			{
				Debug.Log("up");

			}
			else if (tmp.y < -0.5)
			{
				Debug.Log("down");
				//items[activeItemIndex].SetActive(false);
				//activeItemIndex++;
				//activeItemIndex %= items.Count;
				//Debug.Log("new curr index");
				//items[activeItemIndex].SetActive(true);
			}
		}
	}


}
