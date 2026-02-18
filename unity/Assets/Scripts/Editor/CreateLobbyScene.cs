using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// Creates the Lobby scene with all required UI elements for multiplayer room management.
/// Run from Unity menu: Tools > Finance Game > Create Lobby Scene
/// </summary>
public class CreateLobbyScene : Editor
{
    [MenuItem("Tools/Finance Game/Create Lobby Scene")]
    static void Create()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        Debug.Log("Creating Lobby scene...");

        // Set background color
        Camera.main.backgroundColor = new Color(0.1f, 0.12f, 0.18f);

        // ===== Canvas =====
        GameObject canvasGO = new GameObject("Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGO.AddComponent<GraphicRaycaster>();

        // ===== Title =====
        CreateText(canvasGO.transform, "TitleText", "MULTIPLAYER LOBBY",
            new Vector2(0.3f, 0.9f), new Vector2(0.7f, 0.98f),
            32, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white);

        // ===== Lobby Panel =====
        GameObject lobbyPanel = CreatePanel(canvasGO.transform, "LobbyPanel",
            new Vector2(0.05f, 0.05f), new Vector2(0.95f, 0.88f),
            new Color(0.15f, 0.17f, 0.22f, 0.95f));

        // Room Name Input
        GameObject inputGO = CreateInputField(lobbyPanel.transform, "RoomNameInput",
            new Vector2(0.05f, 0.85f), new Vector2(0.55f, 0.95f),
            "Enter room name...");

        // Create Room Button
        GameObject createBtn = CreateButton(lobbyPanel.transform, "CreateRoomButton",
            new Vector2(0.58f, 0.85f), new Vector2(0.78f, 0.95f),
            "Create Room", new Color(0.2f, 0.6f, 0.3f));

        // Join Random Button
        GameObject joinRandomBtn = CreateButton(lobbyPanel.transform, "JoinRandomButton",
            new Vector2(0.8f, 0.85f), new Vector2(0.95f, 0.95f),
            "Quick Join", new Color(0.2f, 0.4f, 0.8f));

        // Room List Label
        CreateText(lobbyPanel.transform, "RoomListLabel", "Available Rooms:",
            new Vector2(0.05f, 0.75f), new Vector2(0.4f, 0.83f),
            20, FontStyle.Bold, TextAnchor.MiddleLeft, Color.white);

        // Room List Container (with scroll)
        GameObject roomScrollGO = new GameObject("RoomListScroll");
        roomScrollGO.transform.SetParent(lobbyPanel.transform, false);
        RectTransform roomScrollRect = roomScrollGO.AddComponent<RectTransform>();
        roomScrollRect.anchorMin = new Vector2(0.05f, 0.1f);
        roomScrollRect.anchorMax = new Vector2(0.95f, 0.75f);
        roomScrollRect.anchoredPosition = Vector2.zero;
        roomScrollRect.sizeDelta = Vector2.zero;
        roomScrollGO.AddComponent<Image>().color = new Color(0.1f, 0.12f, 0.16f, 0.8f);
        roomScrollGO.AddComponent<Mask>().showMaskGraphic = true;

        ScrollRect roomScroll = roomScrollGO.AddComponent<ScrollRect>();
        roomScroll.horizontal = false;

        GameObject roomContent = new GameObject("RoomListContainer");
        roomContent.transform.SetParent(roomScrollGO.transform, false);
        RectTransform roomContentRect = roomContent.AddComponent<RectTransform>();
        roomContentRect.anchorMin = new Vector2(0, 1);
        roomContentRect.anchorMax = new Vector2(1, 1);
        roomContentRect.pivot = new Vector2(0.5f, 1);
        roomContentRect.anchoredPosition = Vector2.zero;
        roomContentRect.sizeDelta = new Vector2(0, 0);

        VerticalLayoutGroup roomVLG = roomContent.AddComponent<VerticalLayoutGroup>();
        roomVLG.spacing = 5;
        roomVLG.padding = new RectOffset(10, 10, 10, 10);
        roomVLG.childControlWidth = true;
        roomVLG.childControlHeight = false;
        roomVLG.childForceExpandWidth = true;
        roomVLG.childForceExpandHeight = false;

        ContentSizeFitter roomCSF = roomContent.AddComponent<ContentSizeFitter>();
        roomCSF.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        roomScroll.content = roomContentRect;

        // Back Button
        GameObject backBtn = CreateButton(lobbyPanel.transform, "BackButton",
            new Vector2(0.35f, 0.01f), new Vector2(0.65f, 0.08f),
            "Back to Menu", new Color(0.5f, 0.3f, 0.3f));

        // ===== Room Panel (hidden by default) =====
        GameObject roomPanel = CreatePanel(canvasGO.transform, "RoomPanel",
            new Vector2(0.05f, 0.05f), new Vector2(0.95f, 0.88f),
            new Color(0.15f, 0.17f, 0.22f, 0.95f));

        // Room Name
        Text roomNameText = CreateText(roomPanel.transform, "RoomNameText", "Room: ...",
            new Vector2(0.05f, 0.88f), new Vector2(0.7f, 0.98f),
            26, FontStyle.Bold, TextAnchor.MiddleLeft, Color.white);

        // Leave Room Button
        GameObject leaveBtn = CreateButton(roomPanel.transform, "LeaveRoomButton",
            new Vector2(0.75f, 0.88f), new Vector2(0.95f, 0.98f),
            "Leave", new Color(0.7f, 0.2f, 0.2f));

        // Players Label
        CreateText(roomPanel.transform, "PlayersLabel", "Players:",
            new Vector2(0.05f, 0.78f), new Vector2(0.3f, 0.86f),
            20, FontStyle.Bold, TextAnchor.MiddleLeft, Color.white);

        // Player List Container
        GameObject playerListGO = new GameObject("PlayerListContainer");
        playerListGO.transform.SetParent(roomPanel.transform, false);
        RectTransform playerListRect = playerListGO.AddComponent<RectTransform>();
        playerListRect.anchorMin = new Vector2(0.05f, 0.3f);
        playerListRect.anchorMax = new Vector2(0.95f, 0.78f);
        playerListRect.anchoredPosition = Vector2.zero;
        playerListRect.sizeDelta = Vector2.zero;

        VerticalLayoutGroup playerVLG = playerListGO.AddComponent<VerticalLayoutGroup>();
        playerVLG.spacing = 8;
        playerVLG.padding = new RectOffset(10, 10, 10, 10);
        playerVLG.childControlWidth = true;
        playerVLG.childControlHeight = false;
        playerVLG.childForceExpandWidth = true;
        playerVLG.childForceExpandHeight = false;

        ContentSizeFitter playerCSF = playerListGO.AddComponent<ContentSizeFitter>();
        playerCSF.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // Ready Button
        GameObject readyBtn = CreateButton(roomPanel.transform, "ReadyButton",
            new Vector2(0.1f, 0.12f), new Vector2(0.45f, 0.25f),
            "Ready", Color.white);

        // Start Game Button (Master Client only)
        GameObject startBtn = CreateButton(roomPanel.transform, "StartGameButton",
            new Vector2(0.55f, 0.12f), new Vector2(0.9f, 0.25f),
            "Start Game", new Color(0.2f, 0.6f, 0.3f));

        roomPanel.SetActive(false);

        // ===== Status Text =====
        Text statusTextComp = CreateText(canvasGO.transform, "StatusText", "",
            new Vector2(0.2f, 0.01f), new Vector2(0.8f, 0.05f),
            16, FontStyle.Italic, TextAnchor.MiddleCenter, new Color(0.7f, 0.7f, 0.7f));

        // ===== LobbyController Component =====
        LobbyController controller = canvasGO.AddComponent<LobbyController>();

        // Use SerializedObject to set private SerializeField references
        SerializedObject so = new SerializedObject(controller);

        so.FindProperty("lobbyPanel").objectReferenceValue = lobbyPanel;
        so.FindProperty("roomNameInput").objectReferenceValue = inputGO.GetComponent<InputField>();
        so.FindProperty("createRoomButton").objectReferenceValue = createBtn.GetComponent<Button>();
        so.FindProperty("joinRandomButton").objectReferenceValue = joinRandomBtn.GetComponent<Button>();
        so.FindProperty("backButton").objectReferenceValue = backBtn.GetComponent<Button>();
        so.FindProperty("roomListContainer").objectReferenceValue = roomContent.transform;
        so.FindProperty("roomPanel").objectReferenceValue = roomPanel;
        so.FindProperty("roomNameText").objectReferenceValue = roomNameText;
        so.FindProperty("playerListContainer").objectReferenceValue = playerListGO.transform;
        so.FindProperty("readyButton").objectReferenceValue = readyBtn.GetComponent<Button>();
        so.FindProperty("readyButtonText").objectReferenceValue = readyBtn.GetComponentInChildren<Text>();
        so.FindProperty("startGameButton").objectReferenceValue = startBtn.GetComponent<Button>();
        so.FindProperty("startGameButtonText").objectReferenceValue = startBtn.GetComponentInChildren<Text>();
        so.FindProperty("leaveRoomButton").objectReferenceValue = leaveBtn.GetComponent<Button>();
        so.FindProperty("statusText").objectReferenceValue = statusTextComp;

        so.ApplyModifiedProperties();

        // ===== Create Prefabs =====
        CreateRoomListItemPrefab();
        CreatePlayerListItemPrefab();

        // Set roomListItemPrefab and playerListItemPrefab
        string roomPrefabPath = "Assets/PreFabs/UI/RoomListItem.prefab";
        string playerPrefabPath = "Assets/PreFabs/UI/PlayerListItem.prefab";
        GameObject roomPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(roomPrefabPath);
        GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(playerPrefabPath);

        so = new SerializedObject(controller);
        so.FindProperty("roomListItemPrefab").objectReferenceValue = roomPrefab;
        so.FindProperty("playerListItemPrefab").objectReferenceValue = playerPrefab;
        so.ApplyModifiedProperties();

        // ===== Event System =====
        if (Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        // ===== Save Scene =====
        string scenePath = "Assets/Scenes/Lobby.unity";
        EditorSceneManager.SaveScene(scene, scenePath);
        Debug.Log($"[OK] Lobby scene created at {scenePath}");

        // Add to Build Settings
        AddSceneToBuildSettings(scenePath);
        Debug.Log("[OK] Lobby scene added to Build Settings");
    }

    static void CreateRoomListItemPrefab()
    {
        string path = "Assets/PreFabs/UI/RoomListItem.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null)
        {
            Debug.Log("  RoomListItem prefab already exists");
            return;
        }

        GameObject go = new GameObject("RoomListItem");

        RectTransform rt = go.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(0, 50);

        Image bg = go.AddComponent<Image>();
        bg.color = new Color(0.2f, 0.22f, 0.28f, 0.9f);

        LayoutElement le = go.AddComponent<LayoutElement>();
        le.minHeight = 50;
        le.preferredHeight = 50;

        // Room Name Text
        GameObject nameGO = new GameObject("RoomNameText");
        nameGO.transform.SetParent(go.transform, false);
        RectTransform nameRT = nameGO.AddComponent<RectTransform>();
        nameRT.anchorMin = new Vector2(0.05f, 0);
        nameRT.anchorMax = new Vector2(0.5f, 1);
        nameRT.anchoredPosition = Vector2.zero;
        nameRT.sizeDelta = Vector2.zero;
        Text nameText = nameGO.AddComponent<Text>();
        nameText.text = "Room Name";
        nameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        nameText.fontSize = 18;
        nameText.color = Color.white;
        nameText.alignment = TextAnchor.MiddleLeft;

        // Player Count Text
        GameObject countGO = new GameObject("PlayerCountText");
        countGO.transform.SetParent(go.transform, false);
        RectTransform countRT = countGO.AddComponent<RectTransform>();
        countRT.anchorMin = new Vector2(0.5f, 0);
        countRT.anchorMax = new Vector2(0.7f, 1);
        countRT.anchoredPosition = Vector2.zero;
        countRT.sizeDelta = Vector2.zero;
        Text countText = countGO.AddComponent<Text>();
        countText.text = "0/4";
        countText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        countText.fontSize = 16;
        countText.color = new Color(0.7f, 0.7f, 0.7f);
        countText.alignment = TextAnchor.MiddleCenter;

        // Join Button
        GameObject joinGO = new GameObject("JoinButton");
        joinGO.transform.SetParent(go.transform, false);
        RectTransform joinRT = joinGO.AddComponent<RectTransform>();
        joinRT.anchorMin = new Vector2(0.75f, 0.1f);
        joinRT.anchorMax = new Vector2(0.95f, 0.9f);
        joinRT.anchoredPosition = Vector2.zero;
        joinRT.sizeDelta = Vector2.zero;
        Image joinBg = joinGO.AddComponent<Image>();
        joinBg.color = new Color(0.2f, 0.5f, 0.8f);
        Button joinBtn = joinGO.AddComponent<Button>();
        joinBtn.targetGraphic = joinBg;

        GameObject joinTextGO = new GameObject("Text");
        joinTextGO.transform.SetParent(joinGO.transform, false);
        RectTransform joinTextRT = joinTextGO.AddComponent<RectTransform>();
        joinTextRT.anchorMin = Vector2.zero;
        joinTextRT.anchorMax = Vector2.one;
        joinTextRT.anchoredPosition = Vector2.zero;
        joinTextRT.sizeDelta = Vector2.zero;
        Text joinText = joinTextGO.AddComponent<Text>();
        joinText.text = "Join";
        joinText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        joinText.fontSize = 16;
        joinText.color = Color.white;
        joinText.alignment = TextAnchor.MiddleCenter;

        // Add RoomListItem component
        RoomListItem roomListItem = go.AddComponent<RoomListItem>();
        SerializedObject so = new SerializedObject(roomListItem);
        so.FindProperty("roomNameText").objectReferenceValue = nameText;
        so.FindProperty("playerCountText").objectReferenceValue = countText;
        so.FindProperty("joinButton").objectReferenceValue = joinBtn;
        so.ApplyModifiedProperties();

        // Ensure directory exists
        if (!AssetDatabase.IsValidFolder("Assets/PreFabs/UI"))
        {
            if (!AssetDatabase.IsValidFolder("Assets/PreFabs"))
                AssetDatabase.CreateFolder("Assets", "PreFabs");
            AssetDatabase.CreateFolder("Assets/PreFabs", "UI");
        }

        PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
        Debug.Log("  [OK] RoomListItem prefab created");
    }

    static void CreatePlayerListItemPrefab()
    {
        string path = "Assets/PreFabs/UI/PlayerListItem.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null)
        {
            Debug.Log("  PlayerListItem prefab already exists");
            return;
        }

        GameObject go = new GameObject("PlayerListItem");

        RectTransform rt = go.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(0, 60);

        Image bg = go.AddComponent<Image>();
        bg.color = new Color(0.2f, 0.22f, 0.28f, 0.5f);

        LayoutElement le = go.AddComponent<LayoutElement>();
        le.minHeight = 60;
        le.preferredHeight = 60;

        // Player Name Text
        GameObject nameGO = new GameObject("PlayerNameText");
        nameGO.transform.SetParent(go.transform, false);
        RectTransform nameRT = nameGO.AddComponent<RectTransform>();
        nameRT.anchorMin = new Vector2(0.05f, 0);
        nameRT.anchorMax = new Vector2(0.65f, 1);
        nameRT.anchoredPosition = Vector2.zero;
        nameRT.sizeDelta = Vector2.zero;
        Text nameText = nameGO.AddComponent<Text>();
        nameText.text = "Player Name";
        nameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        nameText.fontSize = 20;
        nameText.color = Color.white;
        nameText.alignment = TextAnchor.MiddleLeft;

        // Ready Status Text
        GameObject readyGO = new GameObject("ReadyStatusText");
        readyGO.transform.SetParent(go.transform, false);
        RectTransform readyRT = readyGO.AddComponent<RectTransform>();
        readyRT.anchorMin = new Vector2(0.7f, 0);
        readyRT.anchorMax = new Vector2(0.95f, 1);
        readyRT.anchoredPosition = Vector2.zero;
        readyRT.sizeDelta = Vector2.zero;
        Text readyText = readyGO.AddComponent<Text>();
        readyText.text = "NOT READY";
        readyText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        readyText.fontSize = 16;
        readyText.fontStyle = FontStyle.Bold;
        readyText.color = Color.red;
        readyText.alignment = TextAnchor.MiddleCenter;

        // Add PlayerListItem component
        PlayerListItem playerListItem = go.AddComponent<PlayerListItem>();
        SerializedObject so = new SerializedObject(playerListItem);
        so.FindProperty("playerNameText").objectReferenceValue = nameText;
        so.FindProperty("readyStatusText").objectReferenceValue = readyText;
        so.FindProperty("backgroundImage").objectReferenceValue = bg;
        so.ApplyModifiedProperties();

        PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
        Debug.Log("  [OK] PlayerListItem prefab created");
    }

    // ===== Helper Methods =====

    static GameObject CreatePanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = Vector2.zero;
        Image img = go.AddComponent<Image>();
        img.color = color;
        return go;
    }

