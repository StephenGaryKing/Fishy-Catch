using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyDisplay : MonoBehaviour
{
	public Text powerText;
	public Text goldText;

	public void UpdateCurrency()
	{
		if (GameplayFlowManager.Instance.inventoryManager.currencies.ContainsKey(CurrencyTypes.power))
			powerText.text = GameplayFlowManager.Instance.inventoryManager.currencies[CurrencyTypes.power].ToString();
		if (GameplayFlowManager.Instance.inventoryManager.currencies.ContainsKey(CurrencyTypes.gold))
			goldText.text = GameplayFlowManager.Instance.inventoryManager.currencies[CurrencyTypes.gold].ToString();
	}
}
