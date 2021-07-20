﻿using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab.ClientModels;
using System;
using UnityEngine;
using UnityEngine.Events;

public class FishingManager : MonoBehaviour
{
	public Button castButton;
	public Button reelButton;

	Coroutine fishing;
	bool biting = false;
	CatalogItem fish;

	private void Start()
	{
		castButton.onClick.AddListener(CastFishingLine);
		reelButton.onClick.AddListener(ReelFishingLine);
	}

	void GetRandomFish(System.Action<CatalogItem> onComplete)
	{
		GameplayFlowManager.Instance.gatchaManager.RollTable("Anything", i => onComplete(GameplayFlowManager.Instance.catalogueManager.GetItem(i)));
	}

	void CastFishingLine()
	{
		//Create a random fish
		GetRandomFish(f =>
		{
			fish = f;

			if (fishing != null)
				UIManager.Instance.debugDisplay.ShowDebugText("You can only cast your line once");
			else
				fishing = StartCoroutine(Cast(5f, CastFishingLine));
		});
	}

	void ReelFishingLine()
	{
		if (fishing != null)
		{
			StopCoroutine(fishing);
			fishing = null;

			if (biting)
				CatchFish();
			else
				CatchNothing();

			biting = false;
		}
		else
		{
			UIManager.Instance.debugDisplay.ShowDebugText("Cast your line before you reel it in");
		}
	}

	void Bite()
	{
		UIManager.Instance.debugDisplay.ShowDebugText("Fish is biting");
		biting = true;
	}

	void FakeBite()
	{
		UIManager.Instance.debugDisplay.ShowDebugText("Fish is fake biting");
	}

	void CatchNothing()
	{
		UIManager.Instance.debugDisplay.ShowDebugText("Splash! The " + fish.DisplayName + " got away");
	}

	void CatchFish()
	{
		PurchaseItemRequest request = new PurchaseItemRequest()
		{
			CatalogVersion = "Standard_Fish",
			ItemId = fish.ItemId,
			VirtualCurrency = "PW",
			Price = (int)fish.VirtualCurrencyPrices["PW"]
		};
		GameplayFlowManager.Instance.inventoryManager.PurchaseItem(request, Onsuccess, OnFail);

		void Onsuccess(PurchaseItemResult result)
		{
			var item = result.Items[0];
			var popup = UIManager.Instance.popupManager.ShowPopup("ItemDisplayPopup") as ItemPopupDisplay;
			popup?.Setup(new object[]
			{
				"You caught a " + fish.DisplayName,
				new object[]
				{
					new object[]
					{
						"Keep",
						new UnityAction(() =>
						{
							UIManager.Instance.debugDisplay.ShowDebugText(item.DisplayName + " was kept");
							UIManager.Instance.popupManager.HidePopup(popup);
						})
					},
					new object[]
					{
						"Sell",
						new UnityAction(() =>
						{
							GameplayFlowManager.Instance.inventoryManager.SellItem(item, CurrencyTypes.gold);
							UIManager.Instance.debugDisplay.ShowDebugText(item.DisplayName + " was sold");
							UIManager.Instance.popupManager.HidePopup(popup);
						})
					}
				}
			});
		}

		void OnFail(PlayFabError error)
		{
			UIManager.Instance.debugDisplay.ShowDebugText("failed to add " + fish.DisplayName + " to the player's inventory");
			Debug.LogError(error.GenerateErrorReport());
		}
	}

	IEnumerator Cast(float timeToCatch, System.Action OnTimeout)
	{
		while (!biting)
		{
			UIManager.Instance.debugDisplay.ShowDebugText("Fishing: " + fish.DisplayName);
			yield return new WaitForSeconds(timeToCatch);
			Bite();
			//if (UnityEngine.Random.Range(0, 100) < 20)
			//	Bite();
			//else
			//	FakeBite();
			yield return new WaitForSeconds(2f);
		}

		UIManager.Instance.debugDisplay.ShowDebugText("Fish got away");
		biting = false;
		fishing = null;
		OnTimeout();
	}
}
