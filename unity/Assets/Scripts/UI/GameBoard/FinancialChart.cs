using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class FinancialChart : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private RectTransform chartContainer;
    [SerializeField] private GameObject barPrefab;
    [SerializeField] private Text labelPrefab;

    [Header("Chart Settings")]
    [SerializeField] private int maxBars = 12;
    [SerializeField] private Color incomeColor = Color.green;
    [SerializeField] private Color expenseColor = Color.red;

    private List<GameObject> chartBars;

    void Awake()
    {
        chartBars = new List<GameObject>();
    }

    public void UpdateChart(List<TransactionRecord> transactions)
    {
        ClearChart();

        if (transactions == null || transactions.Count == 0)
            return;

        var turnGroups = transactions
            .GroupBy(t => t.turnNumber)
            .OrderBy(g => g.Key)
            .Take(maxBars)
            .ToList();

        float maxAmount = turnGroups.Max(g =>
            Mathf.Max(
                g.Where(t => t.amount > 0).Sum(t => t.amount),
                Mathf.Abs(g.Where(t => t.amount < 0).Sum(t => t.amount))
            )
        );

        if (maxAmount == 0) maxAmount = 1;

        float barWidth = chartContainer.rect.width / (maxBars + 1);
        float maxBarHeight = chartContainer.rect.height * 0.8f;

        int index = 0;
        foreach (var group in turnGroups)
        {
            int income = group.Where(t => t.amount > 0).Sum(t => t.amount);
            int expense = Mathf.Abs(group.Where(t => t.amount < 0).Sum(t => t.amount));

            float xPos = barWidth * (index + 1);

            if (income > 0)
            {
                CreateBar(xPos, income, maxAmount, maxBarHeight, incomeColor);
            }

            if (expense > 0)
            {
                CreateBar(xPos + barWidth * 0.3f, expense, maxAmount, maxBarHeight, expenseColor);
            }

            index++;
        }
    }

    void CreateBar(float xPos, int amount, float maxAmount, float maxHeight, Color color)
    {
        GameObject bar = Instantiate(barPrefab, chartContainer);
        RectTransform barRect = bar.GetComponent<RectTransform>();

        float height = (amount / maxAmount) * maxHeight;
        barRect.sizeDelta = new Vector2(20, height);
        barRect.anchoredPosition = new Vector2(xPos, 0);

        Image barImage = bar.GetComponent<Image>();
        if (barImage != null)
        {
            barImage.color = color;
        }

        chartBars.Add(bar);
    }

    void ClearChart()
    {
        foreach (var bar in chartBars)
        {
            Destroy(bar);
        }
        chartBars.Clear();
    }
}
