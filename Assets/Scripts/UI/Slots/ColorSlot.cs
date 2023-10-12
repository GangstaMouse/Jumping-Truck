using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorSlot : BaseItemSlot
{
    public override void Initialize(object data, Action<BaseItemSlot, object> action)
    {
        base.Initialize(data, action);

        PaintingColorData paintingColorData = (PaintingColorData)data;
        GetComponent<Image>().color = paintingColorData.Color;
    }
}
