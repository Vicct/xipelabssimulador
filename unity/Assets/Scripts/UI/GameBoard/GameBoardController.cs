using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the GameBoard scene buttons and connects them to managers.
/// Attach to Canvas or a root UI object in the GameBoard scene.
/// </summary>
public class GameBoardController : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button nextTurnButton;
    [SerializeField] private Button saveButton;

    [Header("References")]
    [SerializeField] private TurnManager turnManager;

    void Start()
    {
        nextTurnButton.onClick.AddListener(OnNextTurnClicked);

        if (saveButton != null)
        {
            saveButton.onClick.AddListener(OnSaveClicked);
        }

        // Listen for when a turn finishes to re-enable the button
        if (turnManager != null)
        {
            turnManager.OnTurnStart.AddListener(OnTurnStarted);
            turnManager.OnTurnReady.AddListener(OnTurnReady);
        }
    }

    void OnDestroy()
    {
        if (turnManager != null)
        {
            turnManager.OnTurnStart.RemoveListener(OnTurnStarted);
            turnManager.OnTurnReady.RemoveListener(OnTurnReady);
        }
    }

    void OnNextTurnClicked()
    {
        if (turnManager != null && !turnManager.IsTurnInProgress)
        {
            turnManager.StartNewTurn();
        }
    }

    void OnTurnStarted(int turnNumber)
    {
        // Disable button while turn is processing
        nextTurnButton.interactable = false;
        nextTurnButton.GetComponentInChildren<Text>().text = $"Turn {turnNumber}...";
    }

    void OnTurnReady()
    {
        // Re-enable button when turn is done
        nextTurnButton.interactable = true;
        int nextTurn = GameManager.Instance.CurrentTurn + 1;
        int totalTurns = GameManager.Instance.Config.TotalTurns;
        nextTurnButton.GetComponentInChildren<Text>().text = $"Next Turn ({nextTurn}/{totalTurns})";
    }

    void OnSaveClicked()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveGame(GameManager.Instance.GameState);
            Debug.Log("Game saved!");
        }
    }
}
