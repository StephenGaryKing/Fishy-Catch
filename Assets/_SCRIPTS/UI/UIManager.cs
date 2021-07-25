using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
	[Header("Managers")]
	public PopupManager popupManager;

	[Header("Displays")]
	public DebugDisplay debugDisplay;
	public CurrencyDisplay currencyDisplay;
	public ItemDisplay itemDisplay;
	public ItemDisplay shopDisplay;

	public static UIManager Instance;

    void Awake()
    {
		Instance = this;
	}
}
