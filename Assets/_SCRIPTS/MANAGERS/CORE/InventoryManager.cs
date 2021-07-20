using PlayFab;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;
using PlayFab.Json;

public static class CurrencyTypes
{
	public readonly static string power = "PW";
	public readonly static string gold = "GL";
}

public class InventoryManager
{
	public List<ItemInstance> items = new List<ItemInstance>();
	public Dictionary<string, int> currencies = new Dictionary<string, int>();

	public InventoryManager()
	{
		GetUserInventoryRequest request = new GetUserInventoryRequest();
		PlayFabClientAPI.GetUserInventory(request, OnSuccess, OnFail);

		void OnSuccess(GetUserInventoryResult result)
		{
			items = result.Inventory;
			currencies = result.VirtualCurrency;
			UIManager.Instance?.currencyDisplay.UpdateCurrency();
			UIManager.Instance?.itemDisplay.UpdateItems();
		}

		void OnFail(PlayFabError error)
		{
			Debug.LogError("Failed login");
			Debug.LogError(error.GenerateErrorReport());
		}
	}

	public void PurchaseItem(PurchaseItemRequest request, Action<PurchaseItemResult> Onsuccess, Action<PlayFabError> OnFail)
	{
		if (currencies[request.VirtualCurrency] < request.Price)
		{
			OnFail(new PlayFabError() { Error = PlayFabErrorCode.InvalidVirtualCurrency, ErrorMessage = "Not enough local currency" });
			return;
		}

		Onsuccess += (r) =>
		{
			currencies[request.VirtualCurrency] -= request.Price;

			foreach (var item in r.Items)
			{
				bool itemAddedLocaly = false;
				for (int i = 0; i < items.Count; i ++)
				{
					if (items[i].ItemInstanceId == item.ItemInstanceId)
					{
						items[i] = item;
						itemAddedLocaly = true;
						break;
					}
				}
				if (!itemAddedLocaly)
					items.Add(item);
			}

			UIManager.Instance.currencyDisplay.UpdateCurrency();
			UIManager.Instance.itemDisplay.UpdateItems();

			foreach (var i in items)
			{
				Debug.Log(i.DisplayName + ": " + i.RemainingUses);
			}
		};
		PlayFabClientAPI.PurchaseItem(request, Onsuccess, OnFail);
	}

	public void SellItem(ItemInstance itemInstance, string currencyType)
	{
		PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
		{
			FunctionName = "SellItem",
			FunctionParameter = new Dictionary<string, string>{
            // This is a hex-string value from the GetUserInventory result
            { "soldItemInstanceId", itemInstance.ItemInstanceId },
            // Which redeemable virtual currency should be used in your game
            { "requestedVcType", currencyType },
		}
		}, OnSuccess, OnFail);


		void OnSuccess(ExecuteCloudScriptResult result)
		{
			if (result.Error != null)
			{
				Debug.LogError(result.Error.StackTrace);
				return;
			}

			currencies[currencyType] += (int)GameplayFlowManager.Instance.catalogueManager.GetItem(itemInstance.ItemId).VirtualCurrencyPrices[currencyType];
			
			//decrement item count
			
			Debug.Log("Sold item: " + itemInstance.DisplayName);
			UIManager.Instance.currencyDisplay.UpdateCurrency();
			UIManager.Instance.itemDisplay.UpdateItems();
		}

		void OnFail(PlayFabError error)
		{
			Debug.LogError("Failed to sell item");
			Debug.LogError(error.GenerateErrorReport());
		}
	}
}
