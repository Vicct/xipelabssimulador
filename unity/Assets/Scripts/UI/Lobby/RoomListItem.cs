using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI prefab script for a single room entry in the lobby room list.
/// </summary>
public class RoomListItem : MonoBehaviour
{
    [SerializeField] private Text roomNameText;
    [SerializeField] private Text playerCountText;
    [SerializeField] private Button joinButton;

    private string roomName;

    public void Initialize(RoomInfo info)
    {
        roomName = info.Name;
        roomNameText.text = info.Name;
        playerCountText.text = $"{info.PlayerCount}/{info.MaxPlayers}";
        joinButton.interactable = info.IsOpen && info.PlayerCount < info.MaxPlayers;
        joinButton.onClick.AddListener(OnJoinClicked);
    }

    void OnJoinClicked()
    {
        PhotonManager.Instance.JoinRoom(roomName);
    }
}
