using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Speedometer : MonoBehaviour
{
    private TMP_Text text;
    private CarController carController;

    private bool visibility;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
        carController = GetComponentInParent<CarController>();
    }

    private void OnEnable() => carController.OnControllerChanged += ChangeVisibility;

    private void OnDisable() => carController.OnControllerChanged -= ChangeVisibility;

    private void ChangeVisibility(Controller controller) => SetVisibility(controller == Controller.Player);

    public void SetVisibility(bool newVisibility)
    {
        visibility = newVisibility;
        gameObject.SetActive(visibility);
    }

    private void LateUpdate()
    {
        float speed = Mathf.FloorToInt(Mathf.Abs(carController.relativeAirSpeed.z) * 3.6f);
        text.SetText($"{speed} km/h");
    }
}
