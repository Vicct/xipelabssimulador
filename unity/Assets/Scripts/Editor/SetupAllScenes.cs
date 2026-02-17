using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Reflection;
using System.Collections.Generic;

/// <summary>
/// Editor script to setup/verify all scenes and configure Build Settings.
/// Run from Unity menu: Tools > Finance Game > Setup All Scenes
///
/// What it does:
/// 1. Verifies/completes MainMenu scene UI and references
/// 2. Verifies/completes RoleSelection scene UI and references
/// 3. Builds complete GameBoard scene with all Managers and UI panels
/// 4. Configures Build Settings with correct scene order
/// </summary>
public class SetupAllScenes : Editor
{
    private const BindingFlags PRIVATE_FIELD = BindingFlags.NonPublic | BindingFlags.Instance;

    [MenuItem("Tools/Finance Game/Setup All Scenes")]
    static void SetupAll()
    {
        Debug.Log("═══════════════════════════════════════");
        Debug.Log("  Money Matters - Setting Up Scenes    ");
        Debug.Log("═══════════════════════════════════════");

        SetupGameBoardScene();
        SetupBuildSettings();

        Debug.Log("═══════════════════════════════════════");
        Debug.Log("  ALL SCENES CONFIGURED!               ");
        Debug.Log("═══════════════════════════════════════");
    }

    // ─────────────────────────────────────────────
    // GAMEBOARD SCENE (most complex)
    // ─────────────────────────────────────────────

    [MenuItem("Tools/Finance Game/Setup GameBoard Scene")]
    static void SetupGameBoardScene()
    {
        Debug.Log("\n── Setting up GameBoard Scene ──");

        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        // ── CANVAS ──
        GameObject canvasGO = CreateCanvas("Canvas");

        // ── BACKGROUND ──
        GameObject bg = CreatePanel("Background", canvasGO.transform, new Color(0.12f, 0.14f, 0.20f));
        SetFullStretch(bg.GetComponent<RectTransform>());

        // ── TOP BAR (Turn Indicator) ──
        GameObject topBar = CreatePanel("TopBar", canvasGO.transform, new Color(0.08f, 0.10f, 0.15f));
        RectTransform topBarRT = topBar.GetComponent<RectTransform>();
        topBarRT.anchorMin = new Vector2(0, 0.93f);
        topBarRT.anchorMax = new Vector2(1, 1);
        topBarRT.offsetMin = Vector2.zero;
        topBarRT.offsetMax = Vector2.zero;

        GameObject turnText = CreateText("TurnText", topBar.transform, "Turn 1 / 12", 28, Color.white, TextAnchor.MiddleCenter);
        RectTransform turnTextRT = turnText.GetComponent<RectTransform>();
        turnTextRT.anchorMin = new Vector2(0, 0);
        turnTextRT.anchorMax = new Vector2(0.5f, 1);
        turnTextRT.offsetMin = new Vector2(20, 0);
        turnTextRT.offsetMax = Vector2.zero;

        GameObject phaseText = CreateText("PhaseText", topBar.transform, "Playing", 24, new Color(0.7f, 0.8f, 1f), TextAnchor.MiddleRight);
        RectTransform phaseTextRT = phaseText.GetComponent<RectTransform>();
        phaseTextRT.anchorMin = new Vector2(0.5f, 0);
        phaseTextRT.anchorMax = new Vector2(1, 1);
        phaseTextRT.offsetMin = Vector2.zero;
        phaseTextRT.offsetMax = new Vector2(-20, 0);

        TurnIndicator turnIndicator = topBar.AddComponent<TurnIndicator>();
        SetField(turnIndicator, typeof(TurnIndicator), "turnText", turnText.GetComponent<Text>());
        SetField(turnIndicator, typeof(TurnIndicator), "phaseText", phaseText.GetComponent<Text>());

        // ── LEFT PANEL (Dashboard) ──
        GameObject dashPanel = CreatePanel("DashboardPanel", canvasGO.transform, new Color(0.15f, 0.17f, 0.22f, 0.95f));
        RectTransform dashRT = dashPanel.GetComponent<RectTransform>();
        dashRT.anchorMin = new Vector2(0.02f, 0.10f);
        dashRT.anchorMax = new Vector2(0.48f, 0.91f);
        dashRT.offsetMin = Vector2.zero;
        dashRT.offsetMax = Vector2.zero;

        // Dashboard title
        CreateText("DashTitle", dashPanel.transform, "Financial Dashboard", 26, Color.white, TextAnchor.MiddleCenter,
            new Vector2(0, 0.90f), new Vector2(1, 1));

        // Profession info
        GameObject profIcon = new GameObject("ProfessionIcon");
        profIcon.transform.SetParent(dashPanel.transform, false);
        RectTransform profIconRT = profIcon.AddComponent<RectTransform>();
        profIconRT.anchorMin = new Vector2(0.05f, 0.78f);
        profIconRT.anchorMax = new Vector2(0.20f, 0.88f);
        profIconRT.offsetMin = Vector2.zero;
        profIconRT.offsetMax = Vector2.zero;
        profIcon.AddComponent<CanvasRenderer>();
        Image profIconImg = profIcon.AddComponent<Image>();
        profIconImg.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);

