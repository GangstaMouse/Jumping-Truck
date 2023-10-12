using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitchCorrection : MonoBehaviour
{
    [SerializeField] private Vector3 Resistance = Vector3.one * 2f;
    [SerializeField] private Vector3 MaxResistance = Vector3.one * 10f;

    private new Rigidbody rigidbody;
    private List<WheelCollider> wheels = new List<WheelCollider>();

    private Vector3 localAngularVelocity;
    private Vector3 correctionForce;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        wheels = new List<WheelCollider>(GetComponentsInChildren<WheelCollider>());
    }

    private void FixedUpdate()
    {
        localAngularVelocity = transform.InverseTransformDirection(rigidbody.angularVelocity);

        if (DoesCarGrounded())
            return;

        correctionForce.x = Mathf.Min(Mathf.Abs(localAngularVelocity.x) * Resistance.x, MaxResistance.x) * -Mathf.Sign(localAngularVelocity.x);
        correctionForce.y = Mathf.Min(Mathf.Abs(localAngularVelocity.y) * Resistance.y, MaxResistance.y) * -Mathf.Sign(localAngularVelocity.y);
        correctionForce.z = Mathf.Min(Mathf.Abs(localAngularVelocity.z) * Resistance.z, MaxResistance.z) * -Mathf.Sign(localAngularVelocity.z);

        rigidbody.AddRelativeTorque(correctionForce, ForceMode.Acceleration);
    }

    private bool DoesCarGrounded()
    {
        foreach (var wheel in wheels)
            if (wheel.grounded)
                return true;

        return false;
    }
}
