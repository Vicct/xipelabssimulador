using UnityEngine;
using UnityEngine.UI;

public class DashboardPanel : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Text currentMoneyText;
    [SerializeField] private Text totalEarnedText;
    [SerializeField] private Text totalSpentText;
    [SerializeField] private Text professionText;
    [SerializeField] private Image professionIcon;
    [SerializeField] private FinancialChart incomeChart;

    private PlayerState playerState;

    public void Initialize(PlayerState player)
    {
        playerState = player;
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        if (playerState == null) return;

        currentMoneyText.text = FormatMoney(playerState.currentMoney);
        totalEarnedText.text = FormatMoney(playerState.totalEarned);
        totalSpentText.text = FormatMoney(playerState.totalSpent);

        if (playerState.profession != null)
        {
            professionText.text = playerState.profession.ProfessionName;
            professionIcon.sprite = playerState.profession.Icon;
        }

        if (incomeChart != null)
        {
            incomeChart.UpdateChart(playerState.transactionHistory);
        }
    }

    public void UpdateMoneyDisplay(int newAmount)
    {
        currentMoneyText.text = FormatMoney(newAmount);

        StartCoroutine(AnimateMoneyChange());
    }

    System.Collections.IEnumerator AnimateMoneyChange()
    {
        Vector3 originalScale = currentMoneyText.transform.localScale;
        currentMoneyText.transform.localScale = originalScale * 1.2f;

        yield return new WaitForSeconds(0.2f);

        currentMoneyText.transform.localScale = originalScale;
    }

    string FormatMoney(int amount)
    {
        if (amount >= 0)
            return $"${amount:N0}";
        else
            return $"-${Mathf.Abs(amount):N0}";
    }
}