        GameObject profText = CreateText("ProfessionText", dashPanel.transform, "Profession: --", 22, new Color(0.8f, 0.9f, 1f), TextAnchor.MiddleLeft,
            new Vector2(0.22f, 0.78f), new Vector2(0.95f, 0.88f));

        // Money displays
        GameObject currentMoney = CreateText("CurrentMoneyText", dashPanel.transform, "Current: $5,000", 28, new Color(0.3f, 1f, 0.5f), TextAnchor.MiddleLeft,
            new Vector2(0.05f, 0.66f), new Vector2(0.95f, 0.76f));
        currentMoney.GetComponent<Text>().fontStyle = FontStyle.Bold;

        GameObject totalEarned = CreateText("TotalEarnedText", dashPanel.transform, "Earned: $0", 22, new Color(0.5f, 1f, 0.7f), TextAnchor.MiddleLeft,
            new Vector2(0.05f, 0.57f), new Vector2(0.95f, 0.65f));

        GameObject totalSpent = CreateText("TotalSpentText", dashPanel.transform, "Spent: $0", 22, new Color(1f, 0.5f, 0.5f), TextAnchor.MiddleLeft,
            new Vector2(0.05f, 0.49f), new Vector2(0.95f, 0.57f));

        // Financial Chart area
        GameObject chartArea = CreatePanel("ChartArea", dashPanel.transform, new Color(0.1f, 0.12f, 0.16f, 0.8f));
        RectTransform chartAreaRT = chartArea.GetComponent<RectTransform>();
        chartAreaRT.anchorMin = new Vector2(0.05f, 0.05f);
        chartAreaRT.anchorMax = new Vector2(0.95f, 0.47f);
        chartAreaRT.offsetMin = Vector2.zero;
        chartAreaRT.offsetMax = Vector2.zero;

        CreateText("ChartTitle", chartArea.transform, "Income / Expense History", 18, new Color(0.7f, 0.7f, 0.7f), TextAnchor.MiddleCenter,
            new Vector2(0, 0.88f), new Vector2(1, 1));

        // Chart container for bars
        GameObject chartContainer = new GameObject("ChartContainer");
        chartContainer.transform.SetParent(chartArea.transform, false);
        RectTransform chartContRT = chartContainer.AddComponent<RectTransform>();
        chartContRT.anchorMin = new Vector2(0.05f, 0.05f);
        chartContRT.anchorMax = new Vector2(0.95f, 0.85f);
        chartContRT.offsetMin = Vector2.zero;
        chartContRT.offsetMax = Vector2.zero;

        // Bar prefab for chart
        GameObject barPrefab = CreateChartBarPrefab();

        // Label prefab for chart
        GameObject labelPrefab = CreateChartLabelPrefab();

