using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// Editor script to create the GameSummary scene with all UI elements.
/// Run from Unity menu: Tools > Finance Game > Create GameSummary Scene
/// </summary>
public class CreateGameSummaryScene : Editor
{
    [MenuItem("Tools/Finance Game/Create GameSummary Scene")]
    static void CreateScene()
    {
        // Create new empty scene
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        Debug.Log("Creating GameSummary scene...");

        // ── CANVAS ──
        GameObject canvasGO = new GameObject("Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasGO.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1080, 1920);
        canvasGO.AddComponent<GraphicRaycaster>();

        // ── BACKGROUND PANEL ──
        GameObject bgPanel = CreatePanel("BackgroundPanel", canvasGO.transform);
        bgPanel.GetComponent<Image>().color = new Color(0.1f, 0.12f, 0.18f, 1f);
        SetFullStretch(bgPanel.GetComponent<RectTransform>());

        // ── HEADER: "Game Over!" ──
        GameObject headerText = CreateText("HeaderText", canvasGO.transform,
            "Game Over!", 60, Color.white, TextAnchor.MiddleCenter);
        RectTransform headerRT = headerText.GetComponent<RectTransform>();
        headerRT.anchorMin = new Vector2(0.1f, 0.82f);
        headerRT.anchorMax = new Vector2(0.9f, 0.92f);
        headerRT.offsetMin = Vector2.zero;
        headerRT.offsetMax = Vector2.zero;
        headerText.GetComponent<Text>().fontStyle = FontStyle.Bold;

        // ── WINNER TEXT ──
        GameObject winnerText = CreateText("WinnerText", canvasGO.transform,
            "Winner: Player Name\n$00,000", 42, new Color(1f, 0.84f, 0f), TextAnchor.MiddleCenter);
        RectTransform winnerRT = winnerText.GetComponent<RectTransform>();
        winnerRT.anchorMin = new Vector2(0.1f, 0.68f);
        winnerRT.anchorMax = new Vector2(0.9f, 0.80f);
        winnerRT.offsetMin = Vector2.zero;
        winnerRT.offsetMax = Vector2.zero;

        // ── RANKING PANEL ──
        GameObject rankingPanel = CreatePanel("RankingPanel", canvasGO.transform);
        rankingPanel.GetComponent<Image>().color = new Color(0.15f, 0.17f, 0.22f, 0.9f);
        RectTransform rankPanelRT = rankingPanel.GetComponent<RectTransform>();
        rankPanelRT.anchorMin = new Vector2(0.08f, 0.30f);
        rankPanelRT.anchorMax = new Vector2(0.92f, 0.66f);
        rankPanelRT.offsetMin = Vector2.zero;
        rankPanelRT.offsetMax = Vector2.zero;

        // Ranking title
        GameObject rankTitle = CreateText("RankingTitle", rankingPanel.transform,
            "Final Rankings", 32, Color.white, TextAnchor.MiddleCenter);
        RectTransform rankTitleRT = rankTitle.GetComponent<RectTransform>();
        rankTitleRT.anchorMin = new Vector2(0f, 0.85f);
        rankTitleRT.anchorMax = new Vector2(1f, 1f);
        rankTitleRT.offsetMin = Vector2.zero;
        rankTitleRT.offsetMax = Vector2.zero;

        // Ranking container (with VerticalLayoutGroup)
        GameObject rankingContainer = new GameObject("RankingContainer");
        rankingContainer.transform.SetParent(rankingPanel.transform, false);
        RectTransform containerRT = rankingContainer.AddComponent<RectTransform>();
        containerRT.anchorMin = new Vector2(0.05f, 0.05f);
        containerRT.anchorMax = new Vector2(0.95f, 0.83f);
        containerRT.offsetMin = Vector2.zero;
        containerRT.offsetMax = Vector2.zero;

        VerticalLayoutGroup vlg = rankingContainer.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 8;
        vlg.childAlignment = TextAnchor.UpperCenter;
        vlg.childControlWidth = true;
        vlg.childControlHeight = false;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;

        // ── PLAYER RANK PREFAB ──
        GameObject playerRankPrefab = CreatePlayerRankPrefab();

        // ── BUTTONS PANEL ──
        GameObject buttonsPanel = new GameObject("ButtonsPanel");
        buttonsPanel.transform.SetParent(canvasGO.transform, false);
        RectTransform btnPanelRT = buttonsPanel.AddComponent<RectTransform>();
        btnPanelRT.anchorMin = new Vector2(0.1f, 0.10f);
        btnPanelRT.anchorMax = new Vector2(0.9f, 0.26f);
        btnPanelRT.offsetMin = Vector2.zero;
        btnPanelRT.offsetMax = Vector2.zero;

        HorizontalLayoutGroup hlg = buttonsPanel.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 30;
        hlg.childAlignment = TextAnchor.MiddleCenter;
        hlg.childControlWidth = true;
        hlg.childControlHeight = true;
        hlg.childForceExpandWidth = true;
        hlg.childForceExpandHeight = true;

        // Play Again button
        GameObject playAgainBtn = CreateButton("PlayAgainButton", buttonsPanel.transform,
            "Play Again", new Color(0.2f, 0.7f, 0.3f, 1f));

        // Main Menu button
        GameObject mainMenuBtn = CreateButton("MainMenuButton", buttonsPanel.transform,
            "Main Menu", new Color(0.3f, 0.5f, 0.8f, 1f));

        // ── GAME SUMMARY CONTROLLER ──
        GameSummaryController controller = canvasGO.AddComponent<GameSummaryController>();

        // Assign references via reflection
        var type = typeof(GameSummaryController);
        var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;

        type.GetField("winnerText", flags)?.SetValue(controller, winnerText.GetComponent<Text>());
        type.GetField("rankingContainer", flags)?.SetValue(controller, rankingContainer.transform);
        type.GetField("playerRankPrefab", flags)?.SetValue(controller, playerRankPrefab);
        type.GetField("playAgainButton", flags)?.SetValue(controller, playAgainBtn.GetComponent<Button>());
        type.GetField("mainMenuButton", flags)?.SetValue(controller, mainMenuBtn.GetComponent<Button>());

        // ── EVENT SYSTEM ──
        if (Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        // ── SAVE SCENE ──
        string scenePath = "Assets/Scenes/GameSummary.unity";
        EditorSceneManager.SaveScene(scene, scenePath);

        // Add to Build Settings
        AddSceneToBuildSettings(scenePath);

        Debug.Log("GameSummary scene created at: " + scenePath);
        Debug.Log("UI elements: WinnerText, RankingContainer, PlayAgain/MainMenu buttons");
        Debug.Log("GameSummaryController attached with all references connected");
        Debug.Log("Scene added to Build Settings");
    }

    // ── HELPER METHODS ──

    static GameObject CreatePanel(string name, Transform parent)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        panel.AddComponent<RectTransform>();
        panel.AddComponent<CanvasRenderer>();
        panel.AddComponent<Image>();
        return panel;
    }

    static GameObject CreateText(string name, Transform parent, string content,
        int fontSize, Color color, TextAnchor alignment)
    {
        GameObject textGO = new GameObject(name);
        textGO.transform.SetParent(parent, false);
        textGO.AddComponent<RectTransform>();
        Text text = textGO.AddComponent<Text>();
        text.text = content;
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = alignment;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        return textGO;
    }

    static GameObject CreateButton(string name, Transform parent, string label, Color bgColor)
    {
        GameObject btnGO = new GameObject(name);
        btnGO.transform.SetParent(parent, false);
        btnGO.AddComponent<RectTransform>();
        btnGO.AddComponent<CanvasRenderer>();
        Image btnImage = btnGO.AddComponent<Image>();
        btnImage.color = bgColor;
        Button btn = btnGO.AddComponent<Button>();
        btn.targetGraphic = btnImage;

        // Button text
        GameObject textGO = CreateText("Text", btnGO.transform, label, 28, Color.white, TextAnchor.MiddleCenter);
        SetFullStretch(textGO.GetComponent<RectTransform>());

        // Layout element for minimum size
        LayoutElement le = btnGO.AddComponent<LayoutElement>();
        le.minHeight = 60;

        return btnGO;
    }

    static GameObject CreatePlayerRankPrefab()
    {
        // Create a simple prefab for player ranking rows
        GameObject prefab = new GameObject("PlayerRankItem");
        RectTransform rt = prefab.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(0, 50);

        prefab.AddComponent<CanvasRenderer>();
        Image bg = prefab.AddComponent<Image>();
        bg.color = new Color(0.2f, 0.22f, 0.28f, 0.8f);

        // Rank text
        GameObject rankText = CreateText("RankText", prefab.transform,
            "1. Player - $0", 26, Color.white, TextAnchor.MiddleLeft);
        RectTransform textRT = rankText.GetComponent<RectTransform>();
        textRT.anchorMin = new Vector2(0.05f, 0f);
        textRT.anchorMax = new Vector2(0.95f, 1f);
        textRT.offsetMin = Vector2.zero;
        textRT.offsetMax = Vector2.zero;

        LayoutElement le = prefab.AddComponent<LayoutElement>();
        le.minHeight = 50;
        le.preferredHeight = 50;

        // Save as prefab
        string prefabDir = "Assets/PreFabs/UI";
        if (!System.IO.Directory.Exists(prefabDir))
        {
            System.IO.Directory.CreateDirectory(prefabDir);
        }

        string prefabPath = prefabDir + "/PlayerRankItem.prefab";
        GameObject savedPrefab = PrefabUtility.SaveAsPrefabAsset(prefab, prefabPath);
        Object.DestroyImmediate(prefab);

        Debug.Log("PlayerRankItem prefab created at: " + prefabPath);
        return savedPrefab;
    }

    static void SetFullStretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    static void AddSceneToBuildSettings(string scenePath)
    {
        var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);

        // Check if already added
        foreach (var s in scenes)
        {
            if (s.path == scenePath) return;
        }

        scenes.Add(new EditorBuildSettingsScene(scenePath, true));
        EditorBuildSettings.scenes = scenes.ToArray();
    }
}
