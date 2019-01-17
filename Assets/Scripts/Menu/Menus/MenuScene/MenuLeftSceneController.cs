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
	[SerializeField] private Renderer renderer;

	protected ISubject<Unit> exitToMainModeSubject = new Subject<Unit>();
	public IObservable<Unit> ExitToMainModeStream { get { return exitToMainModeSubject; } }

	protected ISubject<Unit> saveAndExitToMainModeSubject = new Subject<Unit>();
	public IObservable<Unit> SaveAndExitToMainModeStream { get { return saveAndExitToMainModeSubject; } }

	protected ISubject<Unit> photoRequesSubject = new Subject<Unit>();
	public IObservable<Unit> PhotoRequestStream { get { return photoRequesSubject; } }

	private OVRInput.Button prevItemButton = OVRInput.Button.PrimaryThumbstickUp;
	private OVRInput.Button nextItemButton = OVRInput.Button.PrimaryThumbstickDown;
	private OVRInput.Button selectItemButton = OVRInput.Button.PrimaryThumbstick;
	private OVRInput.Controller controller = OVRInput.Controller.LTouch;

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
				if (OVRInput.Get(prevItemButton, controller))
				{
					if (currThumbstickState == ButtonState.Normal)
					{
						currThumbstickState = ButtonState.Up;
						items[activeItemIndex].SetInactive();
						DecreaseActiveItemIndex();
						items[activeItemIndex].SetActive();
					}
				}
				else if (OVRInput.Get(nextItemButton, controller))
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
			saveAndExitToMainModeSubject.OnNext(Unit.Default);
		else if (activeItemIndex == 1)
			exitToMainModeSubject.OnNext(Unit.Default);
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


	public void UpdatePhoto(string path = "")
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
