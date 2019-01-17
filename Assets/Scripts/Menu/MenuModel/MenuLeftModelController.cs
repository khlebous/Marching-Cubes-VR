using UnityEngine;
using UniRx;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System;

public class MenuLeftModelController : MonoBehaviour
{
	[Header("Menu items")]
	[SerializeField] private MenuItemV saveExitItem;
	[SerializeField] private MenuItemV dontSaveExitItem;
	[SerializeField] private MenuItemV modelPreviewItem;

	[Header("Other")]
	[SerializeField]
	private Renderer renderer;

	protected ISubject<Unit> exitToSceneModeSubject = new Subject<Unit>();
	public IObservable<Unit> ExitToSceneModeStream { get { return exitToSceneModeSubject; } }

	protected ISubject<Unit> saveAndExitToSceneModeSubject = new Subject<Unit>();
	public IObservable<Unit> SaveAndExitToSceneModeStream { get { return saveAndExitToSceneModeSubject; } }

	protected ISubject<Unit> photoRequesSubject = new Subject<Unit>();
	public IObservable<Unit> PhotoRequestStream { get { return photoRequesSubject; } }

	private OVRInput.Button prevItemButton = OVRInput.Button.PrimaryThumbstickUp;
	private OVRInput.Button nextItemButton = OVRInput.Button.PrimaryThumbstickDown;
	private OVRInput.Button selectItemButton = OVRInput.Button.PrimaryThumbstick;

	private List<MenuItemV> items;
	private ButtonState currThumbstickState;
	private bool isMenuActive;
	private int activeItemIndex;


	private void Awake()
	{
		items = new List<MenuItemV>
		{
			saveExitItem,
			dontSaveExitItem,
			modelPreviewItem
		};
		SetupMenu();
		gameObject.SetActive(false);
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
		SetupMenu();
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

	private void ItemSelected()
	{
		if (activeItemIndex == 0)
			saveAndExitToSceneModeSubject.OnNext(Unit.Default);
		else if (activeItemIndex == 1)
			exitToSceneModeSubject.OnNext(Unit.Default);
		if (activeItemIndex == 2)
		{
			photoRequesSubject.OnNext(Unit.Default);
			StartCoroutine(WaitNextFrameAndSetMenuActive());
		}
	}

	private IEnumerator WaitNextFrameAndSetMenuActive()
	{
		yield return new WaitForSeconds(0.5f);

		isMenuActive = true;
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


	public void UpdatePhoto(string path)
	{
		FileInfo fi = new FileInfo(path);
		Texture2D tex = TextureLoader.LoadTextureFromFile(path);
		if (fi.Exists)
			renderer.material.mainTexture = TextureLoader.LoadTextureFromFile(path);
		else
		{
			path = Application.dataPath + "/Resources/0.png";
			tex = TextureLoader.LoadTextureFromFile(path);
			renderer.material.mainTexture = tex;
		}

	}
}
