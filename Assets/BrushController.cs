using UniRx;
using UnityEngine;

public abstract class BrushController : MonoBehaviour
{
	[SerializeField] private InputController inputController;

	public float Radius { get; private set; }

	private float delta;

	void Start()
	{
		inputController.IncreaseRadiusButtonClickedStream.Subscribe(_ => IncreaseBrushRadius());
		inputController.DecreaseRadiusButtonClickedStream.Subscribe(_ => DecreaseBrushRadius());

		// TODO: get from somewhere
		Configure(0.05f, 0.01f);
	}

	public void Configure(float radius, float delta)
	{
		Radius = radius;
		this.delta = delta;
	}

	private void IncreaseBrushRadius()
	{
		Radius += delta;
		SetBrushMesh();
	}

	private void DecreaseBrushRadius()
	{
		if (Radius <= delta)
			return;

		Radius -= delta;
		SetBrushMesh();
	}

	protected abstract void SetBrushMesh();
}
