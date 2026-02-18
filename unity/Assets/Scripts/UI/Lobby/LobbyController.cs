using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using ExitGames.Client.Photon;

/// <summary>
/// Controls the Lobby scene UI: room creation, room list, player list, ready system.
/// </summary>
public class LobbyController : MonoBehaviour
{
    [Header("Lobby Panel")]
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private InputField roomNameInput;
    [SerializeField] private Button createRoomButton;
    [SerializeField] private Button joinRandomButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Transform roomListContainer;
    [SerializeField] private GameObject roomListItemPrefab;

    [Header("Room Panel")]
    [SerializeField] private GameObject roomPanel;
    [SerializeField] private Text roomNameText;
    [SerializeField] private Transform playerListContainer;
    [SerializeField] private GameObject playerListItemPrefab;
    [SerializeField] private Button readyButton;
    [SerializeField] private Text readyButtonText;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Text startGameButtonText;
    [SerializeField] private Button leaveRoomButton;

    [Header("Matchmaking")]
    [SerializeField] private Button findMatchButton;
    [SerializeField] private Button cancelMatchButton;
    [SerializeField] private Text matchmakingStatusText;

    [Header("Status")]
    [SerializeField] private Text statusText;

    private Dictionary<string, GameObject> roomListItems = new Dictionary<string, GameObject>();
    private Dictionary<int, GameObject> playerListItems = new Dictionary<int, GameObject>();
    private bool isReady;

    void Start()
    {
        createRoomButton.onClick.AddListener(OnCreateRoom);
        joinRandomButton.onClick.AddListener(OnJoinRandom);
        backButton.onClick.AddListener(OnBack);
        readyButton.onClick.AddListener(OnReadyToggle);
        startGameButton.onClick.AddListener(OnStartGame);
        leaveRoomButton.onClick.AddListener(OnLeaveRoom);

        // PlayFab matchmaking buttons (optional, may be null)
        if (findMatchButton != null)
        {
            findMatchButton.onClick.AddListener(OnFindMatch);
        }
        if (cancelMatchButton != null)
        {
            cancelMatchButton.onClick.AddListener(OnCancelMatch);
            cancelMatchButton.gameObject.SetActive(false);
        }

        ShowLobbyPanel();
        SubscribeToPhotonEvents();
        SubscribeToPlayFabEvents();

        if (statusText != null)
        {
            statusText.text = "Connected to server";
        }
    }

    void OnDestroy()
    {
        UnsubscribeFromPhotonEvents();
        UnsubscribeFromPlayFabEvents();
    }

    void SubscribeToPhotonEvents()
    {
        PhotonManager pm = PhotonManager.Instance;
        if (pm == null) return;

        pm.OnRoomListUpdated.AddListener(RefreshRoomList);
        pm.OnJoinedPhotonRoom.AddListener(OnJoinedRoom);
        pm.OnPlayerJoinedRoom.AddListener(OnPlayerJoined);
        pm.OnPlayerLeftRoom.AddListener(OnPlayerLeft);
        pm.OnPlayerPropertiesChanged.AddListener(OnPlayerPropsChanged);
    }

    void UnsubscribeFromPhotonEvents()
    {
        PhotonManager pm = PhotonManager.Instance;
        if (pm == null) return;

        pm.OnRoomListUpdated.RemoveListener(RefreshRoomList);
        pm.OnJoinedPhotonRoom.RemoveListener(OnJoinedRoom);
        pm.OnPlayerJoinedRoom.RemoveListener(OnPlayerJoined);
        pm.OnPlayerLeftRoom.RemoveListener(OnPlayerLeft);
        pm.OnPlayerPropertiesChanged.RemoveListener(OnPlayerPropsChanged);
    }

    // --- Panel Management ---

    void ShowLobbyPanel()
    {
        lobbyPanel.SetActive(true);
        roomPanel.SetActive(false);
        isReady = false;
    }

