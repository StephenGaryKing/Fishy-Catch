using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab.ClientModels;
using PlayFab.Json;
using System.Linq;
using PlayFab.SharedModels;

public class ItemUI : MonoBehaviour
{
	public Text itemName;
	public Text itemAmount;
	public Button button;
	public void ShowItem(string itemJson)
	{
		JsonObject jObject = PlayFabSimpleJson.DeserializeObject<JsonObject>(itemJson);

		if (jObject.ContainsKey("ItemId"))
		{
			string itemId = jObject["ItemId"].ToString();
			string displayName = GameplayFlowManager.Instance.catalogueManager.GetItem(itemId).DisplayName;
			itemName.text = displayName;

			if (jObject.ContainsKey("RemainingUses"))
				itemAmount.text = jObject["RemainingUses"].ToString();

			button.onClick.AddListener(() =>
			{
				var popupReference = UIManager.Instance.popupManager.ShowPopup("ItemDisplayPopup");
				var popup = popupReference.Value as ItemPopupDisplay;

				List<JsonObject> buttonData = new List<JsonObject>();
				JsonObject customData;
				if (GameplayFlowManager.Instance.catalogueManager.GetItem(itemId) == null)
					customData = new JsonObject();
				else
					customData = PlayFabSimpleJson.DeserializeObject<JsonObject>(GameplayFlowManager.Instance.catalogueManager.GetItem(itemId).CustomData);

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
				displayName,
				buttonData.ToArray(),
				itemJson
				});
			});
		}
	}
}
