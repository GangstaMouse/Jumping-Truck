using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileControls : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject steeringButtons;
    [SerializeField] private GameObject buttonPedals;

    [SerializeField] private GameObject steeringWheel;
    [SerializeField] private GameObject sliderPedals;

    private void Awake()
    {
        // Steering
        switch (OptionsSavesManager.SaveData.SteeringType)
        {
            default:
                steeringButtons.SetActive(true);
                steeringWheel.SetActive(false);
                break;

            case 1:
                steeringButtons.SetActive(false);
                steeringWheel.SetActive(true);
                break;
        }

        // Pedals
        switch (OptionsSavesManager.SaveData.PedalsType)
        {
            default:
                buttonPedals.SetActive(true);
                sliderPedals.SetActive(false);
                break;

            case 1:
                buttonPedals.SetActive(false);
                sliderPedals.SetActive(true);
                break;
        }
    }
}