        // FinancialChart component
        FinancialChart chart = chartArea.AddComponent<FinancialChart>();
        SetField(chart, typeof(FinancialChart), "chartContainer", chartContRT);
        SetField(chart, typeof(FinancialChart), "barPrefab", barPrefab);
        SetField(chart, typeof(FinancialChart), "labelPrefab", labelPrefab.GetComponent<Text>());

        // DashboardPanel component
        DashboardPanel dashboard = dashPanel.AddComponent<DashboardPanel>();
        SetField(dashboard, typeof(DashboardPanel), "currentMoneyText", currentMoney.GetComponent<Text>());
        SetField(dashboard, typeof(DashboardPanel), "totalEarnedText", totalEarned.GetComponent<Text>());
        SetField(dashboard, typeof(DashboardPanel), "totalSpentText", totalSpent.GetComponent<Text>());
        SetField(dashboard, typeof(DashboardPanel), "professionText", profText.GetComponent<Text>());
        SetField(dashboard, typeof(DashboardPanel), "professionIcon", profIconImg);
        SetField(dashboard, typeof(DashboardPanel), "incomeChart", chart);

        // ── RIGHT PANEL (Player Status) ──
        GameObject rightPanel = CreatePanel("PlayerStatusArea", canvasGO.transform, new Color(0.15f, 0.17f, 0.22f, 0.95f));
        RectTransform rightRT = rightPanel.GetComponent<RectTransform>();
        rightRT.anchorMin = new Vector2(0.52f, 0.45f);
        rightRT.anchorMax = new Vector2(0.98f, 0.91f);
        rightRT.offsetMin = Vector2.zero;
        rightRT.offsetMax = Vector2.zero;

        CreateText("PlayersTitle", rightPanel.transform, "Players", 24, Color.white, TextAnchor.MiddleCenter,
            new Vector2(0, 0.88f), new Vector2(1, 1));

        // Create 4 player status panels
        PlayerStatusPanel[] statusPanels = new PlayerStatusPanel[4];
        for (int i = 0; i < 4; i++)
        {
            float yMax = 0.86f - (i * 0.22f);
            float yMin = yMax - 0.20f;

            GameObject playerPanel = CreatePanel($"PlayerStatus_{i + 1}", rightPanel.transform, new Color(0.2f, 0.22f, 0.28f, 0.8f));
            RectTransform ppRT = playerPanel.GetComponent<RectTransform>();
            ppRT.anchorMin = new Vector2(0.05f, yMin);
            ppRT.anchorMax = new Vector2(0.95f, yMax);
            ppRT.offsetMin = Vector2.zero;
            ppRT.offsetMax = Vector2.zero;

            // Player icon
            GameObject pIcon = new GameObject("ProfessionIcon");
            pIcon.transform.SetParent(playerPanel.transform, false);
            RectTransform pIconRT = pIcon.AddComponent<RectTransform>();
            pIconRT.anchorMin = new Vector2(0.02f, 0.15f);
            pIconRT.anchorMax = new Vector2(0.18f, 0.85f);
            pIconRT.offsetMin = Vector2.zero;
            pIconRT.offsetMax = Vector2.zero;
            pIcon.AddComponent<CanvasRenderer>();
            Image pIconImg = pIcon.AddComponent<Image>();
            pIconImg.color = new Color(0.4f, 0.4f, 0.4f, 0.5f);

            GameObject pName = CreateText("PlayerNameText", playerPanel.transform, $"Player {i + 1}", 20, Color.white, TextAnchor.MiddleLeft,
                new Vector2(0.20f, 0.5f), new Vector2(0.70f, 1f));

            GameObject pMoney = CreateText("MoneyText", playerPanel.transform, "$5,000", 20, new Color(0.3f, 1f, 0.5f), TextAnchor.MiddleLeft,
                new Vector2(0.20f, 0f), new Vector2(0.70f, 0.5f));

            // Turn indicator dot
            GameObject turnDot = CreatePanel("TurnIndicator", playerPanel.transform, new Color(1f, 0.84f, 0f));
            RectTransform dotRT = turnDot.GetComponent<RectTransform>();
            dotRT.anchorMin = new Vector2(0.85f, 0.3f);
            dotRT.anchorMax = new Vector2(0.95f, 0.7f);
            dotRT.offsetMin = Vector2.zero;
            dotRT.offsetMax = Vector2.zero;
            turnDot.SetActive(false);

            PlayerStatusPanel psp = playerPanel.AddComponent<PlayerStatusPanel>();
            SetField(psp, typeof(PlayerStatusPanel), "playerNameText", pName.GetComponent<Text>());
            SetField(psp, typeof(PlayerStatusPanel), "moneyText", pMoney.GetComponent<Text>());
            SetField(psp, typeof(PlayerStatusPanel), "professionIcon", pIconImg);
            SetField(psp, typeof(PlayerStatusPanel), "turnIndicator", turnDot);

            statusPanels[i] = psp;
        }

