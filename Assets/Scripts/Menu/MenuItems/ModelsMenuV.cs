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
	[SerializeField] private Renderer modelRenderer;

	protected ISubject<Guid> itemSelectedSubject = new Subject<Guid>();
	public IObservable<Guid> ItemSelectedStream { get { return itemSelectedSubject; } }

	private OVRInput.Button selectItemButton = OVRInput.Button.SecondaryThumbstick;
	private OVRInput.Button prevItemButton = OVRInput.Button.SecondaryThumbstickLeft;
	private OVRInput.Button nextItemButton = OVRInput.Button.SecondaryThumbstickRight;

	private int activeItemIndex;
	private Guid sceneGuid;
	private List<Guid> modelGuids = new List<Guid>();

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

			modelRenderer.material.mainTexture = Resources.Load<Texture2D>
				(PathHelper.GetEmptyModelsListPath());
		}
		else
		{
			scenesText.text = (activeItemIndex + 1) + "/" + maxItemIndex;

			string path = PathHelper.GetModelPngPath(modelGuids[activeItemIndex], sceneGuid);
			if (File.Exists(path))
			{
				modelRenderer.material.mainTexture
					= TextureLoader.LoadTextureFromFile(path);
			}
			else
			{
				modelRenderer.material.mainTexture
					= Resources.Load<Texture2D>(PathHelper.GetNoImagePath());
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
				if (OVRInput.GetDown(prevItemButton))
				{
					DecreaseActiveItemIndex();
					UpdateUI();
				}
				else if (OVRInput.GetDown(nextItemButton))
				{
					IncreaseActiveItemIndex();
					UpdateUI();
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
