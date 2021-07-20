using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
	public Helper.SerializableDictionary<string, PopupDisplay> popups;
	/// <summary>
	/// Root locartion for all popups
	/// </summary>
	public Transform uiRoot;

	public PopupDisplay ShowPopup(string popupName)
	{
		if (popups.ContainsKey(popupName))
			return Instantiate(popups[popupName], uiRoot);
		return null;
	}

	public void HidePopup(PopupDisplay popup)
	{
		Destroy(popup.gameObject);
	}
}
