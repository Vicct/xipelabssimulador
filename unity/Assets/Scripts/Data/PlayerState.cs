using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerState
{
    public string playerId;
    public string playerName;
    public ProfessionData profession;
    public int currentMoney;
    public int totalEarned;
    public int totalSpent;
    public bool isReady;
    public bool isLocalPlayer;

    public List<TransactionRecord> transactionHistory;
    public Dictionary<string, int> eventEncounters;

    public int yearsWorked;
    public int currentSalary;

    public PlayerState(string id, string name, bool isLocal)
    {
        playerId = id;
        playerName = name;
        isLocalPlayer = isLocal;
        transactionHistory = new List<TransactionRecord>();
        eventEncounters = new Dictionary<string, int>();
        isReady = false;
        yearsWorked = 0;
    }

    public void InitializeWithProfession(ProfessionData prof, int startCash)
    {
        profession = prof;
        currentMoney = startCash + prof.StartingBonus;
        currentSalary = prof.MonthlySalary;
        totalEarned = prof.StartingBonus;
        totalSpent = prof.EducationCostPaid;

        if (prof.StartingBonus > 0)
        {
            AddTransaction("Starting Bonus", prof.StartingBonus, TransactionType.Income);
        }
        if (prof.EducationCostPaid > 0)
        {
            AddTransaction("Education Cost", -prof.EducationCostPaid, TransactionType.Expense);
        }
    }

    public void AddMoney(int amount, string reason, TransactionType type)
    {
        currentMoney += amount;
        if (amount > 0) totalEarned += amount;
        else totalSpent += Mathf.Abs(amount);

        AddTransaction(reason, amount, type);
    }

    public void AddTransaction(string description, int amount, TransactionType type)
    {
        transactionHistory.Add(new TransactionRecord
        {
            description = description,
            amount = amount,
            type = type,
            turnNumber = GameManager.Instance != null ? GameManager.Instance.CurrentTurn : 0,
            timestamp = DateTime.Now
        });
    }

    public int GetNetWorth()
    {
        return currentMoney;
    }

    public int GetTotalProfit()
    {
        return totalEarned - totalSpent;
    }
}

[System.Serializable]
public class TransactionRecord
{
    public string description;
    public int amount;
    public TransactionType type;
    public int turnNumber;
    public DateTime timestamp;
}

public enum TransactionType
{
    Income,
    Expense,
    Investment,
    Bonus,
    Penalty
}
