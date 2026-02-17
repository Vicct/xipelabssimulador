using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TurnManager : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent<int> OnTurnStart;
    public UnityEvent<int> OnTurnEnd;
    public UnityEvent<PlayerState> OnPlayerTurnStart;
    public UnityEvent<PlayerState> OnPlayerTurnEnd;
    public UnityEvent OnTurnReady; // Fired when ready for player to click Next Turn

    [Header("References")]
    [SerializeField] private EconomyManager economyManager;
    [SerializeField] private EventManager eventManager;

    private GameState gameState;
    private bool isTurnInProgress;

    public bool IsTurnInProgress => isTurnInProgress;

    void Start()
    {
        gameState = GameManager.Instance.GameState;
        // Don't auto-start - wait for player to click Next Turn
        OnTurnReady?.Invoke();
    }

    public void StartNewTurn()
    {
        if (isTurnInProgress) return;

        StartCoroutine(TurnSequence());
    }

    IEnumerator TurnSequence()
    {
        isTurnInProgress = true;
        int turnNumber = gameState.currentTurn + 1;

        Debug.Log($"=== Turn {turnNumber} / {gameState.totalTurns} Start ===");
        OnTurnStart?.Invoke(turnNumber);

        for (int i = 0; i < gameState.players.Count; i++)
        {
            gameState.currentPlayerIndex = i;
            PlayerState currentPlayer = gameState.GetCurrentPlayer();

            yield return StartCoroutine(ProcessPlayerTurn(currentPlayer));
        }

        OnTurnEnd?.Invoke(turnNumber);
        Debug.Log($"=== Turn {turnNumber} End ===");

        isTurnInProgress = false;

        // Check if game is over
        GameManager.Instance.AdvanceTurn();

        // If game didn't end, signal ready for next turn
        if (!gameState.isGameOver)
        {
            OnTurnReady?.Invoke();
        }
    }

    IEnumerator ProcessPlayerTurn(PlayerState player)
    {
        Debug.Log($"Processing turn for {player.playerName}");
        OnPlayerTurnStart?.Invoke(player);

        // Pay salary
        gameState.currentPhase = GamePhase.EarningIncome;
        economyManager.PaySalary(player);
        yield return new WaitForSeconds(0.5f);

        // Random event
        gameState.currentPhase = GamePhase.RandomEvent;
        bool eventOccurred = eventManager.TriggerRandomEvent(player);

        if (eventOccurred)
        {
            // Wait for player to resolve event (close popup or make choice)
            yield return new WaitUntil(() => gameState.currentPhase != GamePhase.PlayerDecision
                                            && gameState.currentPhase != GamePhase.RandomEvent);
        }

        yield return new WaitForSeconds(0.3f);

        gameState.currentPhase = GamePhase.TurnEnd;
        OnPlayerTurnEnd?.Invoke(player);
    }
}
