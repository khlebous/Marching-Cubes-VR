using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ModelsMenuV : MenuItemV
{
	[Header("UI")]
	[SerializeField]
	private Text scenesText;
	[SerializeField] private Renderer renderer;

	protected ISubject<Guid> itemSelectedSubject = new Subject<Guid>();
	public IObservable<Guid> ItemSelectedStream { get { return itemSelectedSubject; } }

	private OVRInput.Button selectItemButton = OVRInput.Button.SecondaryThumbstick;
	private OVRInput.Button prevItemButton = OVRInput.Button.SecondaryThumbstickLeft;
	private OVRInput.Button nextItemButton = OVRInput.Button.SecondaryThumbstickRight;

	private int activeItemIndex;
	private Guid sceneGuid;
	private List<Guid> modelGuids = new List<Guid>();

	private ButtonState currThumbstickState = ButtonState.Normal;
	private bool isMenuActive;
	private int maxItemIndex;

	//public override void SetActive()
	//{
	//	base.SetActive();
	//	StartCoroutine(WaitNextFrame());
	//}

	public Guid GetChoosenGuid()
	{
		if (modelGuids.Count == 0)
			return Guid.Empty;

		return modelGuids[activeItemIndex];
	}

	public bool AtLeastOneObjectExist()
	{
		return modelGuids.Count > 0;
	}

	private void SetupMenu(Guid sceneGuid, List<Guid> objectGuids)
	{
		activeItemIndex = 0;
		this.sceneGuid = sceneGuid;
		this.modelGuids = objectGuids;
		maxItemIndex = objectGuids.Count;

		UpdateUI();
	}

	public void ResetItem()
	{
		activeItemIndex = 0;
		UpdateUI();
	}

	IEnumerator WaitNextFrame()
	{
		yield return new WaitForSeconds(0.5f);

		isMenuActive = true;
	}

	public override void SetChoosen()
	{
		base.SetChoosen();

		StartCoroutine(WaitNextFrame());
	}

	public override void SetInactive()
	{
		base.SetInactive();
		isMenuActive = false;
	}

	private void UpdateUI()
	{
		if (maxItemIndex == 0)
		{
			scenesText.text = "";
			string path = PathHelper.GetEmptyModelsListPath();
			renderer.material.mainTexture = TextureLoader.LoadTextureFromFile(path);
		}
		else
		{
			scenesText.text = (activeItemIndex + 1) + "/" + maxItemIndex;
			string path = PathHelper.GetModelPngPath(modelGuids[activeItemIndex], sceneGuid);

			FileInfo fi = new FileInfo(path);
			if (fi.Exists)
			{
				Texture2D texture = TextureLoader.LoadTextureFromFile(path);
				renderer.material.mainTexture = texture;
			}
			else
			{
				path = PathHelper.GetNoImagePath();
				Texture2D texture = TextureLoader.LoadTextureFromFile(path);
				renderer.material.mainTexture = texture;
			}
		}
	}

	public void SetModelsGuids(Guid sceneGuid, List<Guid> modelsGuids)
	{
		SetupMenu(sceneGuid, modelsGuids);
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

	private void ItemSelected()
	{
		itemSelectedSubject.OnNext(modelGuids[activeItemIndex]);
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
