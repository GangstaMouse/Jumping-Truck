using UnityEngine;

/* [Serializable]
public struct UnlockRequirements
{
    
} */

public abstract class ItemDataSO : ScriptableObject, IResource//, IUnlockable
{
    // Перенести в другое место? Или нет. Для уровней и трасс вполне ещё может сгодиться
    // Добавить поле для наименования предметов, и сцен с трассами
    // Добавить условие получения
    [Tooltip("level_'value' - item can be unlocked at the specified player level. NOT IMPLEMENTED: ads_'value' - item can be unlocked ad the specified number of ad views")]
    [Multiline][SerializeField] private string m_UnlockRequirements;
    // public string Requirements { get; private set; }
    public string UnlockRequirements => m_UnlockRequirements;
    public abstract string ResourceFolderName { get; }

    [field: SerializeField] public string Identifier { get; private set; }
#if UNITY_EDITOR

    [ContextMenu("Generate ID")]
    private void GenerateID() => Identifier = System.Guid.NewGuid().ToString();

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(Identifier))
            GenerateID();
    }
#endif

    public static T[] LoadAssetsFromPath<T>(string path) where T : ItemDataSO
    {
        return Resources.LoadAll<T>(path);
    }

    public static T LoadAssetFromIdentifier<T>(string identifier, string path) where T : ItemDataSO
    {
        var assets = LoadAssetsFromPath<T>(path);

        foreach (var asset in assets)
            if (asset.Identifier == identifier)
                return asset;

        Debug.LogError($"Asset with identifier <<{identifier}>> at path <<{path}>> not found");
        return null;
        // throw new System.Exception($"Asset with identifier <<{identifier}>> at path <<{path}>> not found");
    }
}
