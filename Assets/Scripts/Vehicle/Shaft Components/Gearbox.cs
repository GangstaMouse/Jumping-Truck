using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(EngineData))]
public class Gearbox : ShaftComponent, IInputReceiver
{
    // Parameters
    [field: SerializeField] public List<float> GearRatios { get; internal set; } = new();
    [field: SerializeField] public float MainGearRatio { get; internal set; } = 1f;
    [field: SerializeField] public float GearChangeTime { get; internal set; } = 0.37f;
    [field: SerializeField] public ShaftComponent Output { get; internal set; }

    // Private parameters
    public int CurrentGear { get; internal set; }
    public int NeutralGear { get; internal set; }
    InputHandler IInputReceiver.InputHandler { get => InputHandler; set => InputHandler = value; }
    public InputHandler InputHandler { get; private set; } = new();
    private int m_TargetGear;

    private float m_TotalGearRatio;

    public event Action<int> OnChangengGearCompleted;

    private Coroutine m_Coroutine;
    private Vehicle m_Vehicle;
    private EngineData m_EngineData;

    // Initialization
    private void Awake()
    {
        m_EngineData = GetComponent<EngineData>();
    }

    private void Start()
    {
        m_Vehicle = GetComponent<Vehicle>();

        if (GearRatios.Count == 0)
            Debug.LogWarning($"{gameObject.name}: Gearbox gear ratios array size is zero");

        int newNeutralGear = -1;

        for (int i = 0; i < GearRatios.Count; i++)
        {
            if (GearRatios[i] == 0f)
            {
                newNeutralGear = i;
                NeutralGear = i;
                break;
            }
        }

        if (newNeutralGear == -1)
            Debug.LogWarning($"{gameObject.name}: Gearbox gear ratios didn't have neutral gear");

        m_TargetGear = NeutralGear;
        CurrentGear = m_TargetGear;

        m_TotalGearRatio = MainGearRatio * GearRatios[CurrentGear];
    }

    private void FixedUpdate()
    {
        float combinedInput = InputHandler.GasRawInput - InputHandler.BrakeRawInput;
        InputHandler.IsFlipped = m_TargetGear - 1 < 0;

        // better will be starts time for ~one second (hold input), before switching controls
        if (math.sign(m_TargetGear - 1) != math.sign(combinedInput) && math.abs(m_Vehicle.LocalAirSpeed.z) <= 2.0f)
            OnChangeGear(NeutralGear + (int)math.sign(combinedInput));

        if (m_TargetGear <= NeutralGear)
            return;

        if (CurrentGear != m_TargetGear)
            return;

        if (CurrentGear < GearRatios.Count - 1 && m_EngineData.CurRPM >= m_EngineData.MaxRPM * 0.7f)
            OnChangeGear(CurrentGear + 1);
        else if (m_TargetGear > NeutralGear + 1 && m_EngineData.CurRPM <= m_EngineData.IdleRPM * 1.3f)
            OnChangeGear(CurrentGear - 1);
    }

    public override void Stream(in float inputVelocity, in float inputTorque, out float outputVelocity, out float outputTorque)
    {
        if (m_TotalGearRatio == 0)
        {
            outputVelocity = inputVelocity;
            outputTorque = inputTorque;
            return;
        }

        Output.Stream(inputVelocity / m_TotalGearRatio, inputTorque * m_TotalGearRatio,
            out float rawOutputVelocity, out float rawOutputTorque);

        outputVelocity = rawOutputVelocity * m_TotalGearRatio;
        outputTorque = rawOutputTorque / m_TotalGearRatio;
    }

    private void OnChangeGear(int newTargetGear)
    {
        if (newTargetGear < 0 || newTargetGear > GearRatios.Count - 1 || newTargetGear == m_TargetGear)
            return;

        if (m_Coroutine != null)
            StopCoroutine(m_Coroutine);

        m_Coroutine = StartCoroutine(ChangeGear(newTargetGear));
    }

    private IEnumerator ChangeGear(int newTargetGear)
    {
        m_TargetGear = newTargetGear;
        m_TotalGearRatio = 0f;

        yield return new WaitForSeconds(GearChangeTime);

        CurrentGear = m_TargetGear;
        m_TotalGearRatio = MainGearRatio * GearRatios[CurrentGear];
        OnChangengGearCompleted?.Invoke(CurrentGear);
    }

    void IInputReceiver.OnInputHandlerChanger(in InputHandler inputHandler)
    {
        
    }
}