        // ── CENTER-RIGHT (Action area / Next Turn) ──
        GameObject actionArea = CreatePanel("ActionArea", canvasGO.transform, new Color(0.15f, 0.17f, 0.22f, 0.95f));
        RectTransform actionRT = actionArea.GetComponent<RectTransform>();
        actionRT.anchorMin = new Vector2(0.52f, 0.10f);
        actionRT.anchorMax = new Vector2(0.98f, 0.43f);
        actionRT.offsetMin = Vector2.zero;
        actionRT.offsetMax = Vector2.zero;

        CreateText("ActionTitle", actionArea.transform, "Game Actions", 24, Color.white, TextAnchor.MiddleCenter,
            new Vector2(0, 0.85f), new Vector2(1, 1));

        // Next Turn button
        GameObject nextTurnBtn = CreateButton("NextTurnButton", actionArea.transform, "Next Turn", new Color(0.2f, 0.65f, 0.3f));
        RectTransform ntRT = nextTurnBtn.GetComponent<RectTransform>();
        ntRT.anchorMin = new Vector2(0.15f, 0.45f);
        ntRT.anchorMax = new Vector2(0.85f, 0.75f);
        ntRT.offsetMin = Vector2.zero;
        ntRT.offsetMax = Vector2.zero;

        // Save button
        GameObject saveBtn = CreateButton("SaveButton", actionArea.transform, "Save Game", new Color(0.3f, 0.5f, 0.8f));
        RectTransform saveRT = saveBtn.GetComponent<RectTransform>();
        saveRT.anchorMin = new Vector2(0.15f, 0.10f);
        saveRT.anchorMax = new Vector2(0.85f, 0.38f);
        saveRT.offsetMin = Vector2.zero;
        saveRT.offsetMax = Vector2.zero;

        // ── EVENT POPUP (parent stays active, popupPanel child is hidden via script Start) ──
        GameObject eventPopupGO = new GameObject("EventPopup");
        eventPopupGO.transform.SetParent(canvasGO.transform, false);
        RectTransform epRT = eventPopupGO.AddComponent<RectTransform>();
        SetFullStretch(epRT);

        // Popup content panel
        GameObject popupPanel = CreatePanel("PopupPanel", eventPopupGO.transform, new Color(0.18f, 0.20f, 0.26f));
        RectTransform popupRT = popupPanel.GetComponent<RectTransform>();
        popupRT.anchorMin = new Vector2(0.15f, 0.25f);
        popupRT.anchorMax = new Vector2(0.85f, 0.75f);
        popupRT.offsetMin = Vector2.zero;
        popupRT.offsetMax = Vector2.zero;

