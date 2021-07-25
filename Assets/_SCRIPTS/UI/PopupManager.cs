using PlayFab.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PopupManager : MonoBehaviour
{
	public Helper.SerializableDictionary<string, PopupDisplay> popups;
	Dictionary<byte, PopupDisplay> shownPopups = new Dictionary<byte, PopupDisplay>();
	/// <summary>
	/// Root locartion for all popups
	/// </summary>
	public Transform uiRoot;
	Stack<byte> popupStack = new Stack<byte>();

	byte GetUID()
	{
		byte tempID = 0;
		while (shownPopups.ContainsKey(tempID))
			tempID++;
		return tempID;
	}

	public KeyValuePair<byte, PopupDisplay> ShowPopup(byte popupIndex)
	{
		if (shownPopups.ContainsKey(popupIndex))
		{
			shownPopups[popupIndex].gameObject.SetActive(true);
			return new KeyValuePair<byte, PopupDisplay>(popupIndex, shownPopups[popupIndex]);
		}
		return new KeyValuePair<byte, PopupDisplay>(0, null);
	}

	public KeyValuePair<byte,PopupDisplay> ShowPopup(string popupName)
	{
		if (popups.ContainsKey(popupName))
		{
			if (popupStack.Count > 0)
				shownPopups[popupStack.Peek()].gameObject.SetActive(false);

			PopupDisplay popup = Instantiate(popups[popupName], uiRoot);
			byte uid = GetUID();
			popup.uid = uid;
			shownPopups.Add(uid, popup);
			popupStack.Push(uid);
			return new KeyValuePair<byte, PopupDisplay>(uid, popup);
		}
		return new KeyValuePair<byte, PopupDisplay>(0, null);
	}

	public void HidePopup(UnityEvent<object> onSuccess, UnityEvent<object> onFail)
	{
		if (popupStack.Count > 0)
		{
			byte uid = popupStack.Pop();
			if (shownPopups.ContainsKey(uid))
			{
				Destroy(shownPopups[uid].gameObject);
				shownPopups.Remove(uid);
				onSuccess?.Invoke(null);

				if (popupStack.Count > 0)
					shownPopups[popupStack.Peek()].gameObject.SetActive(true);
				return;
			}
		}
		onFail?.Invoke(null);
	}

	public void ShowItemRequiredPopup(string itemID, int amount)
	{
		var popupReference = ShowPopup("ItemDisplayPopup");
		var popup = popupReference.Value as ItemPopupDisplay;

		//Create the title
		string title;
		if (amount == 1)
			title = "You need a " + GameplayFlowManager.Instance.catalogueManager.GetItem(itemID).DisplayName + " to do that";
		else
			title = "You need " + amount + " " + GameplayFlowManager.Instance.catalogueManager.GetItem(itemID).DisplayName + "s to do that";

		//Create default buttons
		JsonObject function = new JsonObject() {
				{ "Name", "HidePopup"},
				{ "Args", new JsonObject() { { "PopupID", popupReference.Key } } }
			};
		JsonObject[] functions = new JsonObject[] { function };
		JsonObject doneButton = new JsonObject();
		doneButton.Add("Name", "Done");
		doneButton.Add("Functions", PlayFab.PfEditor.Json.PlayFabSimpleJson.SerializeObject(functions));
		List<JsonObject> buttonData = new List<JsonObject>();
		buttonData.Add(doneButton);

		popup.Setup(new object[] {
				title,
				buttonData.ToArray()
			});
	}
}
