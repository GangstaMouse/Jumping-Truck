using System;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour, IInputReceiver
{
    // Public parameters
    [field: SerializeField] public Vector3 CenterOfMass { get; internal set; }

    // Private parametes
    private Rigidbody m_RigidBody;
    [field: SerializeField] public List<CustomWheelCollider> WheelColliders { get; private set; } = new();

    InputHandler IInputReceiver.InputHandler { get => InputHandler; set => InputHandler = value; }
    public InputHandler InputHandler { get; private set; } = new();
    public Vector3 LocalAirSpeed { get; private set; }

    private float steerValue;
    private float brakeValue;
    private float handbrakeValue;

    public event Action OnLateUpdateVisualEffects;

    private void Start()
    {
        m_RigidBody = GetComponent<Rigidbody>();
        m_RigidBody.centerOfMass = CenterOfMass;

        WheelColliders = new List<CustomWheelCollider>(GetComponentsInChildren<CustomWheelCollider>());

        if (WheelColliders.Count == 0)
            Debug.LogWarning($"Vehicle named: <{gameObject.name}> didnt have any wheels");
    }

    private void FixedUpdate()
    {
        float deltaTime = Time.fixedDeltaTime;

        foreach (var wheelCollider in WheelColliders)
        {
            wheelCollider.wheelSteerValue = InputHandler.SteerInput;
            wheelCollider.BrakeValue = InputHandler.BrakeInput;
            wheelCollider.HandbrakeValue = InputHandler.HandbrakeInput;

            wheelCollider.OnPhysicsTick(deltaTime);
        }

        LocalAirSpeed = transform.InverseTransformDirection(m_RigidBody.velocity);
    }

    private void LateUpdate()
    {
        OnLateUpdateVisualEffects?.Invoke();
    }

    // Temp
    public void AssignOptimalWheelsParameters()
    {
        // Optimal spring stiffnes
        float springStiffnesPerWheel = (m_RigidBody.mass / WheelColliders.Count) * -Physics.gravity.y * 2f; // * 10f; // * -Physics.gravity.y;
        float damperStiffness = springStiffnesPerWheel / 20; // test
        // выходит что текущая сила пружины и амортизаторов стабилизирует подвеску на середине


        foreach (var wheelCollider in WheelColliders)
        {
            wheelCollider.SpringStiffness = springStiffnesPerWheel;
            wheelCollider.DamperStiffness = damperStiffness;
        }
    }

    void IInputReceiver.OnInputHandlerChanger(in InputHandler inputHandler)
    {
    }
#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + CenterOfMass, 0.3f);
    }
#endif
}
