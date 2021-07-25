using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;

public class ItemDisplay : MonoBehaviour
{
	public GameObject itemUIPrefab;
	public Transform contentArea;

	public void UpdateItems(Dictionary<string, ItemInstance> items)
	{
		ClearItems();
		foreach(var item in items)
			DisplayItem(item.Value);
	}

	void ClearItems()
	{
		foreach (Transform child in contentArea)
			Destroy(child.gameObject);
	}

	void DisplayItem(ItemInstance item)
	{
		GameObject go = Instantiate(itemUIPrefab, contentArea);
		go?.GetComponentInChildren<ItemUI>()?.ShowItem(item);
	}
}
