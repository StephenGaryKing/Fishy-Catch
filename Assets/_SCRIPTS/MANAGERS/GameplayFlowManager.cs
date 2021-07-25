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
	public StoreManager storeManager;
	[HideInInspector]
	public InventoryManager inventoryManager;
	[HideInInspector]
	public GatchaManager gatchaManager;

	public static GameplayFlowManager Instance;

	void Awake()
    {
		Instance = this;

		InitPlayfabManager(() =>
		{
			InitCatalogueManager();
		});
    }

	public void InitPlayfabManager(System.Action onComplete)
	{
		if (playfabManager == null)
			playfabManager = new PlayfabManager(onComplete);
	}

	public void InitCatalogueManager()
	{
		if (catalogueManager == null)
			catalogueManager = new CatalogueManager();
	}

	public void InitStoreManager()
	{
		if (storeManager == null)
			storeManager = new StoreManager();
	}

	public void InitInventoryManager()
	{
		if (inventoryManager == null)
			inventoryManager = new InventoryManager();
	}

	public void InitGatchaManager()
	{
		if (gatchaManager == null)
			gatchaManager = new GatchaManager();
	}
}
