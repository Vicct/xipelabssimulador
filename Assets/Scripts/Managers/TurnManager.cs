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

    [Header("References")]
    [SerializeField] private EconomyManager economyManager;
    [SerializeField] private EventManager eventManager;

    private GameState gameState;
    private bool isTurnInProgress;

    void Start()
    {
        gameState = GameManager.Instance.GameState;
        StartNewTurn();
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

        Debug.Log($"=== Turn {turnNumber} Start ===");
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

        yield return new WaitForSeconds(1f);
        GameManager.Instance.AdvanceTurn();
    }

    IEnumerator ProcessPlayerTurn(PlayerState player)
    {
        Debug.Log($"Processing turn for {player.playerName}");
        OnPlayerTurnStart?.Invoke(player);

        gameState.currentPhase = GamePhase.EarningIncome;
        economyManager.PaySalary(player);
        yield return new WaitForSeconds(1f);

        gameState.currentPhase = GamePhase.RandomEvent;
        bool eventOccurred = eventManager.TriggerRandomEvent(player);

        if (eventOccurred)
        {
            yield return new WaitUntil(() => gameState.currentPhase != GamePhase.PlayerDecision);
        }

        yield return new WaitForSeconds(0.5f);

        gameState.currentPhase = GamePhase.TurnEnd;
        OnPlayerTurnEnd?.Invoke(player);

        yield return new WaitForSeconds(1f);
    }
}
