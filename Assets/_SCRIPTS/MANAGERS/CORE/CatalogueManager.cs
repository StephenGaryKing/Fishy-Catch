using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using PlayFab;
using PlayFab.ClientModels;

public class CatalogueManager
{
	public static Dictionary<string, Dictionary<string, CatalogItem>> catalogs = new Dictionary<string, Dictionary<string, CatalogItem>>();

	public CatalogueManager()
	{
		GetCatalogItemsRequest request = new GetCatalogItemsRequest
		{
			CatalogVersion = "Standard_Fish"
		};

		PlayFabClientAPI.GetCatalogItems(request, OnSuccess, OnFail);

		void OnSuccess(GetCatalogItemsResult result)
		{
			Debug.Log("Catalog downloaded: " + request.CatalogVersion);
			SetupCatalog(request.CatalogVersion, result.Catalog);
		}

		void OnFail(PlayFabError error)
		{
			Debug.LogError("Catalog failed to download");
			Debug.LogError(error.GenerateErrorReport());
		}
	}

	void SetupCatalog(string catalogName, List<CatalogItem> catalog)
	{
		Dictionary<string, CatalogItem> result = new Dictionary<string, CatalogItem>();
		foreach (var item in catalog)
			result.Add(item.ItemId, item);

		catalogs.Add(catalogName, result);
	}

	public CatalogItem GetItem(string itemName)
	{
		foreach(var cat in catalogs)
		{
			if (cat.Value.ContainsKey(itemName))
				return cat.Value[itemName];
		}
		return null;
	}
}