        // Event icon
        GameObject evtIcon = new GameObject("EventIcon");
        evtIcon.transform.SetParent(popupPanel.transform, false);
        RectTransform evtIconRT = evtIcon.AddComponent<RectTransform>();
        evtIconRT.anchorMin = new Vector2(0.35f, 0.75f);
        evtIconRT.anchorMax = new Vector2(0.65f, 0.95f);
        evtIconRT.offsetMin = Vector2.zero;
        evtIconRT.offsetMax = Vector2.zero;
        evtIcon.AddComponent<CanvasRenderer>();
        Image evtIconImg = evtIcon.AddComponent<Image>();
        evtIconImg.color = new Color(0.4f, 0.6f, 0.9f);

        GameObject evtName = CreateText("EventNameText", popupPanel.transform, "Event Name", 30, Color.white, TextAnchor.MiddleCenter,
            new Vector2(0.05f, 0.62f), new Vector2(0.95f, 0.75f));
        evtName.GetComponent<Text>().fontStyle = FontStyle.Bold;

        GameObject evtDesc = CreateText("EventDescText", popupPanel.transform, "Event description goes here...", 22, new Color(0.8f, 0.8f, 0.8f), TextAnchor.MiddleCenter,
            new Vector2(0.08f, 0.48f), new Vector2(0.92f, 0.62f));

        GameObject evtAmount = CreateText("AmountText", popupPanel.transform, "-$1,200", 36, new Color(1f, 0.4f, 0.4f), TextAnchor.MiddleCenter,
            new Vector2(0.1f, 0.35f), new Vector2(0.9f, 0.48f));
        evtAmount.GetComponent<Text>().fontStyle = FontStyle.Bold;

        // Choice container
        GameObject choiceContainer = new GameObject("ChoiceContainer");
        choiceContainer.transform.SetParent(popupPanel.transform, false);
        RectTransform choiceContRT = choiceContainer.AddComponent<RectTransform>();
        choiceContRT.anchorMin = new Vector2(0.08f, 0.12f);
        choiceContRT.anchorMax = new Vector2(0.92f, 0.33f);
        choiceContRT.offsetMin = Vector2.zero;
        choiceContRT.offsetMax = Vector2.zero;
        VerticalLayoutGroup choiceVLG = choiceContainer.AddComponent<VerticalLayoutGroup>();
        choiceVLG.spacing = 6;
        choiceVLG.childControlWidth = true;
        choiceVLG.childControlHeight = false;
        choiceVLG.childForceExpandWidth = true;

        // Close button
        GameObject closeBtn = CreateButton("CloseButton", popupPanel.transform, "OK", new Color(0.3f, 0.6f, 0.3f));
        RectTransform closeRT = closeBtn.GetComponent<RectTransform>();
        closeRT.anchorMin = new Vector2(0.30f, 0.02f);
        closeRT.anchorMax = new Vector2(0.70f, 0.11f);
        closeRT.offsetMin = Vector2.zero;
        closeRT.offsetMax = Vector2.zero;

        // Choice button prefab
        GameObject choiceBtnPrefab = CreateChoiceButtonPrefab();

        // EventPopup component
        EventPopup eventPopup = eventPopupGO.AddComponent<EventPopup>();
        SetField(eventPopup, typeof(EventPopup), "popupPanel", popupPanel);
        SetField(eventPopup, typeof(EventPopup), "eventIcon", evtIconImg);
        SetField(eventPopup, typeof(EventPopup), "eventNameText", evtName.GetComponent<Text>());
        SetField(eventPopup, typeof(EventPopup), "eventDescriptionText", evtDesc.GetComponent<Text>());
        SetField(eventPopup, typeof(EventPopup), "amountText", evtAmount.GetComponent<Text>());
        SetField(eventPopup, typeof(EventPopup), "closeButton", closeBtn.GetComponent<Button>());
        SetField(eventPopup, typeof(EventPopup), "choiceContainer", choiceContainer.transform);
        SetField(eventPopup, typeof(EventPopup), "choiceButtonPrefab", choiceBtnPrefab.GetComponent<Button>());

        // ── MANAGERS GAMEOBJECT ──
        GameObject managersGO = new GameObject("Managers");