    void ShowRoomPanel()
    {
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);
    }

    // --- Lobby Actions ---

    void OnCreateRoom()
    {
        string roomName = roomNameInput.text.Trim();
        if (string.IsNullOrEmpty(roomName))
        {
            roomName = "Room_" + Random.Range(1000, 9999);
        }

        if (statusText != null)
            statusText.text = $"Creating room '{roomName}'...";

        PhotonManager.Instance.CreateRoom(roomName);
    }

    void OnJoinRandom()
    {
        if (statusText != null)
            statusText.text = "Searching for a room...";

        PhotonManager.Instance.JoinRandomRoom();
    }

    void OnBack()
    {
        PhotonManager.Instance.Disconnect();
        GameManager.Instance.ReturnToMainMenu();
    }

    // --- Room List ---

    void RefreshRoomList(List<RoomInfo> roomList)
    {
        // Clear existing items
        foreach (var item in roomListItems.Values)
        {
            Destroy(item);
        }
        roomListItems.Clear();

        // Create new items
        foreach (RoomInfo info in roomList)
        {
            if (info.RemovedFromList || !info.IsOpen || !info.IsVisible) continue;

            // Skip rooms where game already started
            if (info.CustomProperties.ContainsKey("gameStarted")
                && (bool)info.CustomProperties["gameStarted"])
                continue;

            GameObject item = Instantiate(roomListItemPrefab, roomListContainer);
            RoomListItem roomItem = item.GetComponent<RoomListItem>();
            if (roomItem != null)
            {
                roomItem.Initialize(info);
            }
            roomListItems[info.Name] = item;
        }
    }

    // --- Room Callbacks ---

    void OnJoinedRoom()
    {
        ShowRoomPanel();
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        isReady = false;

        // Initialize GameState players from Photon room
        GameState gs = GameManager.Instance.GameState;
        gs.players.Clear();

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            bool isLocal = p.IsLocal;
            string id = p.UserId ?? p.ActorNumber.ToString();
            PlayerState ps = new PlayerState(id, p.NickName, isLocal);
            gs.AddPlayer(ps);
        }

        RefreshPlayerList();
        UpdateButtons();
    }

    void OnPlayerJoined(Player newPlayer)
    {
        string id = newPlayer.UserId ?? newPlayer.ActorNumber.ToString();
        PlayerState ps = new PlayerState(id, newPlayer.NickName, false);
        GameManager.Instance.GameState.AddPlayer(ps);

        RefreshPlayerList();
        UpdateButtons();

        if (statusText != null)
            statusText.text = $"{newPlayer.NickName} joined the room";
    }

    void OnPlayerLeft(Player otherPlayer)
    {
        string id = otherPlayer.UserId ?? otherPlayer.ActorNumber.ToString();
        GameManager.Instance.GameState.players.RemoveAll(p => p.playerId == id);

        RefreshPlayerList();
        UpdateButtons();

        if (statusText != null)
            statusText.text = $"{otherPlayer.NickName} left the room";
    }

    void OnPlayerPropsChanged(Player targetPlayer, Hashtable changedProps)
    {
        RefreshPlayerList();
        UpdateButtons();
    }

    // --- Player List ---

    void RefreshPlayerList()
    {
        // Clear existing
        foreach (var item in playerListItems.Values)
        {
            Destroy(item);
        }
        playerListItems.Clear();

        // Create new items
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            GameObject item = Instantiate(playerListItemPrefab, playerListContainer);
            PlayerListItem playerItem = item.GetComponent<PlayerListItem>();
            if (playerItem != null)
            {
                playerItem.Initialize(p);
            }
            playerListItems[p.ActorNumber] = item;
        }
    }

    // --- Ready / Start ---

    void OnReadyToggle()
    {
        isReady = !isReady;
        PhotonManager.Instance.SetPlayerProperty("isReady", isReady);
        readyButtonText.text = isReady ? "Not Ready" : "Ready";
        readyButton.GetComponent<Image>().color = isReady
            ? new Color(0.3f, 0.8f, 0.3f) // green
            : Color.white;
    }

    void OnStartGame()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (!AllPlayersReady()) return;

        PhotonManager.Instance.CloseRoom();
        GameManager.Instance.StartMultiplayerGame();
    }

    void OnLeaveRoom()
    {
        PhotonManager.Instance.LeaveRoom();
        ShowLobbyPanel();
        isReady = false;
    }

    bool AllPlayersReady()
    {
        if (PhotonNetwork.PlayerList.Length < 2) return false;

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (!p.CustomProperties.ContainsKey("isReady")) return false;
            if (!(bool)p.CustomProperties["isReady"]) return false;
        }
        return true;
    }

    void UpdateButtons()
    {
        // Start button: only master client, only when all ready
        bool isMaster = PhotonNetwork.IsMasterClient;
        startGameButton.gameObject.SetActive(isMaster);
        startGameButton.interactable = AllPlayersReady();

        if (isMaster)
        {
            int playerCount = PhotonNetwork.PlayerList.Length;
            startGameButtonText.text = AllPlayersReady()
                ? $"Start Game ({playerCount} players)"
                : $"Waiting ({playerCount} players)...";
        }
    }

    // --- PlayFab Matchmaking ---

    void SubscribeToPlayFabEvents()
    {
        if (PlayFabManager.Instance == null) return;

        PlayFabManager.Instance.OnMatchFound.AddListener(OnMatchFound);
        PlayFabManager.Instance.OnMatchmakingFailed.AddListener(OnMatchmakingFailed);
        PlayFabManager.Instance.OnMatchmakingStatus.AddListener(OnMatchmakingStatusUpdate);
    }

    void UnsubscribeFromPlayFabEvents()
    {
        if (PlayFabManager.Instance == null) return;

        PlayFabManager.Instance.OnMatchFound.RemoveListener(OnMatchFound);
        PlayFabManager.Instance.OnMatchmakingFailed.RemoveListener(OnMatchmakingFailed);
        PlayFabManager.Instance.OnMatchmakingStatus.RemoveListener(OnMatchmakingStatusUpdate);
    }

    void OnFindMatch()
    {
        if (findMatchButton != null)
            findMatchButton.gameObject.SetActive(false);
        if (cancelMatchButton != null)
            cancelMatchButton.gameObject.SetActive(true);
        if (matchmakingStatusText != null)
            matchmakingStatusText.text = "Connecting to PlayFab...";

        // Login first, then start matchmaking
        PlayFabManager.Instance.OnLoginSuccess.AddListener(OnPlayFabReady);
        PlayFabManager.Instance.LoginWithDeviceId();
    }

    void OnPlayFabReady(string pfId)
    {
        PlayFabManager.Instance.OnLoginSuccess.RemoveListener(OnPlayFabReady);
        PlayFabManager.Instance.StartMatchmaking();
    }

    void OnMatchFound(string roomName)
    {
        if (matchmakingStatusText != null)
            matchmakingStatusText.text = "Match found! Joining room...";
        if (cancelMatchButton != null)
            cancelMatchButton.gameObject.SetActive(false);

        // Join the Photon room that PlayFab assigned
        PhotonManager.Instance.JoinOrCreateRoom(roomName);
    }

    void OnMatchmakingFailed(string reason)
    {
        if (matchmakingStatusText != null)
            matchmakingStatusText.text = $"Matchmaking failed: {reason}";
        if (findMatchButton != null)
            findMatchButton.gameObject.SetActive(true);
        if (cancelMatchButton != null)
            cancelMatchButton.gameObject.SetActive(false);
    }

    void OnMatchmakingStatusUpdate(string status)
    {
        if (matchmakingStatusText != null)
            matchmakingStatusText.text = status;
    }

    void OnCancelMatch()
    {
        PlayFabManager.Instance.CancelMatchmaking();
        if (findMatchButton != null)
            findMatchButton.gameObject.SetActive(true);
        if (cancelMatchButton != null)
            cancelMatchButton.gameObject.SetActive(false);
        if (matchmakingStatusText != null)
            matchmakingStatusText.text = "";
    }
}
