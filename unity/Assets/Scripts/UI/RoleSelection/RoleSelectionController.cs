using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using ExitGames.Client.Photon;

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
    private bool hasConfirmed;

    void Start()
    {
        roleCards = new List<RoleCard>();
        confirmButton.onClick.AddListener(OnConfirmSelection);
        confirmButton.interactable = false;
        hasConfirmed = false;

        SpawnRoleCards();

        // In multiplayer, listen for other players confirming
        if (GameManager.Instance.GameState.gameMode == GameMode.Multiplayer)
        {
            PhotonManager.Instance.OnPlayerPropertiesChanged.AddListener(OnPlayerPropsChanged);

            if (instructionText != null)
            {
                instructionText.text = "Choose your profession (all players must choose)";
            }
        }
    }

    void OnDestroy()
    {
        if (PhotonManager.Instance != null)
        {
            PhotonManager.Instance.OnPlayerPropertiesChanged.RemoveListener(OnPlayerPropsChanged);
        }
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
        if (hasConfirmed) return;

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
        if (selectedProfession == null || hasConfirmed) return;

        GameState gameState = GameManager.Instance.GameState;

        if (gameState.gameMode == GameMode.Solo)
        {
            // Solo mode: unchanged
            PlayerState localPlayer = gameState.players.Find(p => p.isLocalPlayer);

            if (localPlayer != null)
            {
                localPlayer.InitializeWithProfession(
                    selectedProfession,
                    gameConfig.StartingCash
                );
                localPlayer.isReady = true;

                Debug.Log($"Player ready with {selectedProfession.ProfessionName}");
                GameManager.Instance.OnAllPlayersReady();
            }
        }
        else
        {
            // Multiplayer: sync via Photon custom properties
            hasConfirmed = true;
            confirmButton.interactable = false;

            PhotonManager.Instance.SetPlayerProperty("selectedProfession", selectedProfession.name);
            PhotonManager.Instance.SetPlayerProperty("professionReady", true);

            if (instructionText != null)
            {
                instructionText.text = $"Selected: {selectedProfession.ProfessionName} - Waiting for other players...";
            }

            Debug.Log($"[Multiplayer] Profession selected: {selectedProfession.ProfessionName}");

            // Check if we're the last one
            CheckAllProfessionsSelected();
        }
    }

    void OnPlayerPropsChanged(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("professionReady"))
        {
            CheckAllProfessionsSelected();
        }
    }

    void CheckAllProfessionsSelected()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        foreach (var p in PhotonNetwork.PlayerList)
        {
            if (!p.CustomProperties.ContainsKey("professionReady")
                || !(bool)p.CustomProperties["professionReady"])
            {
                return;
            }
        }

        // All players have selected -- Master Client initializes everyone
        Debug.Log("[Multiplayer] All players have selected professions!");
        InitializeAllPlayersFromProperties();
        GameManager.Instance.OnAllPlayersReady();
    }

    void InitializeAllPlayersFromProperties()
    {
        GameState gs = GameManager.Instance.GameState;

        foreach (var photonPlayer in PhotonNetwork.PlayerList)
        {
            string profName = (string)photonPlayer.CustomProperties["selectedProfession"];
            ProfessionData prof = FindProfessionByName(profName);
            string playerId = photonPlayer.UserId ?? photonPlayer.ActorNumber.ToString();
            PlayerState ps = gs.players.Find(p => p.playerId == playerId);

            if (ps != null && prof != null)
            {
                ps.InitializeWithProfession(prof, gameConfig.StartingCash);
                ps.isReady = true;
                Debug.Log($"[Multiplayer] {ps.playerName} initialized with {prof.ProfessionName}");
            }
        }
    }

    ProfessionData FindProfessionByName(string assetName)
    {
        foreach (var prof in gameConfig.AvailableProfessions)
        {
            if (prof.name == assetName) return prof;
        }
        return null;
    }
}
