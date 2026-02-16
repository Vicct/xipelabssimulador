using UnityEngine;
using UnityEngine.Events;

public class EconomyManager : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent<PlayerState, int> OnMoneyChanged;
    public UnityEvent<PlayerState, int> OnSalaryPaid;
    public UnityEvent<PlayerState, int> OnExpensePaid;

    private GameConfigData config;

    void Start()
    {
        config = GameManager.Instance.Config;
    }

    public void PaySalary(PlayerState player)
    {
        int salary = CalculateCurrentSalary(player);
        player.AddMoney(salary, $"Monthly Salary - {player.profession.ProfessionName}", TransactionType.Income);

        Debug.Log($"{player.playerName} earned ${salary}");
        OnSalaryPaid?.Invoke(player, salary);
        OnMoneyChanged?.Invoke(player, player.currentMoney);
    }

    int CalculateCurrentSalary(PlayerState player)
    {
        int baseSalary = player.profession.MonthlySalary;
        float growthMultiplier = 1 + (player.profession.SalaryGrowthRate * player.yearsWorked);
        int adjustedSalary = Mathf.RoundToInt(baseSalary * growthMultiplier);

        float afterTax = adjustedSalary * (1 - player.profession.TaxRate);

        return Mathf.RoundToInt(afterTax);
    }

    public bool ProcessExpense(PlayerState player, FinancialEventData eventData)
    {
        int expense = CalculateExpenseAmount(player, eventData);

        if (player.currentMoney < expense)
        {
            Debug.LogWarning($"{player.playerName} cannot afford ${expense} expense!");
        }

        player.AddMoney(-expense, eventData.EventName, TransactionType.Expense);

        OnExpensePaid?.Invoke(player, expense);
        OnMoneyChanged?.Invoke(player, player.currentMoney);

        return true;
    }

    public void ProcessIncome(PlayerState player, FinancialEventData eventData)
    {
        int income = eventData.BaseAmount;
        player.AddMoney(income, eventData.EventName, TransactionType.Bonus);

        OnMoneyChanged?.Invoke(player, player.currentMoney);
    }

    int CalculateExpenseAmount(PlayerState player, FinancialEventData eventData)
    {
        int baseAmount = Mathf.Abs(eventData.BaseAmount);

        if (!eventData.AffectedByProfessionModifiers)
            return baseAmount;

        float modifier = 1f;

        if (eventData.EventType == EventType.Emergency && player.profession.HasHealthInsurance)
        {
            modifier = player.profession.MedicalExpenseModifier;
        }

        return Mathf.RoundToInt(baseAmount * modifier);
    }

    public void ProcessPlayerChoice(PlayerState player, EventChoice choice)
    {
        int amount = choice.financialImpact;
        TransactionType type = amount >= 0 ? TransactionType.Income : TransactionType.Expense;

        player.AddMoney(amount, choice.choiceText, type);
        OnMoneyChanged?.Invoke(player, player.currentMoney);
    }
}
