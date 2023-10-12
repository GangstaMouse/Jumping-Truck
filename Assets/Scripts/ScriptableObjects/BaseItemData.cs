using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseItemData : ScriptableObject
{
    // Перенести в другое место? Или нет. Для уровней и трасс вполне ещё может сгодиться
    // Добавить поле для наименования предметов, и сцен с трассами
    // Добавить условие получения
    [Tooltip("level_'value' - item can be unlocked at the specified player level. NOT IMPLEMENTED: ads_'value' - item can be unlocked ad the specified number of ad views")]
    [SerializeField] private string unlockRequirements;
    public string UnlockRequirements => unlockRequirements;
    // protected string assetPath

    [SerializeField] private string id;
    public string ID => id;

    #if UNITY_EDITOR
    [ContextMenu("Generate ID")]
    private void GenerateID() => id = System.Guid.NewGuid().ToString();
    #endif

    protected const string defaultAssetPath = null;

    // Запретить в дочерних классах
    protected static T GetAssetByID<T>(string id, string assetPath = defaultAssetPath) where T : BaseItemData
    {
        foreach (var dataObject in Resources.LoadAll<T>(assetPath))
        {
            // Debug.LogWarning(dataObject);
            if (dataObject.ID == id)
                return dataObject;
        }

        return null;
    }
}
