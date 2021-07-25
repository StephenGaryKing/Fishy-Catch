using PlayFab;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;
using PlayFab.Json;
using UnityEngine.Events;
using PlayFab.SharedModels;

public class StoreManager
{
	public static Dictionary<string, Dictionary<string, StoreItem>> stores = new Dictionary<string, Dictionary<string, StoreItem>>();

	public StoreManager()
	{
		GetStoreItemsRequest request = new GetStoreItemsRequest()
		{
			StoreId = "Adelaide_SA_Australia"
		};
		PlayFabClientAPI.GetStoreItems(request, OnSuccess, OnFail);
	
		void OnSuccess(GetStoreItemsResult result)
		{
			Debug.Log("Shop downloaded: " + request.CatalogVersion);
			SetupCatalog(request.StoreId, result.Store);
		}
	
		void OnFail(PlayFabError error)
		{
			Debug.LogError("Failed login");
			Debug.LogError(error.GenerateErrorReport());
		}
	}

	void SetupCatalog(string storeName, List<StoreItem> catalog)
	{
		Dictionary<string, StoreItem> result = new Dictionary<string, StoreItem>();
		foreach (var item in catalog)
			result.Add(item.ItemId, item);

		stores.Add(storeName, result);
		UIManager.Instance.storeDisplay.UpdateItems(result);
	}

	public StoreItem GetItem(string itemID)
	{
		foreach (var store in stores)
		{
			if (store.Value.ContainsKey(itemID))
				return store.Value[itemID];
		}
		return null;
	}

}
