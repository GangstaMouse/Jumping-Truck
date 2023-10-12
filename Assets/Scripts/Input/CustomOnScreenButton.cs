// Почему-то если раскомментировать это... то не будет работать intellisense
// #if UNITY_INPUT_SYSTEM_ENABLE_UI
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

[AddComponentMenu("Input/Custom On-Screen Button")]
public class CustomOnScreenButton : OnScreenControl, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private bool minusOne = false;

    public void OnPointerUp(PointerEventData eventData)
    {
        SendValueToControl(0.0f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // SendValueToControl(1.0f);
        SendValueToControl((minusOne ? -1.0f : 1.0f));
    }

    [InputControl(layout = "Button")]
    [SerializeField] private string m_ControlPath;

    protected override string controlPathInternal
    {
        get => m_ControlPath;
        set => m_ControlPath = value;
    }
}
// #endif
