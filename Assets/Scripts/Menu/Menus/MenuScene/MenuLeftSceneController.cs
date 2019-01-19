using UnityEngine;
using UniRx;
using System.Collections.Generic;
using System.Collections;
using System.IO;

public class MenuLeftSceneController : MonoBehaviour
{
	[Header("Menu items")]
	[SerializeField] private MenuItemV saveExitItem;
	[SerializeField] private MenuItemV dontSaveExitItem;
	[SerializeField] private MenuItemV scenePreviewItem;

	[Header("Other")]
	[SerializeField] private Renderer sceneImageRenderer;

	protected ISubject<Unit> exitToMainModeSubject = new Subject<Unit>();
	public IObservable<Unit> ExitToMainModeStream { get { return exitToMainModeSubject; } }

	protected ISubject<Unit> saveAndExitToMainModeSubject = new Subject<Unit>();
	public IObservable<Unit> SaveAndExitToMainModeStream { get { return saveAndExitToMainModeSubject; } }

	protected ISubject<Unit> photoRequesSubject = new Subject<Unit>();
	public IObservable<Unit> PhotoRequestStream { get { return photoRequesSubject; } }

	private OVRInput.RawButton prevItemButton = OVRInput.RawButton.LThumbstickUp;
	private OVRInput.RawButton nextItemButton = OVRInput.RawButton.LThumbstickDown;
	private OVRInput.RawButton selectItemButton = OVRInput.RawButton.LThumbstick;

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
			scenePreviewItem
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
			SaveAndExitToMainModeItemSelected();
		else if (activeItemIndex == 1)
			exitToMainModeSubject.OnNext(Unit.Default);
		if (activeItemIndex == 2)
		{
			photoRequesSubject.OnNext(Unit.Default);
			StartCoroutine(WaitNextFrameAndSetMenuActive());
		}
	}

	private void SaveAndExitToMainModeItemSelected()
	{
		saveAndExitToMainModeSubject.OnNext(Unit.Default);
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
		if (File.Exists(path))
		{
			sceneImageRenderer.material.mainTexture 
				= TextureLoader.LoadTextureFromFile(path);
		}
		else
		{
			sceneImageRenderer.material.mainTexture
				= Resources.Load<Texture2D>(PathHelper.GetNoImagePath());
		}
	}

	//IEnumerator TextureUpdate(string url)
	//{
	//	Texture2D tex;
	//	tex = new Texture2D(4, 4, TextureFormat.DXT1, false);
	//	WWW www = new WWW(url);
	//	yield return www;
	//	www.LoadImageIntoTexture(tex);
	//	renderer.material.mainTexture = tex;
	//}
}
