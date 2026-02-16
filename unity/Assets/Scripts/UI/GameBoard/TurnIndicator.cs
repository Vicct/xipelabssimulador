using UnityEngine;
using UnityEngine.UI;

public class TurnIndicator : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Text turnText;
    [SerializeField] private Text phaseText;

    public void UpdateTurn(int turnNumber)
    {
        int totalTurns = GameManager.Instance.Config.TotalTurns;
        turnText.text = $"Turn {turnNumber} / {totalTurns}";
    }

    public void UpdatePhase(string phaseName)
    {
        if (phaseText != null)
        {
            phaseText.text = phaseName;
        }
    }
}
