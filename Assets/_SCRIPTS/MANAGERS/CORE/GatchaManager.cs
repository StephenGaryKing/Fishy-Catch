using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;
using PlayFab.Json;

public class GatchaManager
{

	public void RollTable(string tableName, System.Action<string> onComplete)
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
			onComplete(itemID.ToString());
		}

		void OnFail(PlayFabError error)
		{
			Debug.LogError("Failed login");
			Debug.LogError(error.GenerateErrorReport());
		}
	}
}
