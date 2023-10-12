using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrakelightEffect : MonoBehaviour
{
    // [SerializeField] private Material material;

    private CarController carController;
    private Material materialInstance;

    private void Awake()
    {
        carController = GetComponentInParent<CarController>();
        materialInstance = GetComponent<MeshRenderer>().material;
    }

    private void OnEnable() => carController.OnControlsInputUpdate += SetIntencity;

    private void OnDisable() => carController.OnControlsInputUpdate -= SetIntencity;

    private void SetIntencity() => materialInstance.SetFloat("Intensity", carController.BrakeInput);
}
