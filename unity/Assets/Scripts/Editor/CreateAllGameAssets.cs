using UnityEngine;
using UnityEditor;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

/// <summary>
/// Editor script to create ALL ScriptableObject assets for Money Matters.
/// Run from Unity menu: Tools > Finance Game > Create All Game Assets
///
/// Creates: 10 professions, 20 financial events, and updates GameConfig.
/// Safe to re-run: skips assets that already exist.
/// </summary>
public class CreateAllGameAssets : Editor
{
    private const BindingFlags PRIVATE_FIELD = BindingFlags.NonPublic | BindingFlags.Instance;

    private static readonly string ProfessionsPath = "Assets/Data/Professions";
    private static readonly string EventsPath = "Assets/Data/Events";
    private static readonly string ConfigPath = "Assets/Data/Config";

    // ─────────────────────────────────────────────
    // MAIN ENTRY POINT
    // ─────────────────────────────────────────────

    [MenuItem("Tools/Finance Game/Create All Game Assets")]
    static void CreateAllAssets()
    {
        Debug.Log("═══════════════════════════════════════");
        Debug.Log("  Money Matters - Creating All Assets  ");
        Debug.Log("═══════════════════════════════════════");

        EnsureDirectories();

        // Create all professions
        var professions = CreateAllProfessions();
        Debug.Log($"Professions: {professions.Count} total");

        // Create all events
        var events = CreateAllEvents();
        Debug.Log($"Events: {events.Count} total");

        // Update GameConfig with all references
        UpdateGameConfig(professions, events);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("═══════════════════════════════════════");
        Debug.Log("  ALL ASSETS CREATED SUCCESSFULLY!     ");
        Debug.Log("═══════════════════════════════════════");
    }

    // ─────────────────────────────────────────────
    // DIRECTORIES
    // ─────────────────────────────────────────────

