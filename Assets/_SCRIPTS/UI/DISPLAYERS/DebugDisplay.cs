using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugDisplay : MonoBehaviour
{
	public Text debugText;

	public void ShowDebugText(string text, bool persist = true)
	{
		Debug.Log(text);
		debugText.text = text;
	}
}
