using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab.ClientModels;

public class ItemUI : MonoBehaviour
{
	public Text itemName;
	public Text itemAmount;

	public void ShowItem(ItemInstance item)
	{
		itemName.text = item.DisplayName;
		itemAmount.text = item.RemainingUses.ToString();
	}
}
