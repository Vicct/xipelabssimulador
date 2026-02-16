using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusPanel : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Text playerNameText;
    [SerializeField] private Text moneyText;
    [SerializeField] private Image professionIcon;
    [SerializeField] private GameObject turnIndicator;

    private PlayerState playerState;

    public void Initialize(PlayerState player)
    {
        playerState = player;
        UpdateDisplay(player);
    }

    public void UpdateDisplay(PlayerState player)
    {
        if (player == null) return;

        playerNameText.text = player.playerName;
        moneyText.text = $"${player.currentMoney:N0}";

        if (player.profession != null)
        {
            professionIcon.sprite = player.profession.Icon;
        }
    }

    public void SetTurnActive(bool active)
    {
        if (turnIndicator != null)
        {
            turnIndicator.SetActive(active);
        }
    }
}
