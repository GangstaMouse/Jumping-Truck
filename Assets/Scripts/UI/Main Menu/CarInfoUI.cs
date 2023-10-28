using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// VehicleCharacteristicsCalculator
public class CarInfoUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private float topSpeedMaxValue = 210f;
    [SerializeField] private float accelerationMaxValue = 5000f;
    [SerializeField] private float weightMaxValue = 5000f;
    // [SerializeField] private float topSpeedMaxValue;
    // [SerializeField] private float topSpeedMaxValue;
    
    [SerializeField] private TMP_Text speedText;
    [SerializeField] private Slider speedBar;
    [SerializeField] private TMP_Text accelerationText;
    [SerializeField] private Slider accelerationBar;
    [SerializeField] private TMP_Text suspensionText;
    [SerializeField] private Slider suspensionBar;
    [SerializeField] private TMP_Text handlingText;
    [SerializeField] private Slider handlingBar;
    [SerializeField] private TMP_Text weightText;
    [SerializeField] private Slider weightBar;

    // private void Awake() => GetComponentInParent<CarsMenu>().OnSlotHighlighted += SetInfo;

    private float GetCarTopSpeed(Vehicle carController)
    {
        Engine engine = carController.GetComponent<Engine>();
        Gearbox gearbox = carController.GetComponent<Gearbox>();
        Differential differential = carController.GetComponent<Differential>();

        float topAngularVelocity = (engine.MaxRPM * Engine.RPMToRad) / (gearbox.MainGearRatio * gearbox.GearRatios[gearbox.GearRatios.Count - 1] * differential.GearRatio);
        // Km/h
        float topLinearVelocity = topAngularVelocity * 3.6f;

        return FunctionsLibrary.MapRangeClamped(topAngularVelocity, topSpeedMaxValue, 0f, 10f, 0f);
    }

    private float GetCarAcceleration(Vehicle carController)
    {
        Engine engine = carController.GetComponent<Engine>();
        Gearbox gearbox = carController.GetComponent<Gearbox>();
        Differential differential = carController.GetComponent<Differential>();

        // float angularAcceleration = engine.TorqueCurve.keys[1].value * (gearbox.MainGear * gearbox.GearRatios[2]) * differential.Ratio;
        throw new System.NotImplementedException();
        float angularAcceleration = 1 * (gearbox.MainGearRatio * gearbox.GearRatios[2]) * differential.GearRatio; // temp!

        return FunctionsLibrary.MapRangeClamped(angularAcceleration, accelerationMaxValue, 0f, 10f, 0f);
    }

    private float GetCarWeight(Vehicle carController)
    {
        float weight = carController.GetComponent<Rigidbody>().mass;

        return FunctionsLibrary.MapRangeClamped(weight, weightMaxValue, 0f, 10f, 0f);
    }

    public void UpdateInfo(Vehicle carController)
    {
        float topSpeed = GetCarTopSpeed(carController);
        speedText.SetText($"Top Speed {topSpeed.ToString("F1")}");
        speedBar.value = topSpeed;

        float acceleration = GetCarAcceleration(carController);
        accelerationText.SetText($"Acceleration {acceleration.ToString("F1")}");
        accelerationBar.value = acceleration;

        float weight = GetCarWeight(carController);
        weightText.SetText($"Weight {weight.ToString("F1")}");
        weightBar.value = weight;
    }

    private void SetInfo(CarDataSO carData)
    {
        speedText.SetText($"Speed {carData.Speed.ToString("F1")}");
        speedBar.value = carData.Speed;
        accelerationText.SetText($"Acceleration {carData.Acceleration.ToString("F1")}");
        accelerationBar.value = carData.Acceleration;
        suspensionText.SetText($"Suspension {carData.Suspension.ToString("F1")}");
        suspensionBar.value = carData.Suspension;
        handlingText.SetText($"Handling {carData.Handling.ToString("F1")}");
        handlingBar.value = carData.Handling;
        weightText.SetText($"Weight {carData.Weight.ToString("F1")}");
        weightBar.value = carData.Weight;
    }
}
