using UnityEngine;
using UnityEngine.SceneManagement;

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

    public void StartNewGame(GameMode mode)
    {
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

    public void OnAllPlayersReady()
    {
        if (!gameState.AllPlayersReady())
        {
            Debug.LogWarning("Not all players ready");
            return;
        }

        gameState.currentPhase = GamePhase.Playing;
        SceneManager.LoadScene("GameBoard");
    }

    public void AdvanceTurn()
    {
        gameState.currentTurn++;

        if (gameState.currentTurn >= gameState.totalTurns)
        {
            EndGame();
        }
        else
        {
            if (turnManager != null)
            {
                turnManager.StartNewTurn();
            }
        }
    }

    void EndGame()
    {
        gameState.currentPhase = GamePhase.GameOver;
        gameState.isGameOver = true;
        Debug.Log("Game Over!");
        SceneManager.LoadScene("GameSummary");
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        gameState = new GameState();
    }
}
