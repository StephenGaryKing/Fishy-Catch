using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;
using PlayFab.Json;
using System;
using UnityEngine.Events;

public class GatchaManager
{

	public void RollTable(string tableName, UnityEvent<object> onSuccess, UnityEvent<object> onFail)
	{
		PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
		{
			FunctionName = "RollTable",
			FunctionParameter = new { TableId = tableName }
		},OnSuccess, OnFail);


		void OnSuccess(ExecuteCloudScriptResult result)
		{
			JsonObject obj = (JsonObject)result.FunctionResult;
			obj.TryGetValue("ResultItemId", out object itemID);
			onSuccess?.Invoke(itemID);
		}

		void OnFail(PlayFabError error)
		{
			Debug.LogError("Failed login");
			Debug.LogError(error.GenerateErrorReport());
			onFail?.Invoke(null);
		}
	}
}
