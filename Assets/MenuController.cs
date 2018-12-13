using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
	[SerializeField] private List<List<MenuCell>> menu;
	private int i;
	private int j;

	public void Start()
	{
		i = 0;
		j = 0;

		//List<string> position = new List<string>
		//{
		//	"position",
		//	"X",
		//	"Y",
		//	"Z"
		//};

		//List<string> rotation = new List<string>
		//{
		//	"rotation",
		//	"X",
		//	"Y",
		//	"Z"
		//};

		//List<string> modification = new List<string>
		//{
		//	"modification"
		//};

		//menu = new List<List<string>>
		//{
		//	position,
		//	rotation,
		//	modification
		//};
	}

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.A))
		{
			print("A");
			MoveLeft();
			PrintPosition();
		}
		if (Input.GetKeyDown(KeyCode.W))
		{
			print("W");
			MoveUp();
			PrintPosition();
		}
		if (Input.GetKeyDown(KeyCode.D))
		{
			print("D");
			MoveRight();
			PrintPosition();
		}
		if (Input.GetKeyDown(KeyCode.S))
		{
			print("S");
			MoveDown();
			PrintPosition();
		}

	}

	private void MoveLeft()
	{
		if (j == 0)
			return;

		j--;
	}

	private void MoveRight()
	{
		if (j == menu[i].Count - 1)
			return;

		j++;
	}

	private void MoveUp()
	{
		if (i == 0)
			return;

		i--;
		j = 0;
	}

	private void MoveDown()
	{
		if (i == menu.Count - 1)
			return;

		i++;
		j = 0;
	}

	private void PrintPosition()
	{
		Debug.Log("i: " + i + "; j: " + j + "; ");
		Debug.Log(menu[i][0]);
		if (j > 0)
			Debug.Log(menu[i][j]);

	}
}