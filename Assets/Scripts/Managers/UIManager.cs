using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private DashboardPanel dashboardPanel;
    [SerializeField] private EventPopup eventPopup;
    [SerializeField] private TurnIndicator turnIndicator;
    [SerializeField] private PlayerStatusPanel[] playerStatusPanels;

    [Header("References")]
    [SerializeField] private EconomyManager economyManager;
    [SerializeField] private EventManager eventManager;
    [SerializeField] private TurnManager turnManager;

    void Start()
    {
        RegisterEventListeners();
        InitializeUI();
    }

    void OnDestroy()
    {
        UnregisterEventListeners();
    }

    void RegisterEventListeners()
    {
        if (economyManager != null)
        {
            economyManager.OnMoneyChanged.AddListener(UpdatePlayerMoney);
            economyManager.OnSalaryPaid.AddListener(ShowSalaryNotification);
        }

        if (eventManager != null)
        {
            eventManager.OnEventTriggered.AddListener(ShowEventPopup);
        }

        if (turnManager != null)
        {
            turnManager.OnTurnStart.AddListener(UpdateTurnDisplay);
        }
    }

    void UnregisterEventListeners()
    {
        if (economyManager != null)
        {
            economyManager.OnMoneyChanged.RemoveListener(UpdatePlayerMoney);
            economyManager.OnSalaryPaid.RemoveListener(ShowSalaryNotification);
        }

        if (eventManager != null)
        {
            eventManager.OnEventTriggered.RemoveListener(ShowEventPopup);
        }

        if (turnManager != null)
        {
            turnManager.OnTurnStart.RemoveListener(UpdateTurnDisplay);
        }
    }

    void InitializeUI()
    {
        GameState gameState = GameManager.Instance.GameState;

        for (int i = 0; i < gameState.players.Count; i++)
        {
            if (i < playerStatusPanels.Length && playerStatusPanels[i] != null)
            {
                playerStatusPanels[i].Initialize(gameState.players[i]);
            }
        }

        if (dashboardPanel != null)
        {
            dashboardPanel.Initialize(gameState.players[0]);
        }
    }

    void UpdatePlayerMoney(PlayerState player, int newAmount)
    {
        if (dashboardPanel != null && player.isLocalPlayer)
        {
            dashboardPanel.UpdateMoneyDisplay(newAmount);
        }

        UpdatePlayerStatusPanel(player);
    }

    void UpdatePlayerStatusPanel(PlayerState player)
    {
        int playerIndex = GameManager.Instance.GameState.players.IndexOf(player);
        if (playerIndex >= 0 && playerIndex < playerStatusPanels.Length && playerStatusPanels[playerIndex] != null)
        {
            playerStatusPanels[playerIndex].UpdateDisplay(player);
        }
    }

    void ShowSalaryNotification(PlayerState player, int amount)
    {
        Debug.Log($"Show salary notification: ${amount}");
    }

    void ShowEventPopup(FinancialEventData eventData, PlayerState player)
    {
        if (eventPopup != null)
        {
            eventPopup.Show(eventData, player);
        }
    }

    void UpdateTurnDisplay(int turnNumber)
    {
        if (turnIndicator != null)
        {
            turnIndicator.UpdateTurn(turnNumber);
        }
    }
}
