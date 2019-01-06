using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ModelsMenuV : MonoBehaviour
{
	[Header("UI")]
	[SerializeField] private Text scenesText;

	protected ISubject<Guid> itemSelectedSubject = new Subject<Guid>();
	public IObservable<Guid> ItemSelectedStream { get { return itemSelectedSubject; } }

	private OVRInput.Button selectItemButton = OVRInput.Button.SecondaryThumbstick;
	private OVRInput.Button prevItemButton = OVRInput.Button.SecondaryThumbstickLeft;
	private OVRInput.Button nextItemButton = OVRInput.Button.SecondaryThumbstickRight;

	private int activeItemIndex;
	private List<Guid> objectGuids = new List<Guid>();

	private ButtonState currThumbstickState = ButtonState.Normal;
	private bool isMenuActive;
	private int maxItemIndex;

	public void SetActive()
	{
		StartCoroutine(WaitNextFrame());
	}

	public Guid GetChoosenGuid()
	{
		if (objectGuids.Count == 0)
			return Guid.Empty;

		return objectGuids[activeItemIndex];
	}

	public bool AtLeastOneObjectExist()
	{
		return objectGuids.Count > 0;
	}

	private void SetupMenu(List<Guid> objectGuids)
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
		if (maxItemIndex == 0)
			scenesText.text = "create object to add to scene";
		else
			scenesText.text = (activeItemIndex + 1) + "/" + maxItemIndex
				+ "\n " + objectGuids[activeItemIndex];
	}

	public void SetModelsGuids(List<Guid> modelsGuids)
	{
		SetupMenu(modelsGuids);
	}

	void Update()
	{
		if (isMenuActive)
		{
			if (OVRInput.Get(selectItemButton))
			{
				isMenuActive = false;
				if (maxItemIndex != 0)
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
		if (maxItemIndex != 0)
		{
			activeItemIndex++;
			activeItemIndex %= maxItemIndex;
		}
	}

	private void DecreaseActiveItemIndex()
	{
		if (maxItemIndex != 0)
		{
			if (activeItemIndex == 0)
				activeItemIndex = maxItemIndex;
			activeItemIndex--;
		}
	}
}
