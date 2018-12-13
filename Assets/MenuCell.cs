using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuCell : MonoBehaviour
{
	private Text text;

	void Start ()
	{
		text = GetComponent<Text>();
	}

	public void SetActive()
	{
		text.color = Color.red;
	}

	public void SetInactive()
	{
		text.color = Color.black;
	}
}
