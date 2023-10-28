using UnityEngine;
using Cinemachine;

public class GarageMenuUIManager : BaseUI
{
    [SerializeField] private float m_CartSpeed = 2f;

    private CinemachineDollyCart dollyCart;

    protected override void Awake()
    {
        base.Awake();
        dollyCart = FindObjectOfType<CinemachineDollyCart>();
    }

    protected override void DrawUI()
    {
        dollyCart.m_Speed = m_CartSpeed;
    }

    public override void Exit()
    {
        base.Exit();
        dollyCart.m_Speed = -m_CartSpeed;
    }
}
