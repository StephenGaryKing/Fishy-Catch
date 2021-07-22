using PlayFab.Json;
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
	/// <param name="args">[ (string)itemName, (JsonObject)funtions ]</param>
	public override void Setup(object[] args)
	{
		title.text = args[0].ToString();
		foreach (JsonObject funcInfo in args[1] as JsonObject[])
		{
			var button = Instantiate(buttonPrefab, grid.transform);
			button.GetComponentInChildren<Text>().text = funcInfo["Name"].ToString();
			button.GetComponentInChildren<Button>().onClick.AddListener(() =>
			{
				string json = funcInfo["Functions"].ToString();
				JsonObject[] functions = PlayFabSimpleJson.DeserializeObject<JsonObject[]>(json);
				foreach (var function in functions)
				{
					if (Helper.GenericFunctions.ContainsKey(function["Name"].ToString()))
					{
						JsonObject functionArgs = PlayFabSimpleJson.DeserializeObject<JsonObject>(function["Args"].ToString());
						functionArgs.Add("Sender", args[2]);
						Helper.GenericFunctions[function["Name"].ToString()]?.Invoke(functionArgs);
					}
				}
				UIManager.Instance.popupManager.HidePopup(this);
			});
		}
	}
}
