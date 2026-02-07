using UnityEngine;

public static class MoneyFormatter
{
    public static string Format(int amount, bool showSign = false)
    {
        string formatted = $"${Mathf.Abs(amount):N0}";

        if (showSign)
        {
            if (amount >= 0)
                return $"+{formatted}";
            else
                return $"-{formatted}";
        }

        return amount >= 0 ? formatted : $"-{formatted}";
    }

    public static string FormatWithColor(int amount)
    {
        if (amount >= 0)
            return $"<color=green>+${amount:N0}</color>";
        else
            return $"<color=red>-${Mathf.Abs(amount):N0}</color>";
    }
}
