﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;

public class ItemDisplay : MonoBehaviour
{
	public GameObject itemUIPrefab;
	public Transform contentArea;

	public void UpdateItems()
	{
		ClearItems();
		foreach(var item in GameplayFlowManager.Instance.inventoryManager.items)
			DisplayItem(item);
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