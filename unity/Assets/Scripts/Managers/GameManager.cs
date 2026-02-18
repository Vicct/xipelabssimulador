using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance => instance;

    [Header("Configuration")]
    [SerializeField] private GameConfigData gameConfig;

    [Header("State")]
    private GameState gameState;

    [Header("References")]
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private EconomyManager economyManager;
    [SerializeField] private EventManager eventManager;
    [SerializeField] private UIManager uiManager;

    public GameState GameState => gameState;
    public GameConfigData Config => gameConfig;
    public int CurrentTurn => gameState != null ? gameState.currentTurn : 0;

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

    void Start()
    {
        InitializeGame();
    }

    void InitializeGame()
    {
        gameState = new GameState();
        Debug.Log("GameManager initialized");
    }

    /// <summary>
    /// Starts a new solo game. For multiplayer, use PrepareMultiplayerGame() instead.
    /// </summary>
    public void StartNewGame(GameMode mode)
    {
        if (mode == GameMode.Multiplayer)
        {
            PrepareMultiplayerGame();
            return;
        }

        gameState = new GameState
        {
            gameMode = mode,
            totalTurns = gameConfig.TotalTurns,
            sessionId = System.Guid.NewGuid().ToString(),
            gameStartTime = Time.time
        };

        gameState.currentPhase = GamePhase.RoleSelection;

        PlayerState localPlayer = new PlayerState(
            SystemInfo.deviceUniqueIdentifier,
            "Player 1",
            true
        );
        gameState.AddPlayer(localPlayer);

        Debug.Log($"Starting new {mode} game");
        SceneManager.LoadScene("RoleSelection");
    }

    /// <summary>
    /// Initializes GameState for multiplayer but does NOT create players or load scenes.
    /// Players are added when joining a Photon room (in LobbyController).
    /// </summary>
    public void PrepareMultiplayerGame()
    {
        gameState = new GameState
        {
            gameMode = GameMode.Multiplayer,
            totalTurns = gameConfig.TotalTurns,
            sessionId = System.Guid.NewGuid().ToString(),
            gameStartTime = Time.time
        };
        gameState.currentPhase = GamePhase.Setup;
        Debug.Log("Multiplayer game prepared, waiting for lobby...");
    }

    /// <summary>
    /// Called by LobbyController when all players are ready. Master Client loads RoleSelection.
    /// </summary>
    public void StartMultiplayerGame()
    {
        gameState.currentPhase = GamePhase.RoleSelection;

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("RoleSelection");
        }

        Debug.Log("Multiplayer game starting - loading RoleSelection");
    }

    public void OnAllPlayersReady()
    {
        if (!gameState.AllPlayersReady())
        {
            Debug.LogWarning("Not all players ready");
            return;
        }

        gameState.currentPhase = GamePhase.Playing;

        if (gameState.gameMode == GameMode.Multiplayer)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel("GameBoard");
            }
        }
        else
        {
            SceneManager.LoadScene("GameBoard");
        }
    }

    public void AdvanceTurn()
    {
        gameState.currentTurn++;

        if (gameState.currentTurn >= gameState.totalTurns)
        {
            EndGame();
        }
        // TurnManager handles signaling OnTurnReady for the next turn
    }

    void EndGame()
    {
        gameState.currentPhase = GamePhase.GameOver;
        gameState.isGameOver = true;
        Debug.Log("Game Over!");

        if (gameState.gameMode == GameMode.Multiplayer)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel("GameSummary");
            }
        }
        else
        {
            SceneManager.LoadScene("GameSummary");
        }
    }

    public void ReturnToMainMenu()
    {
        if (PhotonManager.Instance != null && PhotonManager.Instance.IsConnected)
        {
            PhotonManager.Instance.Disconnect();
        }

        gameState = new GameState();
        SceneManager.LoadScene("MainMenu");
    }

    public void PlayAgain()
    {
        if (gameState.gameMode == GameMode.Multiplayer)
        {
            ReturnToMainMenu();
            return;
        }
        StartNewGame(GameMode.Solo);
    }
}
