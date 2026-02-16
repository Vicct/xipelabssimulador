using UnityEngine;

[CreateAssetMenu(fileName = "New Profession", menuName = "Finance Game/Profession")]
public class ProfessionData : ScriptableObject
{
    [Header("Basic Info")]
    [SerializeField] private string professionName;
    [SerializeField] private string description;
    [SerializeField] private Sprite icon;

    [Header("Financial Profile")]
    [SerializeField] private int monthlySalary;
    [SerializeField] private int startingBonus;
    [SerializeField] private float salaryGrowthRate;

    [Header("Characteristics")]
    [SerializeField] private ProfessionTier tier;
    [SerializeField] private bool hasHealthInsurance;
    [SerializeField] private bool hasRetirementPlan;
    [SerializeField] private int educationCostPaid;

    [Header("Event Modifiers")]
    [SerializeField] private float medicalExpenseModifier = 1.0f;
    [SerializeField] private float taxRate;

    public string ProfessionName => professionName;
    public string Description => description;
    public Sprite Icon => icon;
    public int MonthlySalary => monthlySalary;
    public int StartingBonus => startingBonus;
    public float SalaryGrowthRate => salaryGrowthRate;
    public ProfessionTier Tier => tier;
    public bool HasHealthInsurance => hasHealthInsurance;
    public bool HasRetirementPlan => hasRetirementPlan;
    public int EducationCostPaid => educationCostPaid;
    public float MedicalExpenseModifier => medicalExpenseModifier;
    public float TaxRate => taxRate;
}

public enum ProfessionTier
{
    Low,
    Medium,
    High
}
