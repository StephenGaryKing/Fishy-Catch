using PlayFab.ClientModels;
using PlayFab.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper
{
	public static Dictionary<string, System.Action<object>> GenericFunctions = new Dictionary<string, System.Action<object>>()
	{
		{ 
			"SellItem", (o) =>
			{
				JsonObject jObject = o as JsonObject;
				GameplayFlowManager.Instance.inventoryManager.SellItem(jObject["Sender"] as ItemInstance, jObject["CurrencyType"].ToString());
			}
		},
		{ 
			"HidePopup", (o) =>
			{
				JsonObject jObject = o as JsonObject;
				UIManager.Instance.popupManager.HidePopup(byte.Parse(jObject["PopupID"].ToString()));
			}
		}
	};


	[System.Serializable]
	public class SerializableDictionary<K, V>
	{ 
		[System.Serializable]
		class Data
		{
			public K Key;
			public V Value;
		}

		[SerializeField]
		List<Data> list = null;
		Dictionary<K, V> dict = null;

		void VarifyDictionaryIntegrity()
		{
			if (dict == null)
			{
				dict = new Dictionary<K, V>();
				foreach (var content in list)
					dict.Add(content.Key, content.Value);
			}
		}

		public void Add(K key, V value)
		{
			VarifyDictionaryIntegrity();
			dict.Add(key, value);
		}
		public void Remove(K key)
		{
			VarifyDictionaryIntegrity();
			dict.Remove(key);
		}
		public void Clear()
		{
			VarifyDictionaryIntegrity();
			dict.Clear();
		}
		public bool ContainsKey(K key)
		{
			VarifyDictionaryIntegrity();
			return dict.ContainsKey(key);
		}
		public V this[K key] => dict[key];

	}
}
