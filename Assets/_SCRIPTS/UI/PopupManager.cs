using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
	public Helper.SerializableDictionary<string, PopupDisplay> popups;
	Dictionary<byte, PopupDisplay> shownPopups = new Dictionary<byte, PopupDisplay>();
	/// <summary>
	/// Root locartion for all popups
	/// </summary>
	public Transform uiRoot;

	byte GetUID()
	{
		byte tempID = 0;
		while (shownPopups.ContainsKey(tempID))
			tempID++;
		return tempID;
	}

	public KeyValuePair<byte,PopupDisplay> ShowPopup(string popupName)
	{
		if (popups.ContainsKey(popupName))
		{
			PopupDisplay popup = Instantiate(popups[popupName], uiRoot);
			byte uid = GetUID();
			shownPopups.Add(uid, popup);
			return new KeyValuePair<byte, PopupDisplay>(uid, popup);
		}
		return new KeyValuePair<byte, PopupDisplay>(0, null);
	}

	public void HidePopup(PopupDisplay popup)
	{
		byte? uid = 0;
		foreach (var p in shownPopups)
		{
			if (p.Value == popup)
			{
				uid = p.Key;
			}
		}

		if (uid != null)
		{
			Destroy(shownPopups[(byte)uid].gameObject);
			shownPopups.Remove((byte)uid);
		}
	}
	public void HidePopup(byte uid)
	{
		if (shownPopups.ContainsKey(uid))
		{
			Destroy(shownPopups[uid].gameObject);
			shownPopups.Remove(uid);
		}
	}
}
