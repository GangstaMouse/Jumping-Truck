using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GarageMenu : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private float cartSpeed = 2f;

    /* [Header("Reference")]
    [SerializeField] */ private CinemachineDollyCart dollyCart;

    public bool menuVisibility { get; private set; }

    private void Awake() => dollyCart = FindObjectOfType<CinemachineDollyCart>();

    public void ToggleVisibility() => SetVisibility(!menuVisibility);

    public void SetVisibility(bool newVisibility)
    {
        menuVisibility = newVisibility;
        gameObject.SetActive(menuVisibility);

        dollyCart.m_Speed = (menuVisibility ? cartSpeed : -cartSpeed);
    }
}
