using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Modal popup shown when a player disconnects from the Photon network mid-game.
/// Offers a "Return to Main Menu" button.
/// </summary>
public class DisconnectDialog : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject dialogPanel;
    [SerializeField] private Text messageText;
    [SerializeField] private Button returnToMenuButton;

    private static DisconnectDialog instance;
    public static DisconnectDialog Instance => instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        if (dialogPanel != null)
        {
            dialogPanel.SetActive(false);
        }
    }

    void Start()
    {
        if (returnToMenuButton != null)
        {
            returnToMenuButton.onClick.AddListener(OnReturnToMenu);
        }

        // Subscribe to Photon disconnect events
        if (PhotonManager.Instance != null)
        {
            PhotonManager.Instance.OnDisconnectedFromPhoton.AddListener(OnDisconnected);
            PhotonManager.Instance.OnPlayerLeftRoom.AddListener(OnOtherPlayerLeft);
        }
    }

    void OnDestroy()
    {
        if (PhotonManager.Instance != null)
        {
            PhotonManager.Instance.OnDisconnectedFromPhoton.RemoveListener(OnDisconnected);
            PhotonManager.Instance.OnPlayerLeftRoom.RemoveListener(OnOtherPlayerLeft);
        }
    }

    void OnDisconnected()
    {
        GameState gs = GameManager.Instance?.GameState;
        if (gs == null) return;

        // Only show disconnect dialog during active multiplayer game
        if (gs.gameMode == GameMode.Multiplayer
            && gs.currentPhase != GamePhase.Setup
            && gs.currentPhase != GamePhase.GameOver)
        {
            Show("You have been disconnected from the server.");
        }
    }

    void OnOtherPlayerLeft(Photon.Realtime.Player otherPlayer)
    {
        GameState gs = GameManager.Instance?.GameState;
        if (gs == null) return;

        // Mark the player as disconnected in GameState
        if (gs.currentPhase == GamePhase.Playing
            || gs.currentPhase == GamePhase.TurnStart
            || gs.currentPhase == GamePhase.EarningIncome
            || gs.currentPhase == GamePhase.RandomEvent
            || gs.currentPhase == GamePhase.PlayerDecision
            || gs.currentPhase == GamePhase.TurnEnd)
        {
            string id = otherPlayer.UserId ?? otherPlayer.ActorNumber.ToString();
            PlayerState ps = gs.players.Find(p => p.playerId == id);
            if (ps != null)
            {
                ps.isDisconnected = true;
                Debug.Log($"[Disconnect] Player {ps.playerName} marked as disconnected");
            }

            // Show notification but don't block gameplay
            if (messageText != null)
            {
                messageText.text = $"{otherPlayer.NickName} has disconnected. Their turns will be skipped.";
            }
            if (dialogPanel != null)
            {
                dialogPanel.SetActive(true);
                // Auto-hide after 3 seconds
                Invoke(nameof(HideDialog), 3f);
            }
        }
    }

    public void Show(string message)
    {
        if (messageText != null)
        {
            messageText.text = message;
        }
        if (dialogPanel != null)
        {
            dialogPanel.SetActive(true);
        }
        if (returnToMenuButton != null)
        {
            returnToMenuButton.gameObject.SetActive(true);
        }
    }

    void HideDialog()
    {
        if (dialogPanel != null)
        {
            dialogPanel.SetActive(false);
        }
    }

    void OnReturnToMenu()
    {
        HideDialog();
        GameManager.Instance.ReturnToMainMenu();
    }
}