    static void EnsureDirectories()
    {
        string[] dirs = { ProfessionsPath, EventsPath, ConfigPath };
        foreach (var dir in dirs)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
                Debug.Log($"Created directory: {dir}");
            }
        }
    }

    // ─────────────────────────────────────────────
    // PROFESSIONS (10 total)
    // ─────────────────────────────────────────────

    static List<ProfessionData> CreateAllProfessions()
    {
        var professions = new List<ProfessionData>();

        professions.Add(CreateProfession("StreetSweeper", "Street Sweeper",
            "City worker keeping streets clean",
            2200, 0, 0.01f, ProfessionTier.Low, false, false, 0, 1.0f, 0.10f));

        professions.Add(CreateProfession("Cashier", "Cashier",
            "Retail worker handling transactions daily",
            2500, 500, 0.02f, ProfessionTier.Low, false, false, 0, 1.0f, 0.12f));

        professions.Add(CreateProfession("Chef", "Chef",
            "Culinary artist creating delicious dishes",
            3800, 500, 0.04f, ProfessionTier.Medium, false, false, 25000, 1.0f, 0.15f));

        professions.Add(CreateProfession("Teacher", "Teacher",
            "Educator shaping the next generation",
            4200, 1000, 0.025f, ProfessionTier.Medium, true, true, 40000, 0.5f, 0.16f));

        professions.Add(CreateProfession("PoliceOfficer", "Police Officer",
            "Law enforcement protecting the community",
            4800, 2000, 0.03f, ProfessionTier.Medium, true, true, 15000, 0.5f, 0.18f));

        professions.Add(CreateProfession("Engineer", "Engineer",
            "Technical expert designing innovative solutions",
            7500, 5000, 0.05f, ProfessionTier.High, true, true, 80000, 0.5f, 0.22f));

        professions.Add(CreateProfession("Programmer", "Programmer",
            "Software developer building digital solutions",
            8200, 8000, 0.06f, ProfessionTier.High, true, true, 50000, 0.5f, 0.20f));

        professions.Add(CreateProfession("Pilot", "Pilot",
            "Aviation professional flying commercial aircraft",
            9500, 7000, 0.035f, ProfessionTier.High, true, true, 100000, 0.5f, 0.24f));

        professions.Add(CreateProfession("Lawyer", "Lawyer",
            "Legal professional defending rights and justice",
            10500, 5000, 0.045f, ProfessionTier.High, true, true, 150000, 0.5f, 0.26f));

        professions.Add(CreateProfession("Doctor", "Doctor",
            "Medical professional saving lives",
            12000, 10000, 0.04f, ProfessionTier.High, true, true, 200000, 0.5f, 0.28f));

        return professions;
    }

    static ProfessionData CreateProfession(string fileName, string name, string desc,
        int salary, int bonus, float growth, ProfessionTier tier,
        bool insurance, bool retirement, int education, float medModifier, float tax)
    {
        string path = $"{ProfessionsPath}/{fileName}.asset";

        // If already exists, load and return it
        var existing = AssetDatabase.LoadAssetAtPath<ProfessionData>(path);
        if (existing != null)
        {
            Debug.Log($"  [SKIP] {name} already exists");
            return existing;
        }

        var asset = ScriptableObject.CreateInstance<ProfessionData>();
        var type = typeof(ProfessionData);

        type.GetField("professionName", PRIVATE_FIELD)?.SetValue(asset, name);
        type.GetField("description", PRIVATE_FIELD)?.SetValue(asset, desc);
        type.GetField("monthlySalary", PRIVATE_FIELD)?.SetValue(asset, salary);
        type.GetField("startingBonus", PRIVATE_FIELD)?.SetValue(asset, bonus);
        type.GetField("salaryGrowthRate", PRIVATE_FIELD)?.SetValue(asset, growth);
        type.GetField("tier", PRIVATE_FIELD)?.SetValue(asset, tier);
        type.GetField("hasHealthInsurance", PRIVATE_FIELD)?.SetValue(asset, insurance);
        type.GetField("hasRetirementPlan", PRIVATE_FIELD)?.SetValue(asset, retirement);
        type.GetField("educationCostPaid", PRIVATE_FIELD)?.SetValue(asset, education);
        type.GetField("medicalExpenseModifier", PRIVATE_FIELD)?.SetValue(asset, medModifier);
        type.GetField("taxRate", PRIVATE_FIELD)?.SetValue(asset, tax);

        AssetDatabase.CreateAsset(asset, path);
        Debug.Log($"  [NEW] {name} - ${salary}/mo, {tier} tier");
        return asset;
    }

    // ─────────────────────────────────────────────
    // EVENTS (20 total)
    // ─────────────────────────────────────────────

    static List<FinancialEventData> CreateAllEvents()
    {
        var events = new List<FinancialEventData>();

        // ── GASTOS SIMPLES (6) ──

        events.Add(CreateSimpleEvent("CarRepair", "Car Repair",
            "Your car broke down and needs repairs immediately",
            EventType.Expense, -1200, 0.15f, 1, true, false));

        events.Add(CreateSimpleEvent("ParkingTicket", "Parking Ticket",
            "You got a parking ticket downtown",
            EventType.Expense, -150, 0.18f, 1, true, false));

        events.Add(CreateSimpleEvent("HomeAppliance", "Home Appliance Breakdown",
            "Your washing machine stopped working and needs replacement",
            EventType.Expense, -800, 0.14f, 1, true, false));

        events.Add(CreateSimpleEvent("PetEmergency", "Pet Emergency",
            "Your pet needs urgent veterinary care",
            EventType.Emergency, -900, 0.10f, 1, true, false));

        events.Add(CreateSimpleEvent("SpeedingTicket", "Speeding Ticket",
            "You were caught speeding on the highway",
            EventType.Expense, -300, 0.12f, 1, true, false));

        events.Add(CreateSimpleEvent("PhoneReplacement", "Phone Replacement",
            "Your phone screen cracked and needs a new one",
            EventType.Expense, -800, 0.13f, 1, true, false));

        // ── EMERGENCIA CON MODIFICADOR (1) ──

        events.Add(CreateSimpleEvent("MedicalEmergency", "Medical Emergency",
            "You need emergency medical attention at the hospital",
            EventType.Emergency, -3500, 0.08f, 1, true, true));

        // ── INGRESOS (6) ──

        events.Add(CreateSimpleEvent("TaxRefund", "Tax Refund",
            "The government is returning part of your taxes",
            EventType.Income, 1800, 0.12f, 3, false, false));

        events.Add(CreateSimpleEvent("SurpriseBonus", "Surprise Bonus",
            "Your boss gave you a performance bonus!",
            EventType.Income, 2500, 0.10f, 2, true, false));

        events.Add(CreateSimpleEvent("FoundMoney", "Found Money",
            "You found money in an old jacket pocket!",
            EventType.Income, 200, 0.05f, 1, true, false));

        events.Add(CreateSimpleEvent("InsuranceClaim", "Insurance Claim",
            "Your insurance claim was approved",
            EventType.Income, 2200, 0.07f, 2, false, false));

        events.Add(CreateSimpleEvent("FreelanceProject", "Freelance Project",
            "A friend offered you a paid side project",
            EventType.Opportunity, 2800, 0.09f, 3, true, false));

        events.Add(CreateSimpleEvent("SideHustle", "Side Hustle",
            "Your weekend side business is paying off",
            EventType.Opportunity, 1500, 0.13f, 2, true, false));

        // ── EVENTOS CON DECISIONES (7) ──

        events.Add(CreateChoiceEvent("BirthdayParty", "Birthday Party",
            "It's your birthday! How will you celebrate?",
            EventType.Lifestyle, 0.15f, 1, true,
            new EventChoice[] {
                new EventChoice { choiceText = "Big Party", financialImpact = -800, resultDescription = "Epic party! Everyone had an amazing time." },
                new EventChoice { choiceText = "Small Gathering", financialImpact = -300, resultDescription = "Nice intimate celebration with close friends." },
                new EventChoice { choiceText = "Skip It", financialImpact = 0, resultDescription = "Saved money but friends were disappointed." }
            }));

        events.Add(CreateChoiceEvent("WeddingInvitation", "Wedding Invitation",
            "A close friend is getting married. What will you do?",
            EventType.Lifestyle, 0.12f, 1, true,
            new EventChoice[] {
                new EventChoice { choiceText = "Attend + Gift", financialImpact = -400, resultDescription = "Beautiful ceremony! Your friend was very grateful." },
                new EventChoice { choiceText = "Gift Only", financialImpact = -200, resultDescription = "Sent a nice gift. Friend understood you couldn't attend." },
                new EventChoice { choiceText = "Decline", financialImpact = 0, resultDescription = "Saved money but the friendship may suffer." }
            }));

        events.Add(CreateChoiceEvent("StockInvestment", "Stock Investment",
            "A hot stock tip from a reliable source. Invest?",
            EventType.Investment, 0.10f, 2, true,
            new EventChoice[] {
                new EventChoice { choiceText = "Invest $2,000", financialImpact = -2000, resultDescription = "Bold move! Time will tell if it pays off." },
                new EventChoice { choiceText = "Invest $1,000", financialImpact = -1000, resultDescription = "Conservative approach. A safe bet." },
                new EventChoice { choiceText = "Skip", financialImpact = 0, resultDescription = "Played it safe. No risk, no reward." }
            }));

        events.Add(CreateChoiceEvent("GymMembership", "Gym Membership",
            "New Year resolution! Time to get in shape?",
            EventType.Lifestyle, 0.14f, 1, true,
            new EventChoice[] {
                new EventChoice { choiceText = "Annual Plan", financialImpact = -600, resultDescription = "Great deal! Full year of fitness ahead." },
                new EventChoice { choiceText = "Monthly Trial", financialImpact = -60, resultDescription = "Smart! Try before you commit long-term." },
                new EventChoice { choiceText = "Skip", financialImpact = 0, resultDescription = "Saved money. Maybe next year!" }
            }));

        events.Add(CreateChoiceEvent("HolidayShopping", "Holiday Shopping",
            "The holidays are here! How much will you spend on gifts?",
            EventType.Lifestyle, 0.16f, 1, true,
            new EventChoice[] {
                new EventChoice { choiceText = "Big Spender", financialImpact = -1500, resultDescription = "Everyone loved their gifts! You're the favorite." },
                new EventChoice { choiceText = "Budget Shopper", financialImpact = -700, resultDescription = "Thoughtful gifts without breaking the bank." },
                new EventChoice { choiceText = "Handmade Gifts", financialImpact = -200, resultDescription = "Creative and personal. Most people appreciated the effort." }
            }));

        events.Add(CreateChoiceEvent("ComputerUpgrade", "Computer Upgrade",
            "Your computer is getting slow. Time for an upgrade?",
            EventType.Lifestyle, 0.11f, 2, true,
            new EventChoice[] {
                new EventChoice { choiceText = "High-End PC", financialImpact = -2500, resultDescription = "Beast machine! Lightning fast for years to come." },
                new EventChoice { choiceText = "Mid-Range", financialImpact = -1200, resultDescription = "Good balance of performance and value." },
                new EventChoice { choiceText = "Keep Old One", financialImpact = 0, resultDescription = "Saved money but productivity may suffer." }
            }));

        events.Add(CreateChoiceEvent("CharityDonation", "Charity Donation",
            "A local charity is asking for donations. Will you contribute?",
            EventType.Lifestyle, 0.13f, 1, true,
            new EventChoice[] {
                new EventChoice { choiceText = "Generous ($500)", financialImpact = -500, resultDescription = "Very generous! The charity is deeply grateful." },
                new EventChoice { choiceText = "Small Donation ($100)", financialImpact = -100, resultDescription = "Every bit helps. Thank you for contributing." },
                new EventChoice { choiceText = "Not Now", financialImpact = 0, resultDescription = "Maybe next time. No judgment." }
            }));

        return events;
    }

    static FinancialEventData CreateSimpleEvent(string fileName, string name, string desc,
        EventType eventType, int amount, float probability, int minTurn, bool canRepeat,
        bool affectedByModifiers)
    {
        string path = $"{EventsPath}/{fileName}.asset";

        var existing = AssetDatabase.LoadAssetAtPath<FinancialEventData>(path);
        if (existing != null)
        {
            Debug.Log($"  [SKIP] {name} already exists");
            return existing;
        }

        var asset = ScriptableObject.CreateInstance<FinancialEventData>();
        var type = typeof(FinancialEventData);

        type.GetField("eventName", PRIVATE_FIELD)?.SetValue(asset, name);
        type.GetField("description", PRIVATE_FIELD)?.SetValue(asset, desc);
        type.GetField("eventType", PRIVATE_FIELD)?.SetValue(asset, eventType);
        type.GetField("baseAmount", PRIVATE_FIELD)?.SetValue(asset, amount);
        type.GetField("probability", PRIVATE_FIELD)?.SetValue(asset, probability);
        type.GetField("minTurnToAppear", PRIVATE_FIELD)?.SetValue(asset, minTurn);
        type.GetField("canRepeat", PRIVATE_FIELD)?.SetValue(asset, canRepeat);
        type.GetField("hasChoice", PRIVATE_FIELD)?.SetValue(asset, false);
        type.GetField("affectedByProfessionModifiers", PRIVATE_FIELD)?.SetValue(asset, affectedByModifiers);

        AssetDatabase.CreateAsset(asset, path);
        string sign = amount >= 0 ? "+" : "";
        Debug.Log($"  [NEW] {name} ({eventType}) {sign}${amount}");
        return asset;
    }

    static FinancialEventData CreateChoiceEvent(string fileName, string name, string desc,
        EventType eventType, float probability, int minTurn, bool canRepeat,
        EventChoice[] choices)
    {
        string path = $"{EventsPath}/{fileName}.asset";

        var existing = AssetDatabase.LoadAssetAtPath<FinancialEventData>(path);
        if (existing != null)
        {
            Debug.Log($"  [SKIP] {name} already exists");
            return existing;
        }

        var asset = ScriptableObject.CreateInstance<FinancialEventData>();
        var type = typeof(FinancialEventData);

        type.GetField("eventName", PRIVATE_FIELD)?.SetValue(asset, name);
        type.GetField("description", PRIVATE_FIELD)?.SetValue(asset, desc);
        type.GetField("eventType", PRIVATE_FIELD)?.SetValue(asset, eventType);
        type.GetField("baseAmount", PRIVATE_FIELD)?.SetValue(asset, 0);
        type.GetField("probability", PRIVATE_FIELD)?.SetValue(asset, probability);
        type.GetField("minTurnToAppear", PRIVATE_FIELD)?.SetValue(asset, minTurn);
        type.GetField("canRepeat", PRIVATE_FIELD)?.SetValue(asset, canRepeat);
        type.GetField("hasChoice", PRIVATE_FIELD)?.SetValue(asset, true);
        type.GetField("choices", PRIVATE_FIELD)?.SetValue(asset, choices);
        type.GetField("affectedByProfessionModifiers", PRIVATE_FIELD)?.SetValue(asset, false);

        AssetDatabase.CreateAsset(asset, path);
        Debug.Log($"  [NEW] {name} (Choice) {choices.Length} options");
        return asset;
    }

    // ─────────────────────────────────────────────
    // UPDATE GAMECONFIG
    // ─────────────────────────────────────────────

    static void UpdateGameConfig(List<ProfessionData> professions, List<FinancialEventData> events)
    {
        string configAssetPath = $"{ConfigPath}/GameConfig.asset";
        var config = AssetDatabase.LoadAssetAtPath<GameConfigData>(configAssetPath);

        if (config == null)
        {
            config = ScriptableObject.CreateInstance<GameConfigData>();
            AssetDatabase.CreateAsset(config, configAssetPath);
            Debug.Log("  [NEW] GameConfig created");
        }

        var type = typeof(GameConfigData);

        // Set game rules
        type.GetField("totalTurns", PRIVATE_FIELD)?.SetValue(config, 12);
        type.GetField("startingCash", PRIVATE_FIELD)?.SetValue(config, 5000);
        type.GetField("eventChancePerTurn", PRIVATE_FIELD)?.SetValue(config, 0.7f);
        type.GetField("minPlayers", PRIVATE_FIELD)?.SetValue(config, 1);
        type.GetField("maxPlayers", PRIVATE_FIELD)?.SetValue(config, 4);
        type.GetField("inflationRate", PRIVATE_FIELD)?.SetValue(config, 0.02f);
        type.GetField("savingsInterestRate", PRIVATE_FIELD)?.SetValue(config, 3);
        type.GetField("eventsPerTurn", PRIVATE_FIELD)?.SetValue(config, 1);

        // Assign all professions and events
        type.GetField("availableProfessions", PRIVATE_FIELD)?.SetValue(config, professions.ToArray());
        type.GetField("availableEvents", PRIVATE_FIELD)?.SetValue(config, events.ToArray());

        EditorUtility.SetDirty(config);
        Debug.Log($"  [OK] GameConfig updated: {professions.Count} professions, {events.Count} events");
    }
}
