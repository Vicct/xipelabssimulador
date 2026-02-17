using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

/// <summary>
/// Fixes the RoleCard prefab layout so that:
/// - Profession name is at the TOP
/// - Icon is in the middle (smaller)
/// - Salary text is visible below the icon
/// - Description is at the bottom
///
/// The grid cell size is 450x280, so all elements must fit within that height.
/// Run from Unity menu: Tools > Finance Game > Fix RoleCard Prefab
/// </summary>
public class FixRoleCardPrefab : Editor
{
    [MenuItem("Tools/Finance Game/Fix RoleCard Prefab")]
    static void FixPrefab()
    {
        string prefabPath = "Assets/PreFabs/UI/RoleCard.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        if (prefab == null)
        {
            Debug.LogError($"RoleCard prefab not found at {prefabPath}");
            return;
        }

        // Open prefab for editing
        string assetPath = AssetDatabase.GetAssetPath(prefab);
        GameObject prefabRoot = PrefabUtility.LoadPrefabContents(assetPath);

        Debug.Log("Fixing RoleCard prefab layout...");

        RectTransform rootRect = prefabRoot.GetComponent<RectTransform>();
        // Match the grid cell size
        rootRect.sizeDelta = new Vector2(450, 280);
        Debug.Log("  [OK] Root size: 450x280");

        // Fix each child element
        // Layout (top to bottom within 280px height):
        // - ProfessionNameText: y=-25, h=35 (top area)
        // - IconImage:          y=-100, 100x100 (center area)
        // - SalaryText:         y=-175, h=30 (below icon)
        // - DescriptionText:    y=-220, h=50 (bottom area)

        Transform nameTextT = prefabRoot.transform.Find("ProfessionNameText");
        if (nameTextT != null)
        {
            RectTransform rt = nameTextT.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(0.5f, 1);
            rt.anchoredPosition = new Vector2(0, -5);
            rt.sizeDelta = new Vector2(-20, 35); // stretch width with 10px padding each side
            Debug.Log("  [OK] ProfessionNameText: top, y=-5");

            Text nameText = nameTextT.GetComponent<Text>();
            if (nameText != null)
            {
                nameText.fontSize = 26;
                nameText.fontStyle = FontStyle.Bold;
                nameText.alignment = TextAnchor.MiddleCenter;
                nameText.color = new Color(0.15f, 0.15f, 0.15f, 1f);
            }
        }

        Transform iconT = prefabRoot.transform.Find("IconImage");
        if (iconT != null)
        {
            RectTransform rt = iconT.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 1);
            rt.anchorMax = new Vector2(0.5f, 1);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(0, -105);
            rt.sizeDelta = new Vector2(110, 110);
            Debug.Log("  [OK] IconImage: center, 110x110, y=-105");
        }

        Transform salaryTextT = prefabRoot.transform.Find("SalaryText");
        if (salaryTextT != null)
        {
            RectTransform rt = salaryTextT.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(0.5f, 1);
            rt.anchoredPosition = new Vector2(0, -170);
            rt.sizeDelta = new Vector2(-20, 30);
            Debug.Log("  [OK] SalaryText: y=-170, visible");

            Text salaryText = salaryTextT.GetComponent<Text>();
            if (salaryText != null)
            {
                salaryText.fontSize = 22;
                salaryText.fontStyle = FontStyle.Bold;
                salaryText.alignment = TextAnchor.MiddleCenter;
                salaryText.color = new Color(0.18f, 0.8f, 0.44f, 1f); // Green
            }
        }

        Transform descTextT = prefabRoot.transform.Find("DescriptionText");
        if (descTextT != null)
        {
            RectTransform rt = descTextT.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(1, 0);
            rt.pivot = new Vector2(0.5f, 0);
            rt.anchoredPosition = new Vector2(0, 5);
            rt.sizeDelta = new Vector2(-20, 65);
            Debug.Log("  [OK] DescriptionText: bottom, y=5");

            Text descText = descTextT.GetComponent<Text>();
            if (descText != null)
            {
                descText.fontSize = 16;
                descText.alignment = TextAnchor.UpperCenter;
                descText.color = new Color(0.3f, 0.3f, 0.3f, 1f);
            }
        }

        // Reorder children: Name first (index 0), then Icon, Salary, Description
        if (nameTextT != null) nameTextT.SetSiblingIndex(0);
        if (iconT != null) iconT.SetSiblingIndex(1);
        if (salaryTextT != null) salaryTextT.SetSiblingIndex(2);
        if (descTextT != null) descTextT.SetSiblingIndex(3);

        // Fix root scale (was 0.97497 in Y for some reason)
        rootRect.localScale = Vector3.one;

        // Save prefab
        PrefabUtility.SaveAsPrefabAsset(prefabRoot, assetPath);
        PrefabUtility.UnloadPrefabContents(prefabRoot);

        Debug.Log("[OK] RoleCard prefab fixed! Layout: Name(top) > Icon > Salary > Description(bottom)");
    }
}
