using Newtonsoft.Json.Linq;
using PlayFab.ClientModels;
using PlayFab.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public static class Helper
{
	public static Dictionary<string, System.Action<object>> GenericFunctions = new Dictionary<string, System.Action<object>>()
	{
		{ 
			"SellItem", (o) =>
			{
				JsonObject jObject = o as JsonObject;
				Action<object> OnSuccess = (Action<object>)jObject["OnSuccess"];
				Action<object> OnFail = (Action<object>)jObject["OnFail"];
				GameplayFlowManager.Instance.inventoryManager.SellItem(jObject["Sender"] as ItemInstance, jObject["CurrencyType"].ToString(), OnSuccess, OnFail);
			}
		},
		{ 
			"HidePopup", (o) =>
			{
				JsonObject jObject = o as JsonObject;
				Action<object> OnSuccess = (Action<object>)jObject["OnSuccess"];
				Action<object> OnFail = (Action<object>)jObject["OnFail"];
				UIManager.Instance.popupManager.HidePopup(byte.Parse(jObject["PopupID"].ToString()), OnSuccess, OnFail);
			}
		},
		{
			"CheckItem", (o) =>
			{
				JsonObject jObject = o as JsonObject;
				Action<object> OnSuccess = (Action<object>)jObject["OnSuccess"];
				Action<object> OnFail = (Action<object>)jObject["OnFail"];
				GameplayFlowManager.Instance.inventoryManager.CheckItem(jObject["ItemID"].ToString(), int.Parse(jObject["Amount"].ToString()), OnSuccess, OnFail);
			}
		},
		{
			"RollTable", (o) =>
			{
				JsonObject jObject = o as JsonObject;
				Action<object> OnSuccess = (Action<object>)jObject["OnSuccess"];
				Action<object> OnFail = (Action<object>)jObject["OnFail"];
				GameplayFlowManager.Instance.gatchaManager.RollTable(jObject["TableID"].ToString(), OnSuccess, OnFail);
			}
		},
		{
			"DiscardItem", (o) =>
			{
				JsonObject jObject = o as JsonObject;
				GameplayFlowManager.Instance.inventoryManager.DiscardItem(jObject["ItemInstance"] as ItemInstance, int.Parse(jObject["Amount"].ToString()), null, null);
			}
		}
	};

	public static void ExecuteGenericFunction(string functionName, object args)
	{
		JsonObject jObject = args as JsonObject;
		try
		{
			if (GenericFunctions.ContainsKey(functionName))
				GenericFunctions[functionName]?.Invoke(args);
			else
				((Action<object>)jObject["OnSuccess"])?.Invoke(null);
		}
		catch (Exception e)
		{
			Debug.LogError(e.Message + "\n" + e.StackTrace);
			((Action<object>)jObject["OnFail"])?.Invoke(null);
		}
	}


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
