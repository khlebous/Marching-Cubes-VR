using UnityEngine;
using UniRx;

public class TerrainModeController : MonoBehaviour
{
	[SerializeField] private GameObject terrainContiner;
	[SerializeField] private MenuTerrainController menuTerrainController;

	protected ISubject<Unit> modeExitedSubject = new Subject<Unit>();
	public IObservable<Unit> ModeExitedStream { get { return modeExitedSubject; } }

	private void Start()
	{
		menuTerrainController.ExitToSceneModeStream.Subscribe(_ => ExitMode());
		menuTerrainController.SaveAndExitToSceneModeStream.Subscribe(_ => SaveTerrainAndExitMode());
	}

	public void TurnOnMode()
	{
		Debug.Log("TerrainModeController  turn on");
		Debug.Log("TODO smth: ");
		terrainContiner.SetActive(true);
		menuTerrainController.SetActive();
	}

	private void ExitMode()
	{
		Debug.Log("SceneModeController  turn off");
		terrainContiner.SetActive(false);
		menuTerrainController.SetInactive();

		modeExitedSubject.OnNext(Unit.Default);
	}

	private void SaveTerrainAndExitMode()
	{
		Debug.Log("TODO save scene");
		ExitMode();
	}
}