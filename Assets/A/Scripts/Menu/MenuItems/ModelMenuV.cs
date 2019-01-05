using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ModelMenuV : MonoBehaviour
{
	[Header("UI")]
	[SerializeField] private Text scenesText;

	protected ISubject<Guid> itemSelectedSubject = new Subject<Guid>();
	public IObservable<Guid> ItemSelectedStream { get { return itemSelectedSubject; } }

	private OVRInput.Button selectItemButton = OVRInput.Button.SecondaryThumbstick;
	private OVRInput.Button prevItemButton = OVRInput.Button.SecondaryThumbstickLeft;
	private OVRInput.Button nextItemButton = OVRInput.Button.SecondaryThumbstickRight;

	private int activeItemIndex;
	private List<Guid> objectGuids;

	private ButtonState currThumbstickState = ButtonState.Normal;
	private bool isMenuActive;
	private int maxItemIndex;

	public void SetActive()
	{
		StartCoroutine(WaitNextFrame());
	}

	public void SetupMenu(List<Guid> objectGuids)
	{
		activeItemIndex = 0;
		this.objectGuids = objectGuids;
		maxItemIndex = objectGuids.Count;

		UpdateUI();
	}

	IEnumerator WaitNextFrame()
	{
		yield return new WaitForSeconds(0.5f);

		isMenuActive = true;
	}

	public void SetInactive()
	{
		isMenuActive = false;
	}

	private void UpdateUI()
	{
		scenesText.text = (activeItemIndex + 1) + "/" + maxItemIndex 
			+ "\n " + objectGuids[activeItemIndex];
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
				if (OVRInput.Get(prevItemButton))
				{
					if (currThumbstickState == ButtonState.Normal)
					{
						currThumbstickState = ButtonState.Up;
						UpdateUI();
						DecreaseActiveItemIndex();
					}
				}
				else if (OVRInput.Get(nextItemButton))
				{
					if (currThumbstickState == ButtonState.Normal)
					{
						currThumbstickState = ButtonState.Down;
						UpdateUI();
						IncreaseActiveItemIndex();
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

	private void ItemSelected()
	{
		itemSelectedSubject.OnNext(objectGuids[activeItemIndex]);
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
	}
}
