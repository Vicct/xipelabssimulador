using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// Fixes the EventPopup layout in the GameBoard scene:
/// - Makes the popup taller to fit 3+ choice buttons
/// - Adds ContentSizeFitter to ChoiceContainer
/// - Adds VerticalLayoutGroup with spacing to ChoiceContainer
/// - Makes choice buttons a proper size with padding
/// Run from Unity menu: Tools > Finance Game > Fix EventPopup Layout
/// </summary>
public class FixEventPopup : Editor
{
    [MenuItem("Tools/Finance Game/Fix EventPopup Layout")]
    static void Fix()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/GameBoard.unity");
        Debug.Log("Fixing EventPopup layout...");

        // Find EventPopup component
        EventPopup eventPopup = Object.FindObjectOfType<EventPopup>(true);
        if (eventPopup == null)
        {
            Debug.LogError("EventPopup not found in GameBoard scene!");
            return;
        }

        // ===== Fix PopupPanel size (make taller) =====
        // Get the popupPanel via reflection (it's a serialized private field)
        var popupField = typeof(EventPopup).GetField("popupPanel",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        GameObject popupPanel = popupField?.GetValue(eventPopup) as GameObject;

        if (popupPanel != null)
        {
            RectTransform popupRect = popupPanel.GetComponent<RectTransform>();
            // Expand popup: 15%-85% horizontal, 10%-90% vertical (was 25%-75%)
            popupRect.anchorMin = new Vector2(0.1f, 0.1f);
            popupRect.anchorMax = new Vector2(0.9f, 0.9f);
            popupRect.anchoredPosition = Vector2.zero;
            popupRect.sizeDelta = Vector2.zero;
            Debug.Log("  [OK] PopupPanel: expanded to 80% of screen");

            // Reorganize child elements for better spacing
            // Children order: Icon, EventName, Description, Amount, ChoiceContainer, CloseButton

            // Find Icon
            Transform iconT = popupPanel.transform.Find("EventIcon");
            if (iconT != null)
            {
                RectTransform rt = iconT.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.35f, 0.75f);
                rt.anchorMax = new Vector2(0.65f, 0.95f);
                rt.anchoredPosition = Vector2.zero;
                rt.sizeDelta = Vector2.zero;
                Debug.Log("  [OK] EventIcon: top center 30%");
            }

            // Find EventName
            Transform nameT = popupPanel.transform.Find("EventNameText");
            if (nameT != null)
            {
                RectTransform rt = nameT.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.05f, 0.65f);
                rt.anchorMax = new Vector2(0.95f, 0.75f);
                rt.anchoredPosition = Vector2.zero;
                rt.sizeDelta = Vector2.zero;
                Debug.Log("  [OK] EventNameText: below icon");

                Text nameText = nameT.GetComponent<Text>();
                if (nameText != null)
                {
                    nameText.fontSize = 28;
                    nameText.fontStyle = FontStyle.Bold;
                    nameText.alignment = TextAnchor.MiddleCenter;
                }
            }

            // Find Description
            Transform descT = popupPanel.transform.Find("EventDescriptionText");
            if (descT != null)
            {
                RectTransform rt = descT.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.08f, 0.48f);
                rt.anchorMax = new Vector2(0.92f, 0.65f);
                rt.anchoredPosition = Vector2.zero;
                rt.sizeDelta = Vector2.zero;
                Debug.Log("  [OK] EventDescriptionText: middle area");

