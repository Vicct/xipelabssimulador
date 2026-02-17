using UnityEngine;
using UnityEngine.UI;

public class EventPopup : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private Image eventIcon;
    [SerializeField] private Text eventNameText;
    [SerializeField] private Text eventDescriptionText;
    [SerializeField] private Text amountText;
    [SerializeField] private Button closeButton;
    [SerializeField] private Transform choiceContainer;
    [SerializeField] private Button choiceButtonPrefab;

    private FinancialEventData currentEvent;
    private PlayerState currentPlayer;

    void Start()
    {
        closeButton.onClick.AddListener(OnCloseClicked);
        popupPanel.SetActive(false);
    }

    public void Show(FinancialEventData eventData, PlayerState player)
    {
        currentEvent = eventData;
        currentPlayer = player;

        eventNameText.text = eventData.EventName;
        eventDescriptionText.text = eventData.Description;
        eventIcon.sprite = eventData.Icon;

        if (eventData.HasChoice)
        {
            amountText.text = "Choose an option:";
            amountText.color = new Color(1f, 0.85f, 0.3f, 1f); // Yellow
            closeButton.gameObject.SetActive(false);
            ClearChoices();
            CreateChoiceButtons(eventData.Choices);
        }
        else
        {
            int amount = eventData.BaseAmount;
            if (amount < 0)
            {
                amountText.text = $"Cost: ${Mathf.Abs(amount):N0}";
                amountText.color = new Color(1f, 0.3f, 0.3f, 1f); // Red
            }
            else
            {
                amountText.text = $"Gain: ${amount:N0}";
                amountText.color = new Color(0.3f, 1f, 0.5f, 1f); // Green
            }
            ClearChoices();
            closeButton.gameObject.SetActive(true);
        }

        popupPanel.SetActive(true);
    }

    void CreateChoiceButtons(EventChoice[] choices)
    {
        foreach (var choice in choices)
        {
            Button choiceBtn = Instantiate(choiceButtonPrefab, choiceContainer);
            Text btnText = choiceBtn.GetComponentInChildren<Text>();

            string impactLabel = "";
            if (choice.financialImpact > 0)
                impactLabel = $" (+${choice.financialImpact:N0})";
            else if (choice.financialImpact < 0)
                impactLabel = $" (-${Mathf.Abs(choice.financialImpact):N0})";

            btnText.text = $"{choice.choiceText}{impactLabel}";

            EventChoice selectedChoice = choice;
            choiceBtn.onClick.AddListener(() => OnChoiceSelected(selectedChoice));
        }
    }

    void ClearChoices()
    {
        foreach (Transform child in choiceContainer)
        {
            Destroy(child.gameObject);
        }
    }

    void OnChoiceSelected(EventChoice choice)
    {
        Debug.Log($"Choice selected: {choice.choiceText}");

        EventManager eventManager = FindObjectOfType<EventManager>();
        if (eventManager != null)
        {
            eventManager.ResolveEventWithChoice(choice, currentPlayer);
        }

        Hide();
    }

    void OnCloseClicked()
    {
        // Resolve the event (changes phase so turn can continue)
        EventManager eventManager = FindObjectOfType<EventManager>();
        if (eventManager != null)
        {
            eventManager.ResolveCurrentEvent();
        }

        Hide();
    }

    void Hide()
    {
        popupPanel.SetActive(false);
        ClearChoices();
    }
}
