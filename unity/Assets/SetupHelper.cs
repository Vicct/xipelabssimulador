using UnityEngine;

/// <summary>
/// Script helper para configurar la escena MainMenu fácilmente
/// INSTRUCCIONES:
/// 1. Crear GameObject vacío en la escena
/// 2. Renombrarlo a "GameManager"
/// 3. Arrastrar este script al GameObject
/// 4. En Inspector, arrastrar GameConfig.asset al campo
/// 5. Click en botón "Setup Scene" en Inspector
/// 6. Borrar este script cuando termine
/// </summary>
public class SetupHelper : MonoBehaviour
{
    [Header("Config Asset")]
    [Tooltip("Arrastra GameConfig.asset aquí")]
    public GameConfigData gameConfigAsset;

    [Header("Setup Actions")]
    [Tooltip("Marca esto para configurar automáticamente")]
    public bool setupGameManager = false;

    void OnValidate()
    {
        if (setupGameManager)
        {
            setupGameManager = false;
            SetupManagerComponents();
        }
    }

    [ContextMenu("Setup GameManager")]
    void SetupManagerComponents()
    {
        // Verificar que tengamos el config
        if (gameConfigAsset == null)
        {
            Debug.LogError("❌ Necesitas arrastrar GameConfig.asset al campo 'Game Config Asset'");
            return;
        }

        // Asegurarnos que este GameObject se llama GameManager
        if (gameObject.name != "GameManager")
        {
            gameObject.name = "GameManager";
        }

        // Añadir GameManager component si no existe
        GameManager gm = gameObject.GetComponent<GameManager>();
        if (gm == null)
        {
            gm = gameObject.AddComponent<GameManager>();
            Debug.Log("✅ GameManager component añadido");
        }

        // Asignar el config usando reflection (ya que el campo es private SerializeField)
        var field = typeof(GameManager).GetField("gameConfig",
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance);

        if (field != null)
        {
            field.SetValue(gm, gameConfigAsset);
            Debug.Log("✅ GameConfig asignado a GameManager");
        }

        Debug.Log("✅ Setup completado! Ya puedes borrar este SetupHelper script.");
    }

    [ContextMenu("Setup SaveManager")]
    void SetupSaveManager()
    {
        // Buscar si ya existe SaveManager en la escena
        SaveManager existing = FindObjectOfType<SaveManager>();
        if (existing != null)
        {
            Debug.Log("✅ SaveManager ya existe en la escena");
            return;
        }

        // Crear nuevo GameObject para SaveManager
        GameObject smGO = new GameObject("SaveManager");
        smGO.AddComponent<SaveManager>();
        Debug.Log("✅ SaveManager creado");
    }
}
