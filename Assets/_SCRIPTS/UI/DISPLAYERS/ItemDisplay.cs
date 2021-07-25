using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;
using PlayFab.SharedModels;
using PlayFab.Json;

public class ItemDisplay : MonoBehaviour
{
	public GameObject itemUIPrefab;
	public Transform contentArea;

	public void UpdateItems(Dictionary<string, ItemInstance> items)
	{
		ClearItems();
		foreach(var item in items)
			DisplayItem(PlayFabSimpleJson.SerializeObject(item.Value));
	}

	public void UpdateItems(Dictionary<string, StoreItem> items)
	{
		ClearItems();
		foreach(var item in items)
			DisplayItem(PlayFabSimpleJson.SerializeObject(item.Value));
	}

	void ClearItems()
	{
		foreach (Transform child in contentArea)
			Destroy(child.gameObject);
	}

	void DisplayItem(string itemJson)
	{
		GameObject go = Instantiate(itemUIPrefab, contentArea);
		go?.GetComponentInChildren<ItemUI>()?.ShowItem(itemJson);
	}
}
