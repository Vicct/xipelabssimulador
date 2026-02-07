using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RoleCard : MonoBehaviour, IPointerClickHandler
{
    [Header("UI Elements")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Text nameText;
    [SerializeField] private Text salaryText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.yellow;

    private ProfessionData profession;
    private System.Action<ProfessionData> onClickCallback;
    private bool isSelected;

    public ProfessionData Profession => profession;

    public void Initialize(ProfessionData professionData, System.Action<ProfessionData> onClick)
    {
        profession = professionData;
        onClickCallback = onClick;

        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        if (profession == null) return;

        nameText.text = profession.ProfessionName;
        salaryText.text = $"${profession.MonthlySalary:N0}/month";
        descriptionText.text = profession.Description;
        iconImage.sprite = profession.Icon;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClickCallback?.Invoke(profession);
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        backgroundImage.color = selected ? selectedColor : normalColor;
    }
}
