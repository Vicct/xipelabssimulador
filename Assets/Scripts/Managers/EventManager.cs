using System.Collections.Generic;
using System.Linq;
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
        float roll = Random.value;
        if (roll > config.EventChancePerTurn)
        {
            Debug.Log("No event this turn");
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

    List<FinancialEventData> GetAvailableEvents(PlayerState player)
    {
        List<FinancialEventData> available = new List<FinancialEventData>();
        int currentTurn = gameState.currentTurn;

        foreach (var eventData in config.AvailableEvents)
        {
            if (currentTurn < eventData.MinTurnToAppear)
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

        OnEventTriggered?.Invoke(eventData, player);

        if (eventData.HasChoice)
        {
            gameState.currentPhase = GamePhase.PlayerDecision;
        }
        else
        {
            ResolveEventAutomatic(eventData, player);
        }
    }

    void ResolveEventAutomatic(FinancialEventData eventData, PlayerState player)
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
