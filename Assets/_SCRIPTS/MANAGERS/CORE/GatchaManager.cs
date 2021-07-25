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
		UIManager.Instance.popupManager.ShowPopup("LoadingPopup");
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
			UIManager.Instance.popupManager.HidePopup(null, null);
			JsonObject JObject = (JsonObject)result.FunctionResult;
			if (JObject.ContainsKey("Success"))
			{
				if (!(bool)JObject["Success"])
				{
					UIManager.Instance.popupManager.ShowItemRequiredPopup(GameplayFlowManager.Instance.catalogueManager.GetItem(item.ItemId).Container.KeyItemId, 1);
					onFail?.Invoke(null);
					return;
				}
			}

			string keyId = GameplayFlowManager.Instance.catalogueManager.GetItem(item.ItemId).Container.KeyItemId;
			GameplayFlowManager.Instance.inventoryManager.ModifyItemAmountLocal(GameplayFlowManager.Instance.inventoryManager.items[keyId], -1);
			GameplayFlowManager.Instance.inventoryManager.ModifyItemAmountLocal(item, -1);
			ItemInstance[] items = null;
			if (JObject.ContainsKey("GrantedItems"))
			{
				items = PlayFabSimpleJson.DeserializeObject<ItemInstance[]>(JObject["GrantedItems"].ToString());
				foreach (var i in items)
					GameplayFlowManager.Instance.inventoryManager.SetItemAmountLocal(i, (int)i.RemainingUses);
			}

			UIManager.Instance.itemDisplay.UpdateItems(GameplayFlowManager.Instance.inventoryManager.items);
			onSuccess?.Invoke(items);
		}

		void OnFail(PlayFabError error)
		{
			UIManager.Instance.popupManager.HidePopup(null, null);
			Debug.LogError("Failed login");
			Debug.LogError(error.GenerateErrorReport());
			onFail?.Invoke(null);
		}
	}
}
