using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;
using ExitGames.Client.Photon;
using System.Collections.Generic;

/// <summary>
/// Manages Photon PUN2 connection, lobby, and room lifecycle.
/// Replaces the old NetworkManager stub. Sits on the GameManager GameObject (DontDestroyOnLoad).
/// </summary>
public class PhotonManager : MonoBehaviourPunCallbacks
{
    private static PhotonManager instance;
    public static PhotonManager Instance => instance;

    [Header("Events")]
    public UnityEvent OnConnectedToPhoton;
    public UnityEvent OnDisconnectedFromPhoton;
    public UnityEvent<List<RoomInfo>> OnRoomListUpdated;
    public UnityEvent OnJoinedPhotonRoom;
    public UnityEvent<Player> OnPlayerJoinedRoom;
    public UnityEvent<Player> OnPlayerLeftRoom;
    public UnityEvent<Player, Hashtable> OnPlayerPropertiesChanged;
    public UnityEvent<Player> OnMasterClientChanged;

    [Header("Settings")]
    [SerializeField] private byte maxPlayersPerRoom = 4;
    [SerializeField] private string gameVersion = "0.2";

    private List<RoomInfo> cachedRoomList = new List<RoomInfo>();

    public bool IsConnected => PhotonNetwork.IsConnected;
    public bool IsMasterClient => PhotonNetwork.IsMasterClient;
    public bool InRoom => PhotonNetwork.InRoom;
    public Player LocalPlayer => PhotonNetwork.LocalPlayer;
    public Player[] PlayerList => PhotonNetwork.PlayerList;
    public string RoomName => PhotonNetwork.InRoom ? PhotonNetwork.CurrentRoom.Name : "";

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    // --- Connection ---

    public void ConnectToPhoton(string playerName)
    {
        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("Already connected to Photon");
            OnConnectedToPhoton?.Invoke();
            return;
        }

        PhotonNetwork.NickName = playerName;
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log($"Connecting to Photon as '{playerName}'...");
    }

    public void Disconnect()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
    }

    // --- Room Management ---

    public void CreateRoom(string roomName)
    {
        RoomOptions options = new RoomOptions
        {
            MaxPlayers = maxPlayersPerRoom,
            IsVisible = true,
            IsOpen = true,
            CustomRoomProperties = new Hashtable
            {
                { "gameStarted", false }
            },
            CustomRoomPropertiesForLobby = new string[] { "gameStarted" }
        };
        PhotonNetwork.CreateRoom(roomName, options);
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public void JoinRandomRoom()
    {
        Hashtable expectedProps = new Hashtable { { "gameStarted", false } };
        PhotonNetwork.JoinRandomRoom(expectedProps, 0);
    }

    public void JoinOrCreateRoom(string roomName)
    {
        RoomOptions options = new RoomOptions
        {
            MaxPlayers = maxPlayersPerRoom,
            IsVisible = false,
            IsOpen = true,
            CustomRoomProperties = new Hashtable { { "gameStarted", false } }
        };
        PhotonNetwork.JoinOrCreateRoom(roomName, options, TypedLobby.Default);
    }

    public void LeaveRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    // --- Custom Properties Helpers ---

    public void SetPlayerProperty(string key, object value)
    {
        Hashtable props = new Hashtable { { key, value } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public void SetRoomProperty(string key, object value)
    {
        if (!PhotonNetwork.InRoom) return;
        Hashtable props = new Hashtable { { key, value } };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }

    public void CloseRoom()
    {
        if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            SetRoomProperty("gameStarted", true);
        }
    }

    // --- Photon Callbacks ---

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master Server");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Photon Lobby");
        OnConnectedToPhoton?.Invoke();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning($"Disconnected from Photon: {cause}");
        OnDisconnectedFromPhoton?.Invoke();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        UpdateCachedRoomList(roomList);
        OnRoomListUpdated?.Invoke(cachedRoomList);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined room: {PhotonNetwork.CurrentRoom.Name} ({PhotonNetwork.CurrentRoom.PlayerCount} players)");
        OnJoinedPhotonRoom?.Invoke();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Player joined: {newPlayer.NickName}");
        OnPlayerJoinedRoom?.Invoke(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"Player left: {otherPlayer.NickName}");
        OnPlayerLeftRoom?.Invoke(otherPlayer);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        OnPlayerPropertiesChanged?.Invoke(targetPlayer, changedProps);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("No random room available, creating one...");
        CreateRoom("Room_" + Random.Range(1000, 9999));
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Create room failed: {message}");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Join room failed: {message}");
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log($"Master client switched to: {newMasterClient.NickName}");
        OnMasterClientChanged?.Invoke(newMasterClient);
    }

    // --- Internal ---

    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            if (info.RemovedFromList)
            {
                cachedRoomList.RemoveAll(r => r.Name == info.Name);
            }
            else
            {
                int idx = cachedRoomList.FindIndex(r => r.Name == info.Name);
                if (idx >= 0)
                    cachedRoomList[idx] = info;
                else
                    cachedRoomList.Add(info);
            }
        }
    }
}
