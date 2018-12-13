using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuItemV : MonoBehaviour
{
	[SerializeField] MultiStateMaterial highlight;

	public void OnClick()
	{
		// highlight submenu
	}

	public virtual void SetInactive()
	{
		if (highlight != null)
			highlight.SetState(0);
	}

	public virtual void SetActive()
	{
		highlight.SetState(1);
	}

	public virtual void SetChoosen()
	{
		highlight.SetState(2);
	}

	public virtual void SetUnChoosen()
	{
		highlight.SetState(1);
	}

}
