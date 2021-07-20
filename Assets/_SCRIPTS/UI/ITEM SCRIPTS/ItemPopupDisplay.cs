using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ItemPopupDisplay : PopupDisplay
{
	public Text title;
	public GridLayoutGroup grid;

	public GameObject buttonPrefab;

	/// <summary>
	/// Set up the popup with the info it will display
	/// </summary>
	/// <param name="args">[ (string)itemName, [(string)behaviourName, (Action)behaviour] ]</param>
	public override void Setup(object[] args)
	{
		title.text = args[0].ToString();
		foreach (object[] btnInfo in args[1] as object[])
		{
			var button = Instantiate(buttonPrefab, grid.transform);
			button.GetComponentInChildren<Text>().text = btnInfo[0].ToString();
			button.GetComponentInChildren<Button>().onClick.AddListener(btnInfo[1] as UnityAction);
		}
	}
}
