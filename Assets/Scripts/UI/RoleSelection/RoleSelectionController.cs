using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RoleSelectionController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Transform roleCardContainer;
    [SerializeField] private RoleCard roleCardPrefab;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Text instructionText;

    [Header("References")]
    [SerializeField] private GameConfigData gameConfig;

    private List<RoleCard> roleCards;
    private ProfessionData selectedProfession;

    void Start()
    {
        roleCards = new List<RoleCard>();
        confirmButton.onClick.AddListener(OnConfirmSelection);
        confirmButton.interactable = false;

        SpawnRoleCards();
    }

    void SpawnRoleCards()
    {
        foreach (var profession in gameConfig.AvailableProfessions)
        {
            RoleCard card = Instantiate(roleCardPrefab, roleCardContainer);
            card.Initialize(profession, OnRoleCardClicked);
            roleCards.Add(card);
        }
    }

    void OnRoleCardClicked(ProfessionData profession)
    {
        Debug.Log($"Selected profession: {profession.ProfessionName}");

        foreach (var card in roleCards)
        {
            card.SetSelected(false);
        }

        selectedProfession = profession;
        confirmButton.interactable = true;

        foreach (var card in roleCards)
        {
            if (card.Profession == profession)
            {
                card.SetSelected(true);
            }
        }
    }

    void OnConfirmSelection()
    {
        if (selectedProfession == null) return;

        GameState gameState = GameManager.Instance.GameState;
        PlayerState localPlayer = gameState.players.Find(p => p.isLocalPlayer);

        if (localPlayer != null)
        {
            localPlayer.InitializeWithProfession(
                selectedProfession,
                gameConfig.StartingCash
            );
            localPlayer.isReady = true;

            Debug.Log($"Player ready with {selectedProfession.ProfessionName}");

            if (gameState.gameMode == GameMode.Solo)
            {
                GameManager.Instance.OnAllPlayersReady();
            }
        }
    }
}
