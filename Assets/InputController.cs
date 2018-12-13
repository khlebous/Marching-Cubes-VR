using UniRx;
using UnityEngine;

public class InputController : MonoBehaviour
{
	//TODO: moze podpiac skryppt pod player'a?
	[SerializeField] private GameObject player;

	private Subject<Vector3> addButtonClickedSubject = new Subject<Vector3>();
	public IObservable<Vector3> AddButtonClickedStream { get { return addButtonClickedSubject.AsObservable(); } }

	private Subject<Vector3> removeButtonClickedSubject = new Subject<Vector3>();
	public IObservable<Vector3> RemoveButtonClickedStream { get { return removeButtonClickedSubject.AsObservable(); } }

	private Subject<Unit> increaseRadiusButtonClickedSubject = new Subject<Unit>();
	public IObservable<Unit> IncreaseRadiusButtonClickedStream { get { return increaseRadiusButtonClickedSubject.AsObservable(); } }

	private Subject<Unit> decreaseRadiusButtonClickedSubject = new Subject<Unit>();
	public IObservable<Unit> DecreaseRadiusButtonClickedStream { get { return decreaseRadiusButtonClickedSubject.AsObservable(); } }

	//void Update()
	//{
	//	if (OVRInput.Get(OVRInput.Button.Two)) // Right hand B
	//	{
	//		Vector3 position = player.transform.Find("TrackingSpace").
	//			transform.TransformPoint(OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch));
	//		removeButtonClickedSubject.OnNext(position);
	//	}
	//	else if (OVRInput.Get(OVRInput.Button.One)) // Right hand A
	//	{
	//		Vector3 position = player.transform.Find("TrackingSpace").
	//			transform.TransformPoint(OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch));
	//		addButtonClickedSubject.OnNext(position);
	//	}

	//	if (OVRInput.Get(OVRInput.Button.Four)) // Left hand Y
	//	{
	//		Vector3 position = player.transform.Find("TrackingSpace").
	//			transform.TransformPoint(OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch));
	//		removeButtonClickedSubject.OnNext(position);
	//	}
	//	else if (OVRInput.Get(OVRInput.Button.Three)) // Left hand X
	//	{
	//		Vector3 position = player.transform.Find("TrackingSpace").
	//			transform.TransformPoint(OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch));
	//		addButtonClickedSubject.OnNext(position);
	//	}

	//	if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
	//	{
	//		increaseRadiusButtonClickedSubject.OnNext(Unit.Default);
	//	}
	//	else if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger))
	//	{
	//		decreaseRadiusButtonClickedSubject.OnNext(Unit.Default);
	//	}
	//}
}
