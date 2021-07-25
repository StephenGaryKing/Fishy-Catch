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
			FunctionParameter = new Dictionary<string, object>
			{
				{ "tableId", tableName }
			}
		},OnSuccess, OnFail);


		void OnSuccess(ExecuteCloudScriptResult result)
		{
			if (result.Error != null)
			{
				Debug.LogError(result.Error.Message + "\n" + result.Error.StackTrace);
			}

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
	
	public void OpenContainer(ItemInstance item, UnityEvent<object> onSuccess, UnityEvent<object> onFail)
	{
		PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
		{
			FunctionName = "OpenContainer",
			FunctionParameter = new Dictionary<string, object>
			{
				{ "itemId", item.ItemId },
				{ "containerInstanceId", item.ItemInstanceId },
				{ "catalogVersion", item.CatalogVersion }
			}
		},OnSuccess, OnFail);


		void OnSuccess(ExecuteCloudScriptResult result)
		{
			Debug.Log(result.FunctionResult);
			if (result.Error != null)
			{
				Debug.LogError(result.Error.Message + "\n" + result.Error.StackTrace);
			}

			JsonObject obj = (JsonObject)result.FunctionResult;
			if (obj.ContainsKey("Success"))
			{
				if (!(bool)obj["Success"])
				{
					onFail?.Invoke(null);
					return;
				}
			}
			obj.TryGetValue("GrantedItems", out object itemIds);
			onSuccess?.Invoke(itemIds);
		}

		void OnFail(PlayFabError error)
		{
			Debug.LogError("Failed login");
			Debug.LogError(error.GenerateErrorReport());
			onFail?.Invoke(null);
		}
	}
}
