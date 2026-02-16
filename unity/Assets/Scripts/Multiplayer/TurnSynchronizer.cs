using UnityEngine;

public class TurnSynchronizer : MonoBehaviour
{
    public void RequestTurnStart(PlayerState player)
    {
        Debug.Log($"{player.playerName} requesting turn start");
    }

    public void NotifyTurnComplete(PlayerState player)
    {
        Debug.Log($"{player.playerName} completed turn");
    }
}
