using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
	[Header("Input")]
	[SerializeField]
	private OVRInput.Button selectItemButton = OVRInput.Button.PrimaryThumbstick;
	[SerializeField] private OVRInput.Button nextItemButton = OVRInput.Button.PrimaryThumbstickRight;
	[SerializeField] private OVRInput.Button prevItemButton = OVRInput.Button.PrimaryThumbstickLeft;

	[Header("Other")]
	[SerializeField]
	private Text scenesText;

	private ButtonState currThumbstickState = ButtonState.Normal;
	private bool isMenuActive;
	private int activeItemIndex = 0;
	private int maxItemIndex = 5;

	private void Start()
	{
		SetInactive();
	}

	public void SetInactive()
	{
		gameObject.SetActive(false);
		isMenuActive = false;
	}

	public void SetActive()
	{
		gameObject.SetActive(true);
		StartCoroutine(WaitNextFrame());
	}

	IEnumerator WaitNextFrame()
	{
		yield return new WaitForSeconds(0.5f);

		isMenuActive = true;
	}

	void Update()
	{
		if (isMenuActive)
		{
			if (OVRInput.Get(selectItemButton))
			{
				isMenuActive = false;
				//items[activeItemIndex].SetChoosen();
				ItemSelected();
			}
			else
			{
				if (OVRInput.Get(prevItemButton))
				{
					if (currThumbstickState == ButtonState.Normal)
					{
						currThumbstickState = ButtonState.Up;
						DecreaseActiveItemIndex();
						UpdateUI();
					}
				}
				else if (OVRInput.Get(nextItemButton))
				{
					if (currThumbstickState == ButtonState.Normal)
					{
						currThumbstickState = ButtonState.Down;
						IncreaseActiveItemIndex();
						UpdateUI();
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

	private void UpdateUI()
	{
		if (activeItemIndex == 0)
			scenesText.text = activeItemIndex + "/" + maxItemIndex + "\n(New scene)";
		else
			scenesText.text = activeItemIndex + "/" + maxItemIndex + "\n";
	}

	private void ItemSelected()
	{
		Debug.Log("TODO: load scene, nr: " + activeItemIndex);
		scenesText.text = "loading ...";

	}

	private void IncreaseActiveItemIndex()
	{
		activeItemIndex++;
		activeItemIndex %= maxItemIndex;
	}

	private void DecreaseActiveItemIndex()
	{
		if (activeItemIndex == 0)
			activeItemIndex = maxItemIndex;
		activeItemIndex--;
		activeItemIndex %= maxItemIndex;
	}
}
