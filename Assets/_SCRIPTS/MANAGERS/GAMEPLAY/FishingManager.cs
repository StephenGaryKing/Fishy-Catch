using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab.ClientModels;
using System;
using UnityEngine.Events;
using PlayFab.Json;
using System.Linq;

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
		UnityEvent<object> onSuccess = new UnityEvent<object>();
		onSuccess.AddListener(i => onComplete(GameplayFlowManager.Instance.catalogueManager.GetItem(i.ToString())));
		GameplayFlowManager.Instance.gatchaManager.RollTable("Anything", onSuccess, null);
	}

	bool casting = false;
	void CastFishingLine()
	{
		if (casting || fishing != null)
		{
			UIManager.Instance.debugDisplay.ShowDebugText("You can only cast your line once");
			return;
		}

		casting = true;
		//Create a random fish
		GetRandomFish(f =>
		{
			casting = false;
			fish = f;
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
		GameplayFlowManager.Instance.inventoryManager.PurchaseItem(request, OnSuccess, null);

		void OnSuccess(object items)
		{
			var item = (items as List<ItemInstance>)[0];
			var popupReference = UIManager.Instance.popupManager.ShowPopup("ItemDisplayPopup");
			var popup = popupReference.Value as ItemPopupDisplay;

			List<JsonObject> buttonData = new List<JsonObject>();
			JsonObject customData = PlayFabSimpleJson.DeserializeObject<JsonObject>(fish.CustomData);

			if (customData == null)
				customData = new JsonObject();
			if (customData.ContainsKey("Buttons"))
				buttonData = PlayFabSimpleJson.DeserializeObject<JsonObject[]>(customData["Buttons"].ToString()).ToList();
			else
				buttonData = new List<JsonObject>();

			//Create default buttons
			JsonObject function = new JsonObject() { 
				{ "Name", "HidePopup" }
			};
			JsonObject[] functions = new JsonObject[] { function };
			JsonObject doneButton = new JsonObject();
			doneButton.Add("Name", "Done");
			doneButton.Add("Functions", PlayFab.PfEditor.Json.PlayFabSimpleJson.SerializeObject(functions));
			buttonData.Add(doneButton);

			popup?.Setup(new object[]
			{
				"You caught a " + fish.DisplayName,
				buttonData.ToArray(),
				item
			});
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
