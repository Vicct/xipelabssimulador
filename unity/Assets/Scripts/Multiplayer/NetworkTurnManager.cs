using Photon.Pun;
using UnityEngine;

/// <summary>
/// Handles multiplayer turn synchronization via Photon RPCs.
/// Sits alongside TurnManager on the GameManager GameObject.
/// Requires a PhotonView component.
/// </summary>
[RequireComponent(typeof(PhotonView))]
public class NetworkTurnManager : MonoBehaviourPun
{
    [Header("References")]
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private EventManager eventManager;

    private GameState gameState;

    void Start()
    {
        gameState = GameManager.Instance.GameState;

        if (gameState.gameMode != GameMode.Multiplayer)
        {
            enabled = false;
            return;
        }

        Debug.Log("[NetworkTurnManager] Active for multiplayer game");
    }

    // --- Turn Control (Master Client initiates) ---

    /// <summary>
    /// Called by GameBoardController when Master Client clicks Next Turn.
    /// Broadcasts to all clients to start the turn.
    /// </summary>
    public void MasterStartTurn()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        photonView.RPC(nameof(RPC_StartTurn), RpcTarget.All, gameState.currentTurn);
    }

    [PunRPC]
    void RPC_StartTurn(int turnNumber)
    {
        Debug.Log($"[Network] Starting turn {turnNumber + 1}");
        turnManager.StartNewTurn();
    }

    // --- Event Synchronization (Master Client generates, broadcasts) ---

    /// <summary>
    /// Master Client calls this to broadcast an event to all clients.
    /// </summary>
    public void MasterBroadcastEvent(string eventAssetName, int playerIndex)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        Debug.Log($"[Network] Broadcasting event '{eventAssetName}' for player {playerIndex}");
        photonView.RPC(nameof(RPC_TriggerEvent), RpcTarget.All, eventAssetName, playerIndex);
    }

    [PunRPC]
    void RPC_TriggerEvent(string eventAssetName, int playerIndex)
    {
        FinancialEventData eventData = FindEventByName(eventAssetName);
        if (eventData == null)
        {
            Debug.LogError($"[Network] Event not found: {eventAssetName}");
            return;
        }

        if (playerIndex < 0 || playerIndex >= gameState.players.Count)
        {
            Debug.LogError($"[Network] Invalid player index: {playerIndex}");
            return;
        }

        PlayerState player = gameState.players[playerIndex];
        Debug.Log($"[Network] Triggering event '{eventData.EventName}' for {player.playerName}");
        eventManager.TriggerSpecificEvent(eventData, player);
    }

    /// <summary>
    /// Master Client broadcasts that no event occurred (roll failed).
    /// </summary>
    public void MasterBroadcastNoEvent(int playerIndex)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        photonView.RPC(nameof(RPC_NoEvent), RpcTarget.All, playerIndex);
    }

    [PunRPC]
    void RPC_NoEvent(int playerIndex)
    {
        Debug.Log($"[Network] No event for player {playerIndex}");
        // Phase stays at RandomEvent, TurnManager coroutine continues
    }

    // --- Money Synchronization ---

    /// <summary>
    /// Broadcasts a player's current money state to all other clients.
    /// Called after salary payment or event resolution.
    /// </summary>
    public void BroadcastMoneyUpdate(int playerIndex)
    {
        if (playerIndex < 0 || playerIndex >= gameState.players.Count) return;

        PlayerState p = gameState.players[playerIndex];
        photonView.RPC(nameof(RPC_UpdateMoney), RpcTarget.Others,
            playerIndex, p.currentMoney, p.totalEarned, p.totalSpent);
    }

    [PunRPC]
    void RPC_UpdateMoney(int playerIndex, int currentMoney, int totalEarned, int totalSpent)
    {
        if (playerIndex < 0 || playerIndex >= gameState.players.Count) return;

        PlayerState p = gameState.players[playerIndex];
        p.currentMoney = currentMoney;
        p.totalEarned = totalEarned;
        p.totalSpent = totalSpent;
    }

    // --- Event Resolution (player made a choice or closed popup) ---

    /// <summary>
    /// Broadcasts that a player resolved their event with a specific choice.
    /// </summary>
    public void BroadcastChoiceResolution(int playerIndex, int choiceIndex)
    {
        photonView.RPC(nameof(RPC_ResolveChoice), RpcTarget.Others, playerIndex, choiceIndex);
    }

    [PunRPC]
    void RPC_ResolveChoice(int playerIndex, int choiceIndex)
    {
        // Other clients see the same resolution
        Debug.Log($"[Network] Player {playerIndex} chose option {choiceIndex}");
    }

    // --- Game End ---

    /// <summary>
    /// Master Client broadcasts game over to all clients.
    /// </summary>
    public void MasterEndGame()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        photonView.RPC(nameof(RPC_EndGame), RpcTarget.All);
    }

    [PunRPC]
    void RPC_EndGame()
    {
        Debug.Log("[Network] Game Over received");
        gameState.currentPhase = GamePhase.GameOver;
        gameState.isGameOver = true;
    }

    // --- Helpers ---

    FinancialEventData FindEventByName(string assetName)
    {
        foreach (var e in GameManager.Instance.Config.AvailableEvents)
        {
            if (e.name == assetName) return e;
        }
        return null;
    }
}
