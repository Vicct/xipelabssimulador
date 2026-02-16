using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class QuickAssetCreator : MonoBehaviour
{
    public bool createAssets = false;

    void OnValidate()
    {
        if (createAssets)
        {
            createAssets = false;
            CreateAllAssets();
        }
    }

    void CreateAllAssets()
    {
#if UNITY_EDITOR
        Debug.Log("Starting asset creation...");

        // Create GameConfig
        var config = ScriptableObject.CreateInstance<GameConfigData>();
        AssetDatabase.CreateAsset(config, "Assets/Data/Config/GameConfig.asset");
        Debug.Log("✓ GameConfig created");

        // Create Programmer
        var programmer = ScriptableObject.CreateInstance<ProfessionData>();
        AssetDatabase.CreateAsset(programmer, "Assets/Data/Professions/Programmer.asset");
        Debug.Log("✓ Programmer created");

        // Create Doctor
        var doctor = ScriptableObject.CreateInstance<ProfessionData>();
        AssetDatabase.CreateAsset(doctor, "Assets/Data/Professions/Doctor.asset");
        Debug.Log("✓ Doctor created");

        // Create Car Repair Event
        var carRepair = ScriptableObject.CreateInstance<FinancialEventData>();
        AssetDatabase.CreateAsset(carRepair, "Assets/Data/Events/CarRepair.asset");
        Debug.Log("✓ Car Repair created");

        // Create Birthday Party Event
        var birthday = ScriptableObject.CreateInstance<FinancialEventData>();
        AssetDatabase.CreateAsset(birthday, "Assets/Data/Events/BirthdayParty.asset");
        Debug.Log("✓ Birthday Party created");

        // Create Surprise Bonus Event
        var bonus = ScriptableObject.CreateInstance<FinancialEventData>();
        AssetDatabase.CreateAsset(bonus, "Assets/Data/Events/SurpriseBonus.asset");
        Debug.Log("✓ Surprise Bonus created");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("===== ALL ASSETS CREATED SUCCESSFULLY! =====");
        Debug.Log("Check Assets/Data/ folders to see your new assets");
#endif
    }
}