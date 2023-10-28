using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CarCameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    private Vehicle carController;

    private void Awake() => carController = GetComponent<Vehicle>();

/*     private void OnEnable() => carController.OnControllerChanged += ChangeCamera;

    private void OnDisable() => carController.OnControllerChanged -= ChangeCamera;

    private void ChangeCamera(Controller controller) => virtualCamera.gameObject.SetActive(controller == Controller.Player); */
}
