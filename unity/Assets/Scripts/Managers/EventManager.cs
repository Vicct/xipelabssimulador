using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent<FinancialEventData, PlayerState> OnEventTriggered;
    public UnityEvent OnEventResolved;

    [Header("References")]
    [SerializeField] private EconomyManager economyManager;

    private GameConfigData config;
    private GameState gameState;
    private FinancialEventData currentEvent;

    void Start()
    {
        config = GameManager.Instance.Config;
        gameState = GameManager.Instance.GameState;
    }

    public bool TriggerRandomEvent(PlayerState player)
    {
        // In multiplayer, only Master Client generates events
        if (gameState.gameMode == GameMode.Multiplayer)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                // Non-master clients wait for RPC via NetworkTurnManager
                return false;
            }

            // Master Client generates the event and broadcasts
            return MasterGenerateAndBroadcastEvent(player);
        }

        // Solo mode: original logic
        float roll = Random.value;
        if (roll > config.EventChancePerTurn)
        {
            Debug.Log($"No event this turn (roll: {roll:F2} > chance: {config.EventChancePerTurn})");
            return false;
        }

        List<FinancialEventData> availableEvents = GetAvailableEvents(player);

        if (availableEvents.Count == 0)
        {
            Debug.Log("No available events");
            return false;
        }

        FinancialEventData selectedEvent = SelectWeightedEvent(availableEvents);

        if (selectedEvent == null)
            return false;

        TriggerEvent(selectedEvent, player);
        return true;
    }

    /// <summary>
    /// Master Client generates an event and broadcasts it via NetworkTurnManager RPC.
    /// The RPC calls TriggerSpecificEvent on all clients (including Master).
    /// </summary>
    bool MasterGenerateAndBroadcastEvent(PlayerState player)
    {
        float roll = Random.value;
        if (roll > config.EventChancePerTurn)
        {
            Debug.Log($"[Master] No event this turn (roll: {roll:F2})");
            NetworkTurnManager ntm = FindObjectOfType<NetworkTurnManager>();
            ntm?.MasterBroadcastNoEvent(gameState.currentPlayerIndex);
            return false;
        }

        List<FinancialEventData> availableEvents = GetAvailableEvents(player);
        if (availableEvents.Count == 0)
        {
            NetworkTurnManager ntm = FindObjectOfType<NetworkTurnManager>();
            ntm?.MasterBroadcastNoEvent(gameState.currentPlayerIndex);
            return false;
        }

        FinancialEventData selectedEvent = SelectWeightedEvent(availableEvents);
        if (selectedEvent == null)
        {
            return false;
        }

        // Broadcast event to all clients (including self)
        NetworkTurnManager ntm2 = FindObjectOfType<NetworkTurnManager>();
        ntm2?.MasterBroadcastEvent(selectedEvent.name, gameState.currentPlayerIndex);
        return true;
    }

    /// <summary>
    /// Called by NetworkTurnManager RPC on ALL clients (including Master).
    /// Triggers a specific event without random selection.
    /// </summary>
    public void TriggerSpecificEvent(FinancialEventData eventData, PlayerState player)
    {
        TriggerEvent(eventData, player);
    }

    List<FinancialEventData> GetAvailableEvents(PlayerState player)
    {
        List<FinancialEventData> available = new List<FinancialEventData>();
        // currentTurn is 0-based internally, but MinTurnToAppear is 1-based
        int displayTurn = gameState.currentTurn + 1;

        foreach (var eventData in config.AvailableEvents)
        {
            if (displayTurn < eventData.MinTurnToAppear)
                continue;

            if (!eventData.CanRepeat && player.eventEncounters.ContainsKey(eventData.name))
                continue;

            available.Add(eventData);
        }

        return available;
    }

    FinancialEventData SelectWeightedEvent(List<FinancialEventData> events)
    {
        float totalWeight = events.Sum(e => e.Probability);
        float roll = Random.value * totalWeight;
        float cumulative = 0;

        foreach (var eventData in events)
        {
            cumulative += eventData.Probability;
            if (roll <= cumulative)
                return eventData;
        }

        return events[events.Count - 1];
    }

    void TriggerEvent(FinancialEventData eventData, PlayerState player)
    {
        currentEvent = eventData;
        Debug.Log($"Event triggered: {eventData.EventName} for {player.playerName}");

        if (!player.eventEncounters.ContainsKey(eventData.name))
        {
            player.eventEncounters[eventData.name] = 0;
        }
        player.eventEncounters[eventData.name]++;

        // Always set phase to PlayerDecision so the turn waits for popup close
        gameState.currentPhase = GamePhase.PlayerDecision;

        // Process money immediately for non-choice events (popup just shows info)
        if (!eventData.HasChoice)
        {
            ResolveEventMoney(eventData, player);
        }

        // Show the popup (UIManager listens to this event)
        OnEventTriggered?.Invoke(eventData, player);
    }

    void ResolveEventMoney(FinancialEventData eventData, PlayerState player)
    {
        switch (eventData.EventType)
        {
            case EventType.Expense:
            case EventType.Emergency:
                economyManager.ProcessExpense(player, eventData);
                break;

            case EventType.Income:
            case EventType.Opportunity:
                economyManager.ProcessIncome(player, eventData);
                break;
        }
    }

    /// <summary>
    /// Called when player closes the event popup (for non-choice events)
    /// </summary>
    public void ResolveCurrentEvent()
    {
        OnEventResolved?.Invoke();
        gameState.currentPhase = GamePhase.TurnEnd;
    }

    public void ResolveEventWithChoice(EventChoice choice, PlayerState player)
    {
        Debug.Log($"Player chose: {choice.choiceText}");
        economyManager.ProcessPlayerChoice(player, choice);

        OnEventResolved?.Invoke();
        gameState.currentPhase = GamePhase.TurnEnd;
    }
}
