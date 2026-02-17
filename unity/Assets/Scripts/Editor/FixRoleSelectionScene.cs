using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// Fixes the RoleSelection scene scroll to show all 10 professions.
/// Run from Unity menu: Tools > Finance Game > Fix RoleSelection Scroll
/// </summary>
public class FixRoleSelectionScene : Editor
{
    [MenuItem("Tools/Finance Game/Fix RoleSelection Scroll")]
    static void FixScroll()
    {
        // Open the RoleSelection scene
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/RoleSelection.unity");

        Debug.Log("Fixing RoleSelection scroll...");

        // Find the ScrollRect
        ScrollRect scrollRect = Object.FindObjectOfType<ScrollRect>();
        if (scrollRect == null)
        {
            Debug.LogError("No ScrollRect found in RoleSelection scene!");
            return;
        }

        // Fix ScrollRect settings - vertical only for a list of cards
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.movementType = ScrollRect.MovementType.Elastic;
        scrollRect.elasticity = 0.1f;
        scrollRect.scrollSensitivity = 30f;
        Debug.Log("  [OK] ScrollRect: vertical only, sensitivity 30");

        // Get the Content container (where cards are spawned)
        RectTransform content = scrollRect.content;
        if (content == null)
        {
            Debug.LogError("ScrollRect has no content assigned!");
            return;
        }

        // Set content anchors to top-stretch (expands downward)
        content.anchorMin = new Vector2(0, 1);
        content.anchorMax = new Vector2(1, 1);
        content.pivot = new Vector2(0.5f, 1);

        // Add ContentSizeFitter so it grows with children
        ContentSizeFitter fitter = content.GetComponent<ContentSizeFitter>();
        if (fitter == null)
        {
            fitter = content.gameObject.AddComponent<ContentSizeFitter>();
        }
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        Debug.Log("  [OK] ContentSizeFitter: vertical PreferredSize");

        // Fix or add GridLayoutGroup on Content
        GridLayoutGroup grid = content.GetComponent<GridLayoutGroup>();
        if (grid == null)
        {
            // Maybe it has a VerticalLayoutGroup instead
            VerticalLayoutGroup vlg = content.GetComponent<VerticalLayoutGroup>();
            if (vlg == null)
            {
                grid = content.gameObject.AddComponent<GridLayoutGroup>();
            }
        }

        if (grid != null)
        {
            // 2 columns for 10 profession cards
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 2;
            grid.cellSize = new Vector2(450, 280);
            grid.spacing = new Vector2(20, 20);
            grid.padding = new RectOffset(20, 20, 20, 20);
            grid.childAlignment = TextAnchor.UpperCenter;
            grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
            grid.startAxis = GridLayoutGroup.Axis.Horizontal;
            Debug.Log("  [OK] GridLayoutGroup: 2 columns, 450x280 cells");
        }
        else
        {
            // Has VerticalLayoutGroup - fix it
            VerticalLayoutGroup vlg = content.GetComponent<VerticalLayoutGroup>();
            if (vlg != null)
            {
                vlg.spacing = 15;
                vlg.padding = new RectOffset(20, 20, 20, 20);
                vlg.childAlignment = TextAnchor.UpperCenter;
                vlg.childControlWidth = true;
                vlg.childControlHeight = false;
                vlg.childForceExpandWidth = true;
                vlg.childForceExpandHeight = false;
                Debug.Log("  [OK] VerticalLayoutGroup: spacing 15, padding 20");

                // Add LayoutElement to set preferred height for each card
                // (done at runtime by RoleCard prefab)
            }
        }

        // Make sure the ScrollRect viewport has a mask
        Transform viewport = scrollRect.viewport;
        if (viewport != null)
        {
            Mask mask = viewport.GetComponent<Mask>();
            if (mask == null)
            {
                Image viewportImg = viewport.GetComponent<Image>();
                if (viewportImg == null)
                {
                    viewportImg = viewport.gameObject.AddComponent<Image>();
                    viewportImg.color = new Color(1, 1, 1, 0.01f); // Nearly invisible
                }
                mask = viewport.gameObject.AddComponent<Mask>();
                mask.showMaskGraphic = false;
                Debug.Log("  [OK] Added Mask to viewport");
            }
        }

        // Save
        EditorSceneManager.SaveScene(scene);
        Debug.Log("[OK] RoleSelection scroll fixed! Test with 10 professions.");
    }
}
