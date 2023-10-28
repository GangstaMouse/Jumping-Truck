using UnityEngine;

[RequireComponent(typeof(EngineData))]
public class Engine : MonoBehaviour, IInputReceiver
{
    // Parameters
    [field: SerializeField] public float Friction { get; internal set; } = 0.012f;
    [field: SerializeField] public float Inertia { get; internal set; } = 0.08f;
    [field: SerializeField] public float IdleRPM { get; internal set; } = 900f;
    [field: SerializeField] public float MaxRPM { get; internal set; } = 8100f;

    [field: SerializeField] public ShaftComponent Output { get; internal set; }

    // internal variables
    public float AngularVelocity { get; private set; }
    public float CurrentRPM { get; private set; }
    public float ThrottleValue { get; private set; }
    private float m_load = 0f;

    // Const parameters
    public const float RPMToRad = Mathf.PI * 2f / 60f;
    public const float RadToRPM = 1f / RPMToRad;

    // Input handler
    InputHandler IInputReceiver.InputHandler { get => InputHandler; set => InputHandler = value; }
    public InputHandler InputHandler { get; private set; } = new();

    private EngineData m_EngineData;

    private void Awake()
    {
        m_EngineData = GetComponent<EngineData>();
        m_EngineData.IdleRPM = IdleRPM;
        m_EngineData.MaxRPM = MaxRPM;
    }

    private void Start()
    {
        CurrentRPM = IdleRPM;
        AngularVelocity = CurrentRPM * RPMToRad;
    }

    private void FixedUpdate()
    {
        float deltaTime = Time.fixedDeltaTime;
        ThrottleValue = InputHandler.GasInput;

        float frictionTorque = Friction * CurrentRPM;
        float t = ((26f + frictionTorque) * ThrottleValue) - frictionTorque;
        AngularVelocity = Mathf.Clamp((AngularVelocity - m_load) + ((t / Inertia) * deltaTime), IdleRPM * RPMToRad, MaxRPM * RPMToRad);
        CurrentRPM = AngularVelocity * RadToRPM;

        Output.Stream(AngularVelocity, AngularVelocity, out float outputVelocity, out float outputTorque);
        m_load = outputTorque;
        // m_load = Output.Shaft(AngularVelocity, AngularVelocity, 0f);

        m_EngineData.CurRPM = CurrentRPM;
    }

    void IInputReceiver.OnInputHandlerChanger(in InputHandler inputHandler)
    {

    }
}