    static Text CreateText(Transform parent, string name, string content,
        Vector2 anchorMin, Vector2 anchorMax,
        int fontSize, FontStyle style, TextAnchor alignment, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = Vector2.zero;
        Text text = go.AddComponent<Text>();
        text.text = content;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.fontStyle = style;
        text.alignment = alignment;
        text.color = color;
        return text;
    }

    static GameObject CreateButton(Transform parent, string name,
        Vector2 anchorMin, Vector2 anchorMax,
        string label, Color bgColor)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = Vector2.zero;
        Image img = go.AddComponent<Image>();
        img.color = bgColor;
        Button btn = go.AddComponent<Button>();
        btn.targetGraphic = img;

        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(go.transform, false);
        RectTransform textRT = textGO.AddComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.anchoredPosition = Vector2.zero;
        textRT.sizeDelta = Vector2.zero;
        Text text = textGO.AddComponent<Text>();
        text.text = label;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 20;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;

        return go;
    }

    static GameObject CreateInputField(Transform parent, string name,
        Vector2 anchorMin, Vector2 anchorMax,
        string placeholder)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = Vector2.zero;
        Image img = go.AddComponent<Image>();
        img.color = new Color(0.1f, 0.12f, 0.16f);

        // Text child
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(go.transform, false);
        RectTransform textRT = textGO.AddComponent<RectTransform>();
        textRT.anchorMin = new Vector2(0.05f, 0);
        textRT.anchorMax = new Vector2(0.95f, 1);
        textRT.anchoredPosition = Vector2.zero;
        textRT.sizeDelta = Vector2.zero;
        Text text = textGO.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 18;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleLeft;
        text.supportRichText = false;

        // Placeholder child
        GameObject placeholderGO = new GameObject("Placeholder");
        placeholderGO.transform.SetParent(go.transform, false);
        RectTransform phRT = placeholderGO.AddComponent<RectTransform>();
        phRT.anchorMin = new Vector2(0.05f, 0);
        phRT.anchorMax = new Vector2(0.95f, 1);
        phRT.anchoredPosition = Vector2.zero;
        phRT.sizeDelta = Vector2.zero;
        Text phText = placeholderGO.AddComponent<Text>();
        phText.text = placeholder;
        phText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        phText.fontSize = 18;
        phText.fontStyle = FontStyle.Italic;
        phText.color = new Color(0.5f, 0.5f, 0.5f);
        phText.alignment = TextAnchor.MiddleLeft;

        InputField input = go.AddComponent<InputField>();
        input.textComponent = text;
        input.placeholder = phText;
        input.characterLimit = 30;

        return go;
    }

    static void AddSceneToBuildSettings(string scenePath)
    {
        var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);

        // Check if already added
        foreach (var s in scenes)
        {
            if (s.path == scenePath) return;
        }

        // Find MainMenu index and insert Lobby right after it
        int mainMenuIndex = -1;
        for (int i = 0; i < scenes.Count; i++)
        {
            if (scenes[i].path.Contains("MainMenu"))
            {
                mainMenuIndex = i;
                break;
            }
        }

        EditorBuildSettingsScene lobbyScene = new EditorBuildSettingsScene(scenePath, true);

        if (mainMenuIndex >= 0)
        {
            scenes.Insert(mainMenuIndex + 1, lobbyScene);
        }
        else
        {
            scenes.Add(lobbyScene);
        }

        EditorBuildSettings.scenes = scenes.ToArray();
    }
}
