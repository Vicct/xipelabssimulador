using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Finance Game/Game Config")]
public class GameConfigData : ScriptableObject
{
    [Header("Game Rules")]
    [SerializeField] private int totalTurns = 12;
    [SerializeField] private int minPlayers = 1;
    [SerializeField] private int maxPlayers = 4;

    [Header("Economy Settings")]
    [SerializeField] private int startingCash = 5000;
    [SerializeField] private float inflationRate = 0.02f;
    [SerializeField] private int savingsInterestRate = 3;

    [Header("Event Settings")]
    [SerializeField] private int eventsPerTurn = 1;
    [SerializeField] private float eventChancePerTurn = 0.7f;

    [Header("References")]
    [SerializeField] private ProfessionData[] availableProfessions;
    [SerializeField] private FinancialEventData[] availableEvents;

    public int TotalTurns => totalTurns;
    public int MinPlayers => minPlayers;
    public int MaxPlayers => maxPlayers;
    public int StartingCash => startingCash;
    public float InflationRate => inflationRate;
    public int SavingsInterestRate => savingsInterestRate;
    public int EventsPerTurn => eventsPerTurn;
    public float EventChancePerTurn => eventChancePerTurn;
    public ProfessionData[] AvailableProfessions => availableProfessions;
    public FinancialEventData[] AvailableEvents => availableEvents;
}
