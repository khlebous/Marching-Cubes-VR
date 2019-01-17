using UnityEngine;

public class CopyTransform : MonoBehaviour
{
	[SerializeField] private Transform toFollow;
	private Transform mytransform;

	void Start ()
	{
		mytransform = GetComponent<Transform>();
	}
	
	void Update ()
	{
		mytransform.position = toFollow.position;
		mytransform.rotation = toFollow.rotation;
	}
}
