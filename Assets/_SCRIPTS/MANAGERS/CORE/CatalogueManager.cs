using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using PlayFab;
using PlayFab.ClientModels;

public class CatalogueManager
{
	public static List<CatalogItem> standardFish;

	public void GetCatalogue(string catalogueName)
	{
		GetCatalogItemsRequest request = new GetCatalogItemsRequest
		{
			CatalogVersion = catalogueName
		};

		PlayFabServerAPI.GetCatalogItems(request, OnSuccess, OnFail);

		void OnSuccess(GetCatalogItemsResult result)
		{
			Debug.Log("Catalogue downloaded: " + catalogueName);
			standardFish = result.Catalog;

			foreach (var i in standardFish)
				Debug.Log(i.DisplayName);
		}

		void OnFail(PlayFabError error)
		{
			Debug.LogError("Catalogue failed to download");
			Debug.LogError(error.GenerateErrorReport());
		}
	}
}
