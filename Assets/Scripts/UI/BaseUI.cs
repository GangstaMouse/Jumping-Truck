using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseUI : MonoBehaviour
{
    [SerializeField] private List<BaseUI> m_SubMenus = new();
    [SerializeField] private GameObject m_VisualObject;
    [SerializeField] private bool m_Disable;
    public event Action<BaseUI> OnEnter;
    public event Action OnExit;
    protected List<int> Stack = new();
    private BaseUI m_ActiveSubMenuUI;

    protected virtual void Awake()
    {
        foreach (var subMenu in m_SubMenus)
        {
            subMenu.OnEnter += OnEnterSubMenu;
            subMenu.OnExit += OnExitSubMenu;
        }
    }

    protected BaseUI GetEndMenu()
    {
        if (m_ActiveSubMenuUI != null)
            return m_ActiveSubMenuUI.GetEndMenu();

        return this;
    }

    public virtual void Enter()
    {
        gameObject.SetActive(true);

        if (m_VisualObject)
            m_VisualObject.SetActive(true);

        DrawUI();
        OnEnter?.Invoke(this);
    }

    public virtual void Exit() => GetEndMenu().ExitFromSubMenu();

    protected abstract void DrawUI();

    protected virtual void ExitFromSubMenu()
    {
        // gameObject.SetActive(false);

        if (m_VisualObject)
            m_VisualObject.SetActive(false);

        OnExit?.Invoke();
    }

    private void OnEnterSubMenu(BaseUI subMenu)
    {
        m_ActiveSubMenuUI = subMenu;

        if (m_Disable && m_VisualObject)
            m_VisualObject.SetActive(false);
    }
    private void OnExitSubMenu()
    {
        m_ActiveSubMenuUI = null;

        if (m_VisualObject)
            m_VisualObject.SetActive(true);

        DrawUI();
    }
#if UNITY_EDITOR

    [ContextMenu("Assign sub menus")]
    private void AssignSubMenus()
    {
        m_SubMenus.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform childTransform = transform.GetChild(i);

            if (childTransform.gameObject.TryGetComponent(out BaseUI subMenu))
                m_SubMenus.Add(subMenu);
        }
    }
#endif
}
