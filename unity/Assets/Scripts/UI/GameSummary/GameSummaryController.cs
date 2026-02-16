using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GameSummaryController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Text winnerText;
    [SerializeField] private Transform rankingContainer;
    [SerializeField] private GameObject playerRankPrefab;
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button mainMenuButton;

    void Start()
    {
        playAgainButton.onClick.AddListener(OnPlayAgain);
        mainMenuButton.onClick.AddListener(OnMainMenu);

        DisplayResults();
    }

    void DisplayResults()
    {
        GameState gameState = GameManager.Instance.GameState;

        if (gameState == null || gameState.players.Count == 0)
        {
            Debug.LogError("No game state found!");
            return;
        }

        PlayerState winner = gameState.GetWinner();
        winnerText.text = $"Winner: {winner.playerName}\n${winner.GetNetWorth():N0}";

        var sortedPlayers = gameState.players.OrderByDescending(p => p.GetNetWorth()).ToList();

        for (int i = 0; i < sortedPlayers.Count; i++)
        {
            GameObject rankObj = Instantiate(playerRankPrefab, rankingContainer);
            Text rankText = rankObj.GetComponentInChildren<Text>();

            PlayerState player = sortedPlayers[i];
            rankText.text = $"{i + 1}. {player.playerName} - ${player.GetNetWorth():N0}";
        }
    }

    void OnPlayAgain()
    {
        GameManager.Instance.ReturnToMainMenu();
    }

    void OnMainMenu()
    {
        GameManager.Instance.ReturnToMainMenu();
    }
}
