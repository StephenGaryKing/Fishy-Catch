using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab.ClientModels;
using PlayFab.Json;
using System.Linq;

public class ItemUI : MonoBehaviour
{
	public Text itemName;
	public Text itemAmount;
	public Button button;
	public void ShowItem(ItemInstance item)
	{
		itemName.text = item.DisplayName;
		itemAmount.text = item.RemainingUses.ToString();
		button.onClick.AddListener(() =>
		{
			var popupReference = UIManager.Instance.popupManager.ShowPopup("ItemDisplayPopup");
			var popup = popupReference.Value as ItemPopupDisplay;

			List<JsonObject> buttonData = new List<JsonObject>();
			JsonObject customData;
			if (GameplayFlowManager.Instance.catalogueManager.GetItem(item.ItemId) == null)
				customData = new JsonObject();
			else
				customData = PlayFabSimpleJson.DeserializeObject<JsonObject>(GameplayFlowManager.Instance.catalogueManager.GetItem(item.ItemId).CustomData);

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
				item.DisplayName,
				buttonData.ToArray(),
				item
			});
		});
	}
}
