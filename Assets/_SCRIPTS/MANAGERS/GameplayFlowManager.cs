using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;

public class GameplayFlowManager : MonoBehaviour
{
	[HideInInspector]
	public PlayfabManager playfabManager;
	[HideInInspector]
	public CatalogueManager catalogueManager;
	[HideInInspector]
	public InventoryManager inventoryManager;

	public static GameplayFlowManager Instance;

	void Awake()
    {
		Instance = this;

		InitPlayfabManager(() =>
		{
			InitCatalogueManager();
			InitInventoryManager();
		});
    }

	void InitPlayfabManager(System.Action onComplete)
	{
		playfabManager = new PlayfabManager(onComplete);
	}

	void InitCatalogueManager()
	{
		catalogueManager = new CatalogueManager();
		catalogueManager.GetCatalogue("Standard_Fish");
	}

	void InitInventoryManager()
	{
		inventoryManager = new InventoryManager();
	}
}
