using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
	public DebugDisplay debugDisplay;
	public CurrencyDisplay currencyDisplay;
	public ItemDisplay itemDisplay;

	public static UIManager Instance;

    void Awake()
    {
		Instance = this;
	}
}
