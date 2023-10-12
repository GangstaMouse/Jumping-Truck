using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    // Controller
    [Header("Control")]
    public Controller controller;

    // Показывать если контроллер != Disabled/None/Empty
    public bool InputEnabled;

    private PlayerInputActions playerInput;

    // Speed
    [Header("Control Speed")]
    [Range(0f, 10f)]
    public float GasSpeed = 5f;
    [Range(0f, 10f)]
    public float SteerSpeed = 5f;
    [Range(0f, 10f)]
    public float BrakeSpeed = 5f;
    [Range(0f, 10f)]
    public float HandbrakeSpeed = 5f;
    [Range(0f, 10f)]
    public float ClutchSpeed = 5f;

    public enum ControlType { Simple, Realistic }
    public ControlType controlType;
    public event Action<Controller> OnControllerChanged;

    // Input
    // Raw Base
    public float RawGasInput { get; private set; }
    public float RawSteerInput { get; private set; }
    public float RawBrakeInput { get; private set; }
    public bool RawHandbrakeInput { get; private set; }
    public float RawClutchInput { get; private set; }

    // Base
    private float gasInput;
    private float steerInput;
    private float brakeInput;
    private bool handbrakeInput;
    private float clutchInput;

    public float GasInput { get => gasInput; set => gasInput = Mathf.Clamp(value, 0f, 1f); }
    public float SteerInput { get => steerInput; set => steerInput = Mathf.Clamp(value, 0f, 1f); }
    public float BrakeInput { get => brakeInput; set => brakeInput = Mathf.Clamp(value, 0f, 1f); }
    public bool HandbrakeInput { get => handbrakeInput; set => handbrakeInput = value; }
    public float ClutchInput { get => clutchInput; set => clutchInput = Mathf.Clamp(value, 0f, 1f); }

    // Output Value
    public float gasValue { get; private set; }
    public float steerValue { get; private set; }
    public float brakeValue { get; private set; }
    public float handbrakeValue { get; private set; }
    public float clutchValue { get; private set; }

    // Other variables
    private Rigidbody body;
    public Vector3 CenterOfMass;

    public List<WheelCollider> wheels { get; private set; } = new List<WheelCollider>();
    public List<WheelCollider> driveWheels { get; private set; } = new List<WheelCollider>();

    private float physicsDeltaTime;

    // Speed in m/s
    // public Vector3 globalAirSpeed { get; private set; }
    public Vector3 relativeAirSpeed { get; private set; }
    public float driveWheelsSpeed { get; private set; }

    public Engine engine { get; private set; }
    public Clutch clutch { get; private set; }
    public Gearbox gearbox { get; private set; }
    public Differential differential { get; private set; }

    // Camera
    private GameObject cameraGO;
    private List<Camera> cameras;
    private int curCameraIndex;
    private Controller prevController;

    [SerializeField] private GameObject uiPrefab;
    private GameObject uiObject;

    // Sound
    private AudioSource tireSound;

    public event Action OnControlsInputUpdate;
    public event Action OnPrePhysicsUpdate;

    public void OnGas(InputAction.CallbackContext context)
    {
        // throttleInput = context.ReadValue<float>();
        Debug.LogWarning("Test0");
    }

    public void OnBrake(InputValue value)
    {
        // brakeInput = context.ReadValue<float>();
        Debug.Log(value.isPressed);
    }

    public void OnSteer(InputValue input)
    {
        // steerInput = input.Get<float>();
        Debug.Log("gergege");
    }

    // Переделать систему управления, сделать отдельным компонентом
    // Почистить
    private void Awake()
    {
        playerInput = new PlayerInputActions();
        playerInput.Enable();

        body = GetComponent<Rigidbody>();
        body.centerOfMass = CenterOfMass;

        wheels = new List<WheelCollider>(GetComponentsInChildren<WheelCollider>());
        float totalDriveWheelRadius = 0f;
        // fuel = MaxFuel

        foreach (var wheel in wheels)
        {
            wheel.Initialize();

            if (wheel.DriveAxle)
            {
                driveWheels.Add(wheel);
                totalDriveWheelRadius += wheel.radius;
            }
        }

        // Средний
        float driveWheelRadius = totalDriveWheelRadius / driveWheels.Count;

        engine = GetComponent<Engine>();
        clutch = GetComponent<Clutch>();
        gearbox = GetComponent<Gearbox>();
        differential = GetComponent<Differential>();

        engine.Initialize(this, driveWheels);
        // gearbox.Initialize(this, driveWheels, engine);
        gearbox.Initialize(this);

        // Удалить
        // gearbox.radius = middleRadius;
        // tireSound = GetComponent<AudioSource>();

        playerInput.Car.ShiftUp.performed += (context) => StartCoroutine(gearbox.ShiftGear(gearbox.targetGear + 1));
        playerInput.Car.ShiftDown.performed += (context) => StartCoroutine(gearbox.ShiftGear(gearbox.targetGear - 1));
    }

    private void Start() => SetController(controller);

    public void SetController(Controller controller)
    {
        // Controller prevController = this.controller;
        this.controller = controller;

        Destroy(uiObject);

        if (this.controller == Controller.Player)
            uiObject = Instantiate(uiPrefab);
        /* else
            Destroy(uiObject); */

        OnControllerChanged?.Invoke(this.controller);
    }

    public void ChangeController(Controller newController)
    {
        controller = newController;
        // OnControllerChanged?.Invoke(controller);
    }

    /* public void ChangeController(Controller newController)
    {
        // Отключение предыдущего контроллера
        switch (prevController)
        {
            case Controller.Player:
            cameras[curCameraIndex].gameObject.SetActive(false);
            Destroy(FindObjectOfType<UIController>().gameObject);
            // Добавить сброс камер

            // Возможно добавить отключение управления
            break;

            case Controller.AI:
            GetComponent<CarAIBehaviour>().StopBehaviour();
            break;
        }
        prevController = controller;
        controller = newController;

        switch (controller)
        {
            case Controller.Player:
            playerInput = new PlayerInput();
            playerInput.Enable();

            // Загрузка UI Префаба, и создание его
            // Load UI Prefab, and Spawn it
            GameObject uIPrefab = Resources.Load<GameObject>("PlayerUI");
            // Временно
            UIController uIController = Instantiate(uIPrefab).GetComponent<UIController>();
            uIController.ChangeUI(UIType.Car, this);
            // -------

            // Переделать
            playerInput.Car.GearUp.performed += context => StartCoroutine(gearbox.ShiftGear(gearbox.targetGear + 1));
            playerInput.Car.GearDown.performed += context => StartCoroutine(gearbox.ShiftGear(gearbox.targetGear - 1));

            // Добавить делегат на включение камеры для Спавнера Траффика
            // Включение камеры
            cameras = new List<Camera>(GetComponentsInChildren<Camera>(true));
            cameras[curCameraIndex].gameObject.SetActive(true);

            if (cameras.Count > 1)
            {
                playerInput.Car.ChangeCamera.performed += context => ChangeCamera(curCameraIndex + 1);
            }
            break;

            case Controller.AI:
            GetComponent<CarAIBehaviour>().RunBehaviour();
            break;
        }
    } */

    [System.Obsolete]
    public void ChangeCamera(int newCameraIndex)
    {
        // Debug.Log(newCameraIndex);
        if (newCameraIndex == cameras.Count)
        {
            newCameraIndex = 0;
        }

        // Отключать не сам объект, а только камеру (camera component)
        cameras[curCameraIndex].gameObject.SetActive(false);
        cameras[newCameraIndex].gameObject.SetActive(true);

        curCameraIndex = newCameraIndex;
    }

    private void Update()
    {
        if (!InputEnabled)
            return;

        // Упростить
        switch (controller)
        {
            case Controller.Player:
                GetPlayerInput();
                break;

            case Controller.AI:
                // AI Control
                break;
        }
    }

    private void FixedUpdate()
    {
        physicsDeltaTime = Time.fixedDeltaTime;
        ProcessInput(physicsDeltaTime);
        OnControlsInputUpdate?.Invoke();

        relativeAirSpeed = transform.InverseTransformDirection(body.velocity);

        /* engine.EngineUpdate(physicsDeltaTime, gasValue);
        gearbox.clutch = gasValue;
        // Временные параметры
        gearbox.AutomaticProcessor(gasInput, brakeInput);
        // gearbox.ClutchUpdatet(null, physicsDeltaTime, engine.outputTorque, engine.engineAngularVelocity);
        // gearbox.ClutchUpdate(physicsDeltaTime, engine.outputTorque, engine.engineAngularVelocity);
        gearbox.ClutchUpdate(physicsDeltaTime, engine.outputTorque);
        // Differential differential = GetComponent<Differential>();
        differential.TransferOutputTorqueToWheels(gearbox.outputTorque);
        // differential.TransferOutputTorqueToWheels(gearbox.GetOutputTorque(engine.outputTorque));

        foreach (var wheel in wheels)
        {
            // wheel.brakeValue = brakeValue;
            // wheel.steerValue = steerValue; 
            // wheel.handbrakeValue = handbrakeValue;
            wheel.SetInput(steerValue, brakeValue, handbrakeValue);
            wheel.UpdatePhysics(physicsDeltaTime);
        } */

        // Test
        /* gearbox.AutomaticProcessor(gasInput, brakeInput);

        differential.TransferOutputTorqueToWheels(gearbox.GetOutputTorque(clutch.torque * gasInput));

        foreach (var wheel in wheels)
        {
            // wheel.brakeValue = brakeValue;
            // wheel.steerValue = steerValue; 
            // wheel.handBrakeValue = handbrakeValue;
            wheel.SetInput(steerValue, brakeValue, handbrakeValue);
            wheel.UpdatePhysics(physicsDeltaTime);
        }

        clutch.SetTorque(gearbox.GetInputShaftVelocity(differential.GetInputShaftVelocity()), engine.outputTorque);

        // gearbox.ClutchUpdate(physicsDeltaTime, );
        // engine.loadTorque = clutch.torque;
        engine.EngineUpdate(physicsDeltaTime, gasValue); */

        OnPrePhysicsUpdate?.Invoke();
        gearbox.AutomaticProcessor(0f, 0f);

        float gearboxOutputTorque = gearbox.GetOutputTorque(clutch.torque);
        differential.TransferOutputTorqueToWheels(physicsDeltaTime, gearboxOutputTorque);

        foreach (var wheel in wheels)
        {
            wheel.SetInput(steerValue, brakeValue, handbrakeValue);
            wheel.UpdatePhysics(physicsDeltaTime);
        }

        float differentialInputShaftVelocity = differential.GetInputShaftVelocity();
        float gearboxInputShaftVelocity = gearbox.GetInputShaftVelocity(differentialInputShaftVelocity);
        // clutch.SetTorque(gearboxInputShaftVelocity, engine.outputTorque);
        clutch.UpdatePhysics(gearboxInputShaftVelocity, engine.outputTorque, gearbox.totalGearRatio, clutchValue);
        engine.loadTorque = clutch.torque;
        engine.EngineUpdate(physicsDeltaTime, gasValue);
    }

    private void GetPlayerInput()
    {
        RawGasInput = playerInput.Car.Gas.ReadValue<float>();
        RawBrakeInput = playerInput.Car.Brake.ReadValue<float>();
        RawSteerInput = playerInput.Car.Steer.ReadValue<float>();
        RawHandbrakeInput = playerInput.Car.Handbrake.ReadValue<float>() != 0f;
        // rawClutchInput = playerInput.Car.
        RawClutchInput = RawGasInput; // Временно

        switch (controlType)
        {
            case ControlType.Simple:
                bool isReverse = gearbox.targetGear < gearbox.neutralGear;

                gasInput = (isReverse ? RawBrakeInput : RawGasInput);
                brakeInput = (isReverse ? RawGasInput : RawBrakeInput);
                clutchInput = gasInput; // Временно, наверное
                break;

            case ControlType.Realistic:
                gasInput = RawGasInput;
                brakeInput = RawBrakeInput;
                clutchInput = RawClutchInput;
                break;
        }

        steerInput = RawSteerInput;
        handbrakeInput = RawHandbrakeInput;
    }

    private void ProcessInput(float deltaTime)
    {
        // Возможно есть проблема, неуверен что работает правильно
        gasValue = Mathf.LerpUnclamped(gasValue, gasInput, GasSpeed * deltaTime);
        brakeValue = Mathf.LerpUnclamped(brakeValue, brakeInput, BrakeSpeed * deltaTime);
        steerValue = Mathf.LerpUnclamped(steerValue, steerInput, SteerSpeed * deltaTime);
        handbrakeValue = Mathf.LerpUnclamped(handbrakeValue, handbrakeInput ? 1f : 0f, HandbrakeSpeed * deltaTime);
        clutchValue = Mathf.LerpUnclamped(clutchValue, clutchInput, ClutchSpeed * deltaTime);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // Перенести в CustomEditor
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireSphere(CenterOfMass, 0.2f);
    }
#endif
}

public enum Controller {Disabled, Player, AI}
