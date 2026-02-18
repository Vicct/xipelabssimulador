using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button soloButton;
    [SerializeField] private Button multiplayerButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Text statusText;

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

        if (statusText != null)
        {
            statusText.text = "";
        }
    }

    void OnDestroy()
    {
        if (PhotonManager.Instance != null)
        {
            PhotonManager.Instance.OnConnectedToPhoton.RemoveListener(OnPhotonConnected);
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

        GameManager.Instance.PrepareMultiplayerGame();

        multiplayerButton.interactable = false;
        soloButton.interactable = false;

        if (statusText != null)
        {
            statusText.text = "Connecting to server...";
        }

        PhotonManager.Instance.OnConnectedToPhoton.AddListener(OnPhotonConnected);
        PhotonManager.Instance.ConnectToPhoton("Player_" + Random.Range(1000, 9999));
    }

    void OnPhotonConnected()
    {
        PhotonManager.Instance.OnConnectedToPhoton.RemoveListener(OnPhotonConnected);

        if (statusText != null)
        {
            statusText.text = "Connected! Loading lobby...";
        }

        SceneManager.LoadScene("Lobby");
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
