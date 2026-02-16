using UnityEngine;

[CreateAssetMenu(fileName = "New Event", menuName = "Finance Game/Financial Event")]
public class FinancialEventData : ScriptableObject
{
    [Header("Event Info")]
    [SerializeField] private string eventName;
    [SerializeField] private string description;
    [SerializeField] private Sprite icon;

    [Header("Financial Impact")]
    [SerializeField] private EventType eventType;
    [SerializeField] private int baseAmount;
    [SerializeField] private bool affectedByProfessionModifiers;

    [Header("Occurrence")]
    [SerializeField] private float probability = 0.1f;
    [SerializeField] private int minTurnToAppear = 1;
    [SerializeField] private bool canRepeat = true;

    [Header("Player Choice")]
    [SerializeField] private bool hasChoice;
    [SerializeField] private EventChoice[] choices;

    public string EventName => eventName;
    public string Description => description;
    public Sprite Icon => icon;
    public EventType EventType => eventType;
    public int BaseAmount => baseAmount;
    public bool AffectedByProfessionModifiers => affectedByProfessionModifiers;
    public float Probability => probability;
    public int MinTurnToAppear => minTurnToAppear;
    public bool CanRepeat => canRepeat;
    public bool HasChoice => hasChoice;
    public EventChoice[] Choices => choices;
}

[System.Serializable]
public class EventChoice
{
    public string choiceText;
    public int financialImpact;
    public string resultDescription;
}

public enum EventType
{
    Expense,
    Income,
    Investment,
    Emergency,
    Opportunity,
    Lifestyle
}