        TurnManager turnMgr = managersGO.AddComponent<TurnManager>();
        EconomyManager econMgr = managersGO.AddComponent<EconomyManager>();
        EventManager evtMgr = managersGO.AddComponent<EventManager>();
        UIManager uiMgr = managersGO.AddComponent<UIManager>();

        // Wire TurnManager
        SetField(turnMgr, typeof(TurnManager), "economyManager", econMgr);
        SetField(turnMgr, typeof(TurnManager), "eventManager", evtMgr);

        // Wire EventManager
        SetField(evtMgr, typeof(EventManager), "economyManager", econMgr);

        // Wire UIManager
        SetField(uiMgr, typeof(UIManager), "dashboardPanel", dashboard);
        SetField(uiMgr, typeof(UIManager), "eventPopup", eventPopup);
        SetField(uiMgr, typeof(UIManager), "turnIndicator", turnIndicator);
        SetField(uiMgr, typeof(UIManager), "playerStatusPanels", statusPanels);
        SetField(uiMgr, typeof(UIManager), "economyManager", econMgr);
        SetField(uiMgr, typeof(UIManager), "eventManager", evtMgr);
        SetField(uiMgr, typeof(UIManager), "turnManager", turnMgr);

        // ── GAMEBOARD CONTROLLER (connects buttons to managers) ──
        GameBoardController boardController = canvasGO.AddComponent<GameBoardController>();
        SetField(boardController, typeof(GameBoardController), "nextTurnButton", nextTurnBtn.GetComponent<Button>());
        SetField(boardController, typeof(GameBoardController), "saveButton", saveBtn.GetComponent<Button>());
        SetField(boardController, typeof(GameBoardController), "turnManager", turnMgr);

        // ── EVENT SYSTEM ──
        if (Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        // ── SAVE ──
        string scenePath = "Assets/Scenes/GameBoard.unity";
        EditorSceneManager.SaveScene(scene, scenePath);
        Debug.Log("[OK] GameBoard scene created with all UI + Managers wired");
    }

    // ─────────────────────────────────────────────
    // BUILD SETTINGS
    // ─────────────────────────────────────────────

    [MenuItem("Tools/Finance Game/Setup Build Settings")]
    static void SetupBuildSettings()
    {
        Debug.Log("\n── Configuring Build Settings ──");

        string[] sceneOrder = {
            "Assets/Scenes/MainMenu.unity",
            "Assets/Scenes/RoleSelection.unity",
            "Assets/Scenes/GameBoard.unity",
            "Assets/Scenes/GameSummary.unity"
        };

        var buildScenes = new List<EditorBuildSettingsScene>();

        foreach (string path in sceneOrder)
        {
            if (System.IO.File.Exists(path))
            {
                buildScenes.Add(new EditorBuildSettingsScene(path, true));
                Debug.Log($"  [{buildScenes.Count - 1}] {path}");
            }
            else
            {
                Debug.LogWarning($"  [MISSING] {path} - scene file not found!");
            }
        }

        EditorBuildSettings.scenes = buildScenes.ToArray();
        Debug.Log($"[OK] Build Settings: {buildScenes.Count} scenes configured");
    }

    // ─────────────────────────────────────────────
    // PREFAB CREATORS
    // ─────────────────────────────────────────────

    static GameObject CreateChartBarPrefab()
    {
        string prefabDir = "Assets/PreFabs/UI";
        if (!System.IO.Directory.Exists(prefabDir))
            System.IO.Directory.CreateDirectory(prefabDir);

        string path = prefabDir + "/ChartBar.prefab";
        var existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (existing != null) return existing;

        GameObject bar = new GameObject("ChartBar");
        RectTransform rt = bar.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(20, 50);
        bar.AddComponent<CanvasRenderer>();
        Image img = bar.AddComponent<Image>();
        img.color = Color.green;

        GameObject saved = PrefabUtility.SaveAsPrefabAsset(bar, path);
        Object.DestroyImmediate(bar);
        Debug.Log("  ChartBar prefab created");
        return saved;
    }

