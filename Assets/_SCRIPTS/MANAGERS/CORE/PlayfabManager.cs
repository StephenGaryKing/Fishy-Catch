using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class PlayfabManager
{
    public PlayfabManager(System.Action onComplete)
	{
		Login(onComplete);
	}

	void Login(System.Action onComplete)
	{
		var request = new LoginWithCustomIDRequest
		{
			CustomId = SystemInfo.deviceUniqueIdentifier,
			CreateAccount = true
		};
		PlayFabClientAPI.LoginWithCustomID(request, OnSuccess, OnFail);

		void OnSuccess(LoginResult result)
		{
			Debug.Log("Successful login: " + result.PlayFabId);
			onComplete?.Invoke();
		}

		void OnFail(PlayFabError error)
		{
			Debug.LogError("Failed login");
			Debug.LogError(error.GenerateErrorReport());
		}
	}
}
