using System;
using UnityEngine.UI;

public class ColorSlot : BaseItemSlot
{
    public override void Initialize(object data)
    {
        base.Initialize(data);

        PaintDataSO paintingColorData = (PaintDataSO)data;
        m_DefaultColor = paintingColorData.Color;
        GetComponent<Image>().color = paintingColorData.Color;
    }
}
