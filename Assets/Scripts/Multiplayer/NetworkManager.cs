using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    private static NetworkManager instance;
    public static NetworkManager Instance => instance;

    [Header("Network Settings")]
    [SerializeField] private bool isOnline;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void CreateRoom(string roomName)
    {
        Debug.Log($"Creating room: {roomName}");
    }

    public void JoinRoom(string roomName)
    {
        Debug.Log($"Joining room: {roomName}");
    }

    public void SendTurnData(PlayerState player)
    {
        Debug.Log($"Sending turn data for {player.playerName}");
    }

    public void SyncGameState(GameState state)
    {
        Debug.Log("Syncing game state");
    }
}
