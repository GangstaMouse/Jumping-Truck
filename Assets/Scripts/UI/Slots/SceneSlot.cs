using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class SceneSlot : BaseItemSlot
{
    [Header("References")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image previewImage;

    // public override void Initialize(object data, Action<BaseItemSlot> action)
    public override void Initialize(object data, Action<BaseItemSlot, object> action)
    {
        base.Initialize(data, action);

        if (this.data is SceneData)
        {
            SceneData sceneData = (SceneData)this.data;

            previewImage.sprite = sceneData.PreviewSprite;

            nameText.SetText(sceneData.SceneName);
        }

    }
}
