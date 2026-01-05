using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayFabLogin : MonoBehaviour
{
    [SerializeField] private string titleId;

    void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
            PlayFabSettings.staticSettings.TitleId = titleId;

        var req = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(req, OnSuccess, OnError);
    }

    void OnSuccess(LoginResult r)
    {
        Debug.Log($"PlayFab Login OK: {r.PlayFabId}");
    }

    void OnError(PlayFabError e)
    {
        Debug.LogError(e.GenerateErrorReport());
    }
}