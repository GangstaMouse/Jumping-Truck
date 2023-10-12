using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Assets/Painting Color")]
// Переименовать в PaintData
public class PaintingColorData : BaseItemData
{
    // Добавить наименования
    public Color Color = Color.white;
    public int Price = 240;

    // Не реализовано
    // public new const string defaultAssetPath = "Painting/Color"
    public new const string defaultAssetPath = "Colors"; // Временно
}
