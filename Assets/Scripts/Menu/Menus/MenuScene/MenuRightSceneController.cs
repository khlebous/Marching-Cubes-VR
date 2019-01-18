using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;

public class MenuRightSceneController : MonoBehaviour
{
	[Header("Menu items")]
	[SerializeField] private MenuItemV terrainMode;
	[SerializeField] private MenuItemV newModel;
	[SerializeField] private ModelsMenuV modelsList;
	[SerializeField] private MenuItemV addModelFromList;
	[SerializeField] private MenuItemV editModel;
	[SerializeField] private MenuItemV deleteModel;

	[Header("Other")]
	[SerializeField] private McManager mcManager;

	protected ISubject<Unit> exitToTerrainModeSubject = new Subject<Unit>();
	public IObservable<Unit> ExitToTerrainModeStream { get { return exitToTerrainModeSubject; } }

	protected ISubject<Guid> exitToObjectModeSubject = new Subject<Guid>();
	public IObservable<Guid> ExitToObjectModeStream { get { return exitToObjectModeSubject; } }

	private ISubject<Guid> modelToAddSelectedSubject = new Subject<Guid>();
	public IObservable<Guid> ModelToAddSelectedStream { get { return modelToAddSelectedSubject; } }

	private ISubject<Guid> modelToEditSelectedSubject = new Subject<Guid>();
	public IObservable<Guid> ModelToEditSelectedStream { get { return modelToEditSelectedSubject; } }

	private ISubject<Guid> modelToDeleteSelectedSubject = new Subject<Guid>();
	public IObservable<Guid> ModelToDeleteSelectedStream { get { return modelToDeleteSelectedSubject; } }

	protected ISubject<Unit> itemSelectedSubject = new Subject<Unit>();
	public IObservable<Unit> ItemSelectedStream { get { return itemSelectedSubject; } }

	private OVRInput.RawButton prevItemButton = OVRInput.RawButton.RThumbstickUp;
	private OVRInput.RawButton nextItemButton = OVRInput.RawButton.RThumbstickDown;
	private OVRInput.RawButton selectItemButton = OVRInput.RawButton.RThumbstick;

	private List<MenuItemV> items;
	private ButtonState currThumbstickState;
	private bool isMenuActive;
	private int activeItemIndex;


	private void Awake()
	{
		items = new List<MenuItemV>
		{
			terrainMode,
			newModel,
			modelsList,
			addModelFromList,
			editModel,
			deleteModel
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
		// modelsList.ResetItem();
		SetupMenu();
	}

	private void Start()
	{
		modelsList.ItemSelectedStream.Subscribe
			(_ => StartCoroutine(WaitNextFrameAndSetMenuActive()));
	}

	IEnumerator WaitNextFrameAndSetMenuActive()
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
		switch (activeItemIndex)
		{
			case 0: // Edycja terenu
				exitToTerrainModeSubject.OnNext(Unit.Default);
				break;
			case 1: // Nowy obiekt
				exitToObjectModeSubject.OnNext(Guid.Empty);
				break;
			case 2: // modelsList
				itemSelectedSubject.OnNext(Unit.Default);
				modelsList.SetChoosen();
				break;
			case 3: // Add model to scene
				if (modelsList.AtLeastOneObjectExist())
					modelToAddSelectedSubject.OnNext(modelsList.GetChoosenGuid());
				break;
			case 4: // Edit model
				if (modelsList.AtLeastOneObjectExist())
					modelToEditSelectedSubject.OnNext(modelsList.GetChoosenGuid());
				break;
			case 5: // Delete Model
				if (modelsList.AtLeastOneObjectExist())
					modelToDeleteSelectedSubject.OnNext(modelsList.GetChoosenGuid());
				StartCoroutine(WaitNextFrameAndSetMenuActive());
				break;
			default:
				break;
		}
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
	
	public void UpdateModelsGuids(Guid sceneGuid, List<Guid> modelsGuids)
	{
		modelsList.SetModelsGuids(sceneGuid, modelsGuids);
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

}
