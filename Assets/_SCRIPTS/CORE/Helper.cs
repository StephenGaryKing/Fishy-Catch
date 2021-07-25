using Newtonsoft.Json.Linq;
using PlayFab.ClientModels;
using PlayFab.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Events;

public static class Helper
{
	[Serializable]
	public class OnEvent : UnityEvent<object>
	{
	}

	public static Dictionary<string, System.Action<object>> GenericFunctions = new Dictionary<string, System.Action<object>>()
	{
		{ 
			"SellItem", (o) =>
			{
				Dictionary<string, object> packet = o as Dictionary<string, object>;
				OnEvent OnSuccess = (OnEvent)packet["OnSuccess"];
				OnEvent OnFail = (OnEvent)packet["OnFail"];
				GameplayFlowManager.Instance.inventoryManager.SellItem(packet["Sender"] as ItemInstance, packet["CurrencyType"].ToString(), OnSuccess, OnFail);
			}
		},
		{ 
			"HidePopup", (o) =>
			{
				Dictionary<string, object> packet = o as Dictionary<string, object>;
				OnEvent OnSuccess = (OnEvent)packet["OnSuccess"];
				OnEvent OnFail = (OnEvent)packet["OnFail"];
				UIManager.Instance.popupManager.HidePopup(OnSuccess, OnFail);
			}
		},
		{
			"CheckItem", (o) =>
			{
				Dictionary<string, object> packet = o as Dictionary<string, object>;
				OnEvent OnSuccess = (OnEvent)packet["OnSuccess"];
				OnEvent OnFail = (OnEvent)packet["OnFail"];
				GameplayFlowManager.Instance.inventoryManager.CheckItem(packet["ItemID"].ToString(), int.Parse(packet["Amount"].ToString()), OnSuccess, OnFail);
			}
		},
		{
			"RollTable", (o) =>
			{
				Dictionary<string, object> packet = o as Dictionary<string, object>;
				OnEvent OnSuccess = (OnEvent)packet["OnSuccess"];
				OnEvent OnFail = (OnEvent)packet["OnFail"];
				GameplayFlowManager.Instance.gatchaManager.RollTable(packet["TableID"].ToString(), OnSuccess, OnFail);
			}
		},
		{
			"GrantTable", (o) =>
			{
				Dictionary<string, object> packet = o as Dictionary<string, object>;
				OnEvent OnSuccess = (OnEvent)packet["OnSuccess"];
				OnEvent OnFail = (OnEvent)packet["OnFail"];
				GameplayFlowManager.Instance.gatchaManager.GrantTable(packet["TableID"].ToString(), OnSuccess, OnFail);
			}
		},
		{
			"DiscardItem", (o) =>
			{
				Dictionary<string, object> packet = o as Dictionary<string, object>;
				OnEvent OnSuccess = (OnEvent)packet["OnSuccess"];
				OnEvent OnFail = (OnEvent)packet["OnFail"];
				ItemInstance itemInstance;
				if (packet.ContainsKey("ItemID"))
					itemInstance = GameplayFlowManager.Instance.inventoryManager.items[packet["ItemID"].ToString()];
				else
					itemInstance = packet["Sender"] as ItemInstance;
				GameplayFlowManager.Instance.inventoryManager.DiscardItem(itemInstance, int.Parse(packet["Amount"].ToString()), OnSuccess, OnFail);
			}
		}
	};

	public static void ExecuteGenericFunction(string functionName, object args)
	{
		Dictionary<string, object> packet = args as Dictionary<string, object>;
		try
		{
			if (GenericFunctions.ContainsKey(functionName))
				GenericFunctions[functionName]?.Invoke(args);
			else
				((OnEvent)packet["OnFail"])?.Invoke(null);
		}
		catch (Exception e)
		{
			Debug.LogError(e.Message + "\n" + e.StackTrace);
			((OnEvent)packet["OnFail"])?.Invoke(null);
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
