using UnityEngine;

[CreateAssetMenu(menuName = "Resources/Scene Data")]
public class SceneData : ItemDataSO
{
    [field: SerializeField] public string SceneName { get; private set; } = "None";
    [field: SerializeField] public Sprite PreviewSprite { get; private set; }
    [field: SerializeField] public bool Unlocked { get; private set; } = false;

    public override string ResourceFolderName => "Levels";
}
