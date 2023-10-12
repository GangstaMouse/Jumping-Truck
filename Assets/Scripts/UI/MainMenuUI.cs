using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ScenesMenu scenesMenu;
    [SerializeField] private CarsMenu carsMenu;
    [SerializeField] private OptionsMenuUI optionsMenu;

    public void StartButton()
    {
        scenesMenu.ToggleVisibility();
        // carsMenu.SetVisibility(false);
        optionsMenu.SetVisibility(false);
    }

    public void CarsButton()
    {
        scenesMenu.SetVisibility(false);
        // carsMenu.ToggleVisibility();
        optionsMenu.SetVisibility(false);
    }

    public void OptionsButton()
    {
        scenesMenu.SetVisibility(false);
        // carsMenu.SetVisibility(false);
        optionsMenu.ToggleVisibility();
    }

    public void ExitButton() => Application.Quit();
}
