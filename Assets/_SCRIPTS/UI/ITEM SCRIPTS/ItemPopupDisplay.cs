using PlayFab.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
				HandleFunction(functions, args.Length > 2? args[2] : null, 0);
			});
		}
	}

	void HandleFunction(JsonObject[] functions, object sender, int currentFunctionIndex)
	{
		if (currentFunctionIndex >= functions.Length)
			return;
		var function = functions[currentFunctionIndex];
		Dictionary<string, object> functionArgs;
		if (function.ContainsKey("Args"))
			functionArgs = PlayFabSimpleJson.DeserializeObject<Dictionary<string, object>>(function["Args"].ToString());
		else
			functionArgs = new Dictionary<string, object>();
		functionArgs.Add("Sender", sender);

		//Modify the Onsuccess callback
		if (!functionArgs.ContainsKey("OnSuccess"))
			functionArgs.Add("OnSuccess", new Helper.OnEvent());
		Helper.OnEvent onSuccess = (Helper.OnEvent)functionArgs["OnSuccess"];
		onSuccess.AddListener(o => HandleFunction(functions, sender, currentFunctionIndex + 1));
		functionArgs["OnSuccess"] = onSuccess;

		//Modify the OnFail callback
		if (!functionArgs.ContainsKey("OnFail"))
			functionArgs.Add("OnFail", new Helper.OnEvent());
		Helper.OnEvent onFail = (Helper.OnEvent)functionArgs["OnFail"];
		onFail.AddListener(o => UIManager.Instance.debugDisplay.ShowDebugText("Failed to execute " + function["Name"].ToString()));
		functionArgs["OnFail"] = onFail;

		Helper.ExecuteGenericFunction(function["Name"].ToString(), functionArgs);
	}
}
