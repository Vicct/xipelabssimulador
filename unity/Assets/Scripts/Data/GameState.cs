using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameState
{
    public GameMode gameMode;
    public GamePhase currentPhase;
    public int currentTurn;
    public int totalTurns;
    public List<PlayerState> players;
    public int currentPlayerIndex;
    public bool isGameOver;

    public string sessionId;
    public float gameStartTime;
    public Dictionary<int, List<FinancialEventData>> turnEvents;

    public GameState()
    {
        players = new List<PlayerState>();
        turnEvents = new Dictionary<int, List<FinancialEventData>>();
        currentPhase = GamePhase.Setup;
        currentTurn = 0;
        isGameOver = false;
    }

    public PlayerState GetCurrentPlayer()
    {
        if (players.Count == 0 || currentPlayerIndex >= players.Count)
            return null;
        return players[currentPlayerIndex];
    }

    public void AddPlayer(PlayerState player)
    {
        players.Add(player);
    }

    public bool AllPlayersReady()
    {
        foreach (var player in players)
        {
            if (!player.isReady) return false;
        }
        return players.Count > 0;
    }

    public PlayerState GetWinner()
    {
        if (players.Count == 0) return null;

        PlayerState winner = players[0];
        foreach (var player in players)
        {
            if (player.GetNetWorth() > winner.GetNetWorth())
            {
                winner = player;
            }
        }
        return winner;
    }
}

public enum GameMode
{
    Solo,
    Multiplayer
}

public enum GamePhase
{
    Setup,
    RoleSelection,
    Playing,
    TurnStart,
    EarningIncome,
    RandomEvent,
    PlayerDecision,
    TurnEnd,
    GameOver
}
