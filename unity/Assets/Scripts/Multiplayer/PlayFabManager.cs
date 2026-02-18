using PlayFab;
using PlayFab.ClientModels;
using PlayFab.MultiplayerModels;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Handles PlayFab authentication and matchmaking.
/// Players are matched via PlayFab, then join the same Photon room.
/// </summary>
public class PlayFabManager : MonoBehaviour
{
    private static PlayFabManager instance;
    public static PlayFabManager Instance => instance;

    [Header("Events")]
    public UnityEvent<string> OnLoginSuccess;
    public UnityEvent<string> OnLoginFailed;
    public UnityEvent<string> OnMatchFound;
    public UnityEvent<string> OnMatchmakingFailed;
    public UnityEvent<string> OnMatchmakingStatus;

    [Header("Settings")]
    [SerializeField] private string matchmakingQueue = "MoneyMattersQueue";
    [SerializeField] private float pollIntervalSeconds = 6f;

    private string playFabId;
    private string entityId;
    private string entityType;
    private string matchmakingTicketId;
    private bool isPolling;
    private bool isLoggedIn;

    public bool IsLoggedIn => isLoggedIn;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    // --- Authentication ---

    public void LoginWithDeviceId()
    {
        if (isLoggedIn)
        {
            OnLoginSuccess?.Invoke(playFabId);
            return;
        }

        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginResult, OnLoginError);
        Debug.Log("[PlayFab] Logging in with device ID...");
    }

    void OnLoginResult(LoginResult result)
    {
        playFabId = result.PlayFabId;
        entityId = result.EntityToken.Entity.Id;
        entityType = result.EntityToken.Entity.Type;
        isLoggedIn = true;
        Debug.Log($"[PlayFab] Login success: {playFabId}");
        OnLoginSuccess?.Invoke(playFabId);
    }

    void OnLoginError(PlayFabError error)
    {
        Debug.LogError($"[PlayFab] Login failed: {error.GenerateErrorReport()}");
        OnLoginFailed?.Invoke(error.ErrorMessage);
    }

    // --- Matchmaking ---

    public void StartMatchmaking()
    {
        if (!isLoggedIn)
        {
            Debug.LogError("[PlayFab] Must be logged in before matchmaking");
            return;
        }

        var request = new CreateMatchmakingTicketRequest
        {
            Creator = new MatchmakingPlayer
            {
                Entity = new PlayFab.MultiplayerModels.EntityKey
                {
                    Id = entityId,
                    Type = entityType
                },
                Attributes = new MatchmakingPlayerAttributes
                {
                    DataObject = new { skill = 1 }
                }
            },
            GiveUpAfterSeconds = 120,
            QueueName = matchmakingQueue
        };

        PlayFabMultiplayerAPI.CreateMatchmakingTicket(request, OnTicketCreated, OnMatchmakingError);
        OnMatchmakingStatus?.Invoke("Creating matchmaking ticket...");
        Debug.Log("[PlayFab] Creating matchmaking ticket...");
    }

    void OnTicketCreated(CreateMatchmakingTicketResult result)
    {
        matchmakingTicketId = result.TicketId;
        Debug.Log($"[PlayFab] Ticket created: {matchmakingTicketId}");
        OnMatchmakingStatus?.Invoke("Searching for players...");
        isPolling = true;
        InvokeRepeating(nameof(PollMatchmaking), pollIntervalSeconds, pollIntervalSeconds);
    }

    void PollMatchmaking()
    {
        if (!isPolling) return;

        var request = new GetMatchmakingTicketRequest
        {
            TicketId = matchmakingTicketId,
            QueueName = matchmakingQueue
        };
        PlayFabMultiplayerAPI.GetMatchmakingTicket(request, OnPollResult, OnMatchmakingError);
    }

    void OnPollResult(GetMatchmakingTicketResult result)
    {
        Debug.Log($"[PlayFab] Matchmaking status: {result.Status}");

        switch (result.Status)
        {
            case "Matched":
                StopPolling();
                OnMatchmakingStatus?.Invoke("Match found!");
                GetMatch(result.MatchId);
                break;

            case "Canceled":
                StopPolling();
                OnMatchmakingFailed?.Invoke("Matchmaking was canceled");
                break;

            case "Failed":
                StopPolling();
                OnMatchmakingFailed?.Invoke("Matchmaking failed");
                break;

            case "WaitingForMatch":
                OnMatchmakingStatus?.Invoke("Waiting for players...");
                break;

            case "WaitingForPlayers":
                OnMatchmakingStatus?.Invoke("Players found, waiting for confirmation...");
                break;
        }
    }

    void GetMatch(string matchId)
    {
        var request = new GetMatchRequest
        {
            MatchId = matchId,
            QueueName = matchmakingQueue
        };
        PlayFabMultiplayerAPI.GetMatch(request, OnMatchResult, OnMatchmakingError);
    }

    void OnMatchResult(GetMatchResult result)
    {
        // Use matchId as Photon room name so all matched players join the same room
        string roomName = "PF_" + result.MatchId;
        Debug.Log($"[PlayFab] Match found! Room: {roomName}, Players: {result.Members.Count}");
        OnMatchFound?.Invoke(roomName);
    }

    void OnMatchmakingError(PlayFabError error)
    {
        Debug.LogError($"[PlayFab] Matchmaking error: {error.GenerateErrorReport()}");
        StopPolling();
        OnMatchmakingFailed?.Invoke(error.ErrorMessage);
    }

    // --- Cancel ---

    public void CancelMatchmaking()
    {
        if (string.IsNullOrEmpty(matchmakingTicketId))
        {
            StopPolling();
            return;
        }

        isPolling = false;
        CancelInvoke(nameof(PollMatchmaking));

        var request = new CancelMatchmakingTicketRequest
        {
            TicketId = matchmakingTicketId,
            QueueName = matchmakingQueue
        };
        PlayFabMultiplayerAPI.CancelMatchmakingTicket(request,
            _ => Debug.Log("[PlayFab] Matchmaking canceled"),
            error => Debug.LogWarning($"[PlayFab] Cancel error: {error.ErrorMessage}")
        );

        matchmakingTicketId = null;
        OnMatchmakingStatus?.Invoke("Matchmaking canceled");
    }

    void StopPolling()
    {
        isPolling = false;
        CancelInvoke(nameof(PollMatchmaking));
    }
}
