using System.Collections;
using UniRx;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
	[Header("Input")]
	[SerializeField] private OVRInput.Button selectItemButton = OVRInput.Button.PrimaryThumbstick;
	[SerializeField] private OVRInput.Button nextItemButton = OVRInput.Button.PrimaryThumbstickRight;
	[SerializeField] private OVRInput.Button prevItemButton = OVRInput.Button.PrimaryThumbstickLeft;

	//[Header("Other")]
	//[SerializeField] private MenuSceneController menuSceneController;

	public int ActiveItemIndex { get; private set; } 
	public int MaxItemIndex { get; private set; }

	protected ISubject<Unit> itemChangedSubject = new Subject<Unit>();
	public IObservable<Unit> ItemChangedStream { get { return itemChangedSubject; } }

	protected ISubject<Unit> menuEnabledSubject = new Subject<Unit>();
	public IObservable<Unit> MenuEnabledStream { get { return menuEnabledSubject; } }

	protected ISubject<Unit> itemSelectedSubject = new Subject<Unit>();
	public IObservable<Unit> ItemSelectedStream { get { return itemSelectedSubject; } }

	private ButtonState currThumbstickState = ButtonState.Normal;
	private bool isMenuActive;

	private void Start()
	{
		SetInactive();
	}

	public void SetActive()
	{
		SetupMenu();
		menuEnabledSubject.OnNext(Unit.Default);
		StartCoroutine(WaitNextFrame());
	}

	private void SetupMenu()
	{
		// TODO read scenes, set maxItemIndex
		ActiveItemIndex = 0;
		MaxItemIndex = Random.Range(3, 8);
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
						DecreaseActiveItemIndex();
						itemChangedSubject.OnNext(Unit.Default);
					}
				}
				else if (OVRInput.Get(nextItemButton))
				{
					if (currThumbstickState == ButtonState.Normal)
					{
						currThumbstickState = ButtonState.Down;
						IncreaseActiveItemIndex();
						itemChangedSubject.OnNext(Unit.Default);
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
		Debug.Log("TODO: load scene, nr: " + ActiveItemIndex);
		Debug.Log("if 0 create new one");
		itemSelectedSubject.OnNext(Unit.Default);
	}

	private void IncreaseActiveItemIndex()
	{
		ActiveItemIndex++;
		ActiveItemIndex %= MaxItemIndex;
	}

	private void DecreaseActiveItemIndex()
	{
		if (ActiveItemIndex == 0)
			ActiveItemIndex = MaxItemIndex;
		ActiveItemIndex--;
		ActiveItemIndex %= MaxItemIndex;
	}
}
