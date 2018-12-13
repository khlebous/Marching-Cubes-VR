﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum buttonState
{
	Left, Up, Right, Down, Normal
}

public class HandMenuController : MonoBehaviour
{
	[SerializeField] private List<MenuItemV> items;

	private int activeItemIndex = 0;
	private buttonState thumbstick = buttonState.Normal;
	private bool subMenuChoosen;
	[SerializeField] private bool active;

	private void Start()
	{
		foreach (var item in items)
			item.SetInactive();
		//items[activeItemIndex].SetActive();
	}

	public void SetActive(bool active)
	{
		StartCoroutine(WaitNextFrame(active));
	}

	IEnumerator WaitNextFrame(bool active)
	{
		yield return new WaitForSeconds(0.5f);
		this.active = active;
	}

	void Update()
	{
		if (active)
		{

			if (OVRInput.Get(OVRInput.Button.PrimaryThumbstick))
			{
				if (subMenuChoosen)
				{

					items[activeItemIndex].SetUnChoosen();
				}
				else
				{
					active = false;
					items[activeItemIndex].SetChoosen();
				}

				subMenuChoosen = !subMenuChoosen;
				Debug.Log("Clicked");
			}
			else if (!subMenuChoosen)
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

		//Vector2 tmp = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
		////Debug.Log(tmp);
		//if (Mathf.Abs(tmp.x) > Mathf.Abs(tmp.y))
		//{
		//	if (tmp.x > 0.9999)
		//		Debug.Log("right");
		//	else if (tmp.x < -0.5)
		//		Debug.Log("left");
		//}
		//else
		//{
		//	if (tmp.y > 0.5)
		//	{
		//		Debug.Log("up");

		//	}
		//	else if (tmp.y < -0.5)
		//	{
		//		Debug.Log("down");
		//		//items[activeItemIndex].SetActive(false);
		//		//activeItemIndex++;
		//		//activeItemIndex %= items.Count;
		//		//Debug.Log("new curr index");
		//		//items[activeItemIndex].SetActive(true);
		//	}
		//}
	}
}
