using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class PopupDisplay : MonoBehaviour
{
	public abstract void Setup(object[] args);
}
