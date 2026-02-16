using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button soloButton;
    [SerializeField] private Button multiplayerButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button quitButton;

    void Start()
    {
        soloButton.onClick.AddListener(OnSoloClicked);
        multiplayerButton.onClick.AddListener(OnMultiplayerClicked);
        continueButton.onClick.AddListener(OnContinueClicked);
        quitButton.onClick.AddListener(OnQuitClicked);

        if (SaveManager.Instance != null && SaveManager.Instance.HasSaveGame())
        {
            continueButton.interactable = true;
        }
        else
        {
            continueButton.interactable = false;
        }
    }

    void OnSoloClicked()
    {
        Debug.Log("Solo mode selected");
        GameManager.Instance.StartNewGame(GameMode.Solo);
    }

    void OnMultiplayerClicked()
    {
        Debug.Log("Multiplayer mode selected");
        GameManager.Instance.StartNewGame(GameMode.Multiplayer);
    }

    void OnContinueClicked()
    {
        Debug.Log("Continue game");
    }

    void OnQuitClicked()
    {
        Debug.Log("Quit game");
        Application.Quit();
    }
}
