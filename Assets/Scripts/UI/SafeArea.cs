using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeArea : MonoBehaviour
{
    private ScreenOrientation currentOrientation = ScreenOrientation.AutoRotation;

    private void Awake() => ApplySafeArea();

    private void ApplySafeArea()
    {
        RectTransform rect = GetComponent<RectTransform>();

        Rect safeArea = Screen.safeArea;

        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = anchorMin + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;

        currentOrientation = Screen.orientation;
    }

    private void Update()
    {
        // Возможно нужно будет добавить проверку на несоответствие safeArea для складных смартфонов
        if (currentOrientation != Screen.orientation)
            ApplySafeArea();
    }
}
