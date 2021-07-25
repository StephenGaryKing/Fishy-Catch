using PlayFab;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;
using PlayFab.Json;
using UnityEngine.Events;

public static class CurrencyTypes
{
	public readonly static string power = "PW";
	public readonly static string gold = "GL";
}

public class InventoryManager
{
	public Dictionary<string, ItemInstance> items = new Dictionary<string, ItemInstance>();
	public Dictionary<string, int> currencies = new Dictionary<string, int>();

	public InventoryManager()
	{
		GetUserInventoryRequest request = new GetUserInventoryRequest();
		PlayFabClientAPI.GetUserInventory(request, OnSuccess, OnFail);

		void OnSuccess(GetUserInventoryResult result)
		{
			foreach (var item in result.Inventory)
				items.Add(item.ItemId, item);
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

	public void PurchaseItem(PurchaseItemRequest request, Action<object> onSuccess, Action<object> onFail)
	{
		if (currencies[request.VirtualCurrency] < request.Price)
		{
			OnFail(new PlayFabError() { Error = PlayFabErrorCode.InvalidVirtualCurrency, ErrorMessage = "Not enough local currency" });
			return;
		}
		PlayFabClientAPI.PurchaseItem(request, OnSuccess, OnFail);

		void OnSuccess(PurchaseItemResult result)
		{
			currencies[request.VirtualCurrency] -= request.Price;

			foreach (var item in result.Items)
				ModifyItemAmountLocal(item, 1);

			UIManager.Instance.currencyDisplay.UpdateCurrency();
			UIManager.Instance.itemDisplay.UpdateItems();

			onSuccess?.Invoke(result.Items);
		}

		void OnFail(PlayFabError error)
		{
			Debug.LogError("Failed to sell item");
			Debug.LogError(error.GenerateErrorReport());
			onFail?.Invoke(null);
		}
	}

	public void SellItem(ItemInstance itemInstance, string currencyType, UnityEvent<object> onSuccess, UnityEvent<object> onFail)
	{
		PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
		{
			FunctionName = "SellItem",
			FunctionParameter = new Dictionary<string, object>{
            // This is a hex-string value from the GetUserInventory result
            { "soldItemInstanceId", itemInstance.ItemInstanceId },
            // Which redeemable virtual currency should be used in your game
            { "requestedVcType", currencyType },
		}
		}, OnSuccess, OnFail);

		void OnSuccess(ExecuteCloudScriptResult result)
		{
			currencies[currencyType] += (int)GameplayFlowManager.Instance.catalogueManager.GetItem(itemInstance.ItemId).VirtualCurrencyPrices[currencyType];

			ModifyItemAmountLocal(itemInstance, -1);
			
			Debug.Log("Sold item: " + itemInstance.DisplayName);
			UIManager.Instance.currencyDisplay.UpdateCurrency();
			UIManager.Instance.itemDisplay.UpdateItems();
			onSuccess?.Invoke(null);
		}

		void OnFail(PlayFabError error)
		{
			Debug.LogError("Failed to sell item");
			Debug.LogError(error.GenerateErrorReport());
			onFail?.Invoke(null);
		}
	}

	public void CheckItem(string itemID, int amount, UnityEvent<object> onSuccess, UnityEvent<object> onFail)
	{
		if (!items.ContainsKey(itemID))
		{
			var popupReference = UIManager.Instance.popupManager.ShowPopup("ItemDisplayPopup");
			var popup = popupReference.Value as ItemPopupDisplay;
			
			//Create the title
			string title;
			if (amount == 1)
				title = "You need a " + GameplayFlowManager.Instance.catalogueManager.GetItem(itemID).DisplayName + " to do that";
			else
				title = "You need " + amount + " " + GameplayFlowManager.Instance.catalogueManager.GetItem(itemID).DisplayName + "s to do that";

			//Create default buttons
			JsonObject function = new JsonObject() {
				{ "Name", "HidePopup"},
				{ "Args", new JsonObject() { { "PopupID", popupReference.Key } } }
			};
			JsonObject[] functions = new JsonObject[] { function };
			JsonObject doneButton = new JsonObject();
			doneButton.Add("Name", "Done");
			doneButton.Add("Functions", PlayFab.PfEditor.Json.PlayFabSimpleJson.SerializeObject(functions));
			List<JsonObject> buttonData = new List<JsonObject>();
			buttonData.Add(doneButton);

			popup.Setup(new object[] {
				title,
				buttonData.ToArray()
			});
			onFail?.Invoke(null);
		}
		else
			CheckItem(items[itemID], amount, onSuccess, onFail);
	}
	public void CheckItem(ItemInstance itemInstance, int amount, UnityEvent<object> onSuccess, UnityEvent<object> onFail)
	{
		PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
		{
			FunctionName = "CheckItem",
			FunctionParameter = new Dictionary<string, object>{
            // This is a hex-string value from the GetUserInventory result
            { "checkItemInstanceId", itemInstance.ItemInstanceId },
            // How many the user must have
            { "amount", amount },
		}
		}, OnSuccess, OnFail);


		void OnSuccess(ExecuteCloudScriptResult result)
		{
			if (result.Error != null)
			{
				Debug.LogError(result.Error.Message + "/n" + result.Error.StackTrace);
			}

			bool success = (bool)result.FunctionResult;
			if (success)
				onSuccess?.Invoke(null);
			else
				onFail?.Invoke(null);
		}

		void OnFail(PlayFabError error)
		{
			Debug.LogError("Failed to check item");
			Debug.LogError(error.GenerateErrorReport());
			onFail?.Invoke(null);
		}
	}

	public void DiscardItem(ItemInstance itemInstance, int amount, UnityEvent<object> onSuccess, UnityEvent<object> onFail)
	{
		PlayFabClientAPI.ConsumeItem(new ConsumeItemRequest()
		{
			ItemInstanceId = itemInstance.ItemInstanceId,
			ConsumeCount = amount
		}, OnSuccess, OnFail);

		void OnSuccess(ConsumeItemResult result)
		{
			ModifyItemAmountLocal(itemInstance, -amount);
			Debug.Log("Discarded item: " + itemInstance.DisplayName);
			UIManager.Instance.currencyDisplay.UpdateCurrency();
			UIManager.Instance.itemDisplay.UpdateItems();
			onSuccess?.Invoke(null);
		}

		void OnFail(PlayFabError error)
		{
			Debug.LogError("Failed to discard item");
			Debug.LogError(error.GenerateErrorReport());
			onFail?.Invoke(null);
		}
	}

	void ModifyItemAmountLocal(ItemInstance itemInstance, int amount)
	{
		if (items.ContainsKey(itemInstance.ItemId))
			items[itemInstance.ItemId].RemainingUses += amount;
		else
			items.Add(itemInstance.ItemId, itemInstance);

		if (items[itemInstance.ItemId].RemainingUses <= 0)
			items.Remove(itemInstance.ItemId);
	}
	void SetItemAmountLocal(ItemInstance itemInstance, int amount)
	{
		if (items.ContainsKey(itemInstance.ItemId))
			items[itemInstance.ItemId].RemainingUses = amount;
		else
			items.Add(itemInstance.ItemId, itemInstance);

		if (items[itemInstance.ItemId].RemainingUses <= 0)
			items.Remove(itemInstance.ItemId);
	}
}
