using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Resources/Scene Data")]
public class SceneData : BaseItemData
{
    public string SceneName = "None";
    public Sprite PreviewSprite;
    public bool Unlocked;
}