    static GameObject CreateChartLabelPrefab()
    {
        string prefabDir = "Assets/PreFabs/UI";
        string path = prefabDir + "/ChartLabel.prefab";
        var existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (existing != null) return existing;

        GameObject label = new GameObject("ChartLabel");
        label.AddComponent<RectTransform>();
        Text t = label.AddComponent<Text>();
        t.fontSize = 14;
        t.color = Color.white;
        t.alignment = TextAnchor.MiddleCenter;
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        GameObject saved = PrefabUtility.SaveAsPrefabAsset(label, path);
        Object.DestroyImmediate(label);
        Debug.Log("  ChartLabel prefab created");
        return saved;
    }

    static GameObject CreateChoiceButtonPrefab()
    {
        string prefabDir = "Assets/PreFabs/UI";
        string path = prefabDir + "/ChoiceButton.prefab";
        var existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (existing != null) return existing;

        GameObject btn = new GameObject("ChoiceButton");
        btn.AddComponent<RectTransform>();
        btn.AddComponent<CanvasRenderer>();
        Image img = btn.AddComponent<Image>();
        img.color = new Color(0.25f, 0.45f, 0.7f);
        Button b = btn.AddComponent<Button>();
        b.targetGraphic = img;

        GameObject txt = new GameObject("Text");
        txt.transform.SetParent(btn.transform, false);
        RectTransform txtRT = txt.AddComponent<RectTransform>();
        txtRT.anchorMin = Vector2.zero;
        txtRT.anchorMax = Vector2.one;
        txtRT.offsetMin = Vector2.zero;
        txtRT.offsetMax = Vector2.zero;
        Text t = txt.AddComponent<Text>();
        t.text = "Choice";
        t.fontSize = 20;
        t.color = Color.white;
        t.alignment = TextAnchor.MiddleCenter;
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        LayoutElement le = btn.AddComponent<LayoutElement>();
        le.minHeight = 45;
        le.preferredHeight = 45;

        GameObject saved = PrefabUtility.SaveAsPrefabAsset(btn, path);
        Object.DestroyImmediate(btn);
        Debug.Log("  ChoiceButton prefab created");
        return saved;
    }

    // ─────────────────────────────────────────────
    // UI HELPERS
    // ─────────────────────────────────────────────

    static GameObject CreateCanvas(string name)
    {
        GameObject go = new GameObject(name);
        Canvas canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        go.AddComponent<GraphicRaycaster>();
        return go;
    }

    static GameObject CreatePanel(string name, Transform parent, Color color)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        panel.AddComponent<RectTransform>();
        panel.AddComponent<CanvasRenderer>();
        Image img = panel.AddComponent<Image>();
        img.color = color;
        return panel;
    }

    static GameObject CreateText(string name, Transform parent, string content,
        int fontSize, Color color, TextAnchor alignment)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.AddComponent<RectTransform>();
        Text text = go.AddComponent<Text>();
        text.text = content;
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = alignment;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        return go;
    }

    static GameObject CreateText(string name, Transform parent, string content,
        int fontSize, Color color, TextAnchor alignment, Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject go = CreateText(name, parent, content, fontSize, color, alignment);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        return go;
    }

    static GameObject CreateButton(string name, Transform parent, string label, Color bgColor)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.AddComponent<RectTransform>();
        go.AddComponent<CanvasRenderer>();
        Image img = go.AddComponent<Image>();
        img.color = bgColor;
        Button btn = go.AddComponent<Button>();
        btn.targetGraphic = img;

        GameObject txt = CreateText("Text", go.transform, label, 24, Color.white, TextAnchor.MiddleCenter);
        SetFullStretch(txt.GetComponent<RectTransform>());

        return go;
    }

    static void SetFullStretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    static void SetField(object obj, System.Type type, string fieldName, object value)
    {
        var field = type.GetField(fieldName, PRIVATE_FIELD);
        if (field != null)
            field.SetValue(obj, value);
        else
            Debug.LogWarning($"Field '{fieldName}' not found on {type.Name}");
    }
}
