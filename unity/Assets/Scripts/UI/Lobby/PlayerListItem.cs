using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI prefab script for a single player entry in the room player list.
/// </summary>
public class PlayerListItem : MonoBehaviour
{
    [SerializeField] private Text playerNameText;
    [SerializeField] private Text readyStatusText;
    [SerializeField] private Image backgroundImage;

    [Header("Colors")]
    [SerializeField] private Color readyColor = new Color(0.3f, 0.8f, 0.3f, 0.3f);
    [SerializeField] private Color notReadyColor = new Color(0.8f, 0.3f, 0.3f, 0.3f);
    [SerializeField] private Color masterColor = new Color(1f, 0.85f, 0.3f, 0.3f);

    public void Initialize(Player player)
    {
        string displayName = player.NickName;
        if (player.IsMasterClient)
        {
            displayName += " (Host)";
        }
        if (player.IsLocal)
        {
            displayName += " (You)";
        }

        playerNameText.text = displayName;

        bool isReady = player.CustomProperties.ContainsKey("isReady")
                       && (bool)player.CustomProperties["isReady"];

        readyStatusText.text = isReady ? "READY" : "NOT READY";
        readyStatusText.color = isReady ? Color.green : Color.red;

        if (backgroundImage != null)
        {
            if (player.IsMasterClient)
                backgroundImage.color = masterColor;
            else
                backgroundImage.color = isReady ? readyColor : notReadyColor;
        }
    }
}