                Text descText = descT.GetComponent<Text>();
                if (descText != null)
                {
                    descText.fontSize = 20;
                    descText.alignment = TextAnchor.UpperCenter;
                }
            }

            // Find Amount
            Transform amountT = popupPanel.transform.Find("AmountText");
            if (amountT != null)
            {
                RectTransform rt = amountT.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.15f, 0.38f);
                rt.anchorMax = new Vector2(0.85f, 0.48f);
                rt.anchoredPosition = Vector2.zero;
                rt.sizeDelta = Vector2.zero;
                Debug.Log("  [OK] AmountText: below description");

                Text amountText = amountT.GetComponent<Text>();
                if (amountText != null)
                {
                    amountText.fontSize = 26;
                    amountText.fontStyle = FontStyle.Bold;
                    amountText.alignment = TextAnchor.MiddleCenter;
                }
            }

            // ===== Fix ChoiceContainer =====
            Transform choiceT = popupPanel.transform.Find("ChoiceContainer");
            if (choiceT != null)
            {
                RectTransform rt = choiceT.GetComponent<RectTransform>();
                // Give much more space: 5%-95% horizontal, 5%-38% vertical
                rt.anchorMin = new Vector2(0.05f, 0.05f);
                rt.anchorMax = new Vector2(0.95f, 0.38f);
                rt.anchoredPosition = Vector2.zero;
                rt.sizeDelta = Vector2.zero;
                Debug.Log("  [OK] ChoiceContainer: expanded to 33% of popup height");

                // Fix VerticalLayoutGroup
                VerticalLayoutGroup vlg = choiceT.GetComponent<VerticalLayoutGroup>();
                if (vlg == null)
                {
                    vlg = choiceT.gameObject.AddComponent<VerticalLayoutGroup>();
                }
                vlg.spacing = 8;
                vlg.padding = new RectOffset(10, 10, 5, 5);
                vlg.childAlignment = TextAnchor.MiddleCenter;
                vlg.childControlWidth = true;
                vlg.childControlHeight = true;
                vlg.childForceExpandWidth = true;
                vlg.childForceExpandHeight = false;
                Debug.Log("  [OK] VerticalLayoutGroup: spacing 8, padding 10");

                // Add ContentSizeFitter
                ContentSizeFitter csf = choiceT.GetComponent<ContentSizeFitter>();
                if (csf == null)
                {
                    csf = choiceT.gameObject.AddComponent<ContentSizeFitter>();
                }
                csf.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                Debug.Log("  [OK] ContentSizeFitter: vertical PreferredSize");
            }

            // Find CloseButton
            Transform closeT = popupPanel.transform.Find("CloseButton");
            if (closeT != null)
            {
                RectTransform rt = closeT.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.25f, 0.05f);
                rt.anchorMax = new Vector2(0.75f, 0.15f);
                rt.anchoredPosition = Vector2.zero;
                rt.sizeDelta = Vector2.zero;
                Debug.Log("  [OK] CloseButton: bottom center");

                // Make button text bigger
                Text btnText = closeT.GetComponentInChildren<Text>();
                if (btnText != null)
                {
                    btnText.fontSize = 24;
                    btnText.text = "OK";
                }
            }
        }

        // ===== Fix ChoiceButton prefab =====
        string prefabPath = "Assets/PreFabs/UI/ChoiceButton.prefab";
        GameObject choicePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (choicePrefab != null)
        {
            string assetPath = AssetDatabase.GetAssetPath(choicePrefab);
            GameObject prefabRoot = PrefabUtility.LoadPrefabContents(assetPath);

            RectTransform prefabRect = prefabRoot.GetComponent<RectTransform>();
            prefabRect.sizeDelta = new Vector2(0, 50); // 50px tall buttons

            // Add LayoutElement for proper sizing
            LayoutElement le = prefabRoot.GetComponent<LayoutElement>();
            if (le == null)
            {
                le = prefabRoot.AddComponent<LayoutElement>();
            }
            le.minHeight = 45;
            le.preferredHeight = 50;

            // Fix button text
            Text choiceText = prefabRoot.GetComponentInChildren<Text>();
            if (choiceText != null)
            {
                choiceText.fontSize = 18;
                choiceText.alignment = TextAnchor.MiddleCenter;
                choiceText.horizontalOverflow = HorizontalWrapMode.Wrap;
                choiceText.verticalOverflow = VerticalWrapMode.Truncate;
            }

            PrefabUtility.SaveAsPrefabAsset(prefabRoot, assetPath);
            PrefabUtility.UnloadPrefabContents(prefabRoot);
            Debug.Log("  [OK] ChoiceButton prefab: 50px tall, text wrap enabled");
        }
        else
        {
            Debug.LogWarning($"ChoiceButton prefab not found at {prefabPath}");
        }

        EditorSceneManager.SaveScene(scene);
        Debug.Log("[OK] EventPopup layout fixed! Choices now have room for 3+ buttons.");
    }
}
