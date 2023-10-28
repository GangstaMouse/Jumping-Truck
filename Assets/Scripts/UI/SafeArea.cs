using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeArea : MonoBehaviour
{
    private ScreenOrientation m_CurrentOrientation = ScreenOrientation.AutoRotation;

    private void Awake() => RefreshSafeArea();

    private void RefreshSafeArea()
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

        m_CurrentOrientation = Screen.orientation;
    }

    private void Update()
    {
        // Возможно нужно будет добавить проверку на несоответствие safeArea для складных смартфонов
        if (m_CurrentOrientation != Screen.orientation)
            RefreshSafeArea();
    }
}
