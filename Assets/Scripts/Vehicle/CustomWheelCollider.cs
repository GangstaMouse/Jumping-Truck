using UnityEngine;
using Unity.Mathematics;

[AddComponentMenu("Vehicle/Custom Wheel Collider")]
public class CustomWheelCollider : ShaftComponent
{
    // Parameters
    // Suspension
    [field: SerializeField] public float MaxSuspensionLenght { get; internal set; } = 0.6f;
    [field: SerializeField] public float SpringStiffness { get; internal set; }
    [field: SerializeField] public float DamperStiffness { get; internal set; }
    [field: SerializeField] public LayerMask CollisionLayer { get; internal set; } = -1;

    // Wheel
    [field: SerializeField] public float MaxWheelSteerAngle { get; internal set; }
    [field: SerializeField] public float Radius { get; internal set; } = 0.2f;
    [field: SerializeField] public float WheelWidth { get; internal set; } = 0.16f;
    [field: SerializeField] public float WheelMass { get; internal set; } = 7.2f;

    // Brake
    [field: SerializeField] public float MaxBrakeTorque { get; internal set; }
    [field: SerializeField] public float MaxHandbrakeTorque { get; internal set; }

    // Tire
    [field: SerializeField] public float2 TireStiffnes { get; internal set; } = new(0.87f, 0.87f);

    private Rigidbody m_RigidBody;

    // Wheel
    public float WheelInertia { get; private set; }
    public bool HasContact { get; private set; }
    public float WheelAngularVelocity { get; private set; }
    private float3 m_WheelLocalLinearVelocity;

    // Suspension
    public float CurrentSuspensionLenght { get; private set; }
    // private float suspensionSpeed;
    private float springCompressionValue;
    private float springForce;
    private float dampingForce;
    private float suspensionForce;

    // Tire
    private Vector2 tireSlipVelocity;
    private Vector2 tireSlipVelocityNormalized;
    private Vector2 tireGripForce;

    public float wheelAngularAcceleration;

    float m_CachedSuspensionLenght;

    public float2 CachedTireFrictionForce { get; private set; }

    public float wheelSteerValue;
    public float BrakeValue;
    public float HandbrakeValue;

    private void Awake()
    {
        m_RigidBody = GetComponentInParent<Rigidbody>();

        m_CachedSuspensionLenght = CurrentSuspensionLenght;
        WheelInertia = Mathf.Pow(Radius, 2f) * (WheelMass / 2f);

    }

    public void OnPhysicsTick(float deltaTime)
    {
        if (MaxWheelSteerAngle != 0.0f)
            transform.localRotation = quaternion.AxisAngle(math.up(), math.radians(wheelSteerValue * MaxWheelSteerAngle));

        Transform wheelTransform = transform;

        #region Suspension Calculations
        float3 rayStart = wheelTransform.position;
        float3 rayDirection = -wheelTransform.up;
        float3 rayEnd = rayStart + (rayDirection * (MaxSuspensionLenght + Radius));

        bool surfaceDetected = Physics.Linecast(rayStart, rayEnd, out RaycastHit hitResult, CollisionLayer, QueryTriggerInteraction.Ignore);
        HasContact = surfaceDetected;

        suspensionForce = 0.0f;

        if (HasContact)
        {
            CurrentSuspensionLenght = math.max(hitResult.distance - Radius, 0.0f);

            springCompressionValue = 1.0f - (CurrentSuspensionLenght / MaxSuspensionLenght);
            springForce = SpringStiffness * springCompressionValue;

            float springCompressionSpeed = (m_CachedSuspensionLenght - CurrentSuspensionLenght) / deltaTime;
            dampingForce = DamperStiffness * springCompressionSpeed; // bad!!! appying when wheel dont have any contact

            suspensionForce = springForce + dampingForce;

            float3 suspensionForceVector = wheelTransform.up * suspensionForce;
            m_RigidBody.AddForceAtPosition(suspensionForceVector * deltaTime, wheelTransform.position, ForceMode.Impulse);
        }
        else
        {
            CurrentSuspensionLenght = MaxSuspensionLenght;
        }

        m_CachedSuspensionLenght = CurrentSuspensionLenght;
        #endregion

        #region Wheel Calculations
        float3 wheelContactPoint = wheelTransform.position - (wheelTransform.up * (CurrentSuspensionLenght + Radius));
        m_WheelLocalLinearVelocity = float3.zero;
        float2 cachedTireSlipVelocity = CachedTireFrictionForce;
        float2 tireFrictionForce = float2.zero;

        // DriveShaft acceleration (test, remake)
        WheelAngularVelocity += wheelAngularAcceleration;

        // reset acceleration
        wheelAngularAcceleration = 0.0f;

        if (HasContact)
        {
            // TODO: Add ability to stay on moving surfaces
            float3 wheelWorldLinearVelocity = m_RigidBody.GetPointVelocity(wheelContactPoint); // relative to surface velocity // test
            m_WheelLocalLinearVelocity = wheelTransform.InverseTransformDirection(wheelWorldLinearVelocity);
            // Difference in wheel linear speed
            float forwardLinearSlip = m_WheelLocalLinearVelocity.z - (WheelAngularVelocity * Radius);

            // Surface friction // TODO: need more parameters, like spiked tires, tire pressure, tire width
            float surfaceFriction = hitResult.collider.material.dynamicFriction;
            float2 totalFriction = TireStiffnes * surfaceFriction;

            float2 tfm = new(totalFriction.x == 0.0f ? 0.0f : 1.0f / totalFriction.x,
                totalFriction.y == 0.0f ? 0.0f : 1.0f / totalFriction.y);

            // ex: max friction 2, speed 3, ranged = 3 / 2 = 1.5, then clamp this to 1, and up to max friction, = 2
            float2 tireSlipVelocityRanged = (cachedTireSlipVelocity + new float2(m_WheelLocalLinearVelocity.x, forwardLinearSlip)) * tfm;
            tireFrictionForce = (math.length(tireSlipVelocityRanged) > 1.0f ? math.normalize(tireSlipVelocityRanged) : tireSlipVelocityRanged) * totalFriction;
            CachedTireFrictionForce = tireFrictionForce;

            // Acceleration wheel by friction forces
            // try remake, for better physics quality, faster, without delay in 1 frame
            // wheelAngularVelocity = wheelAngularVelocity + (tireSlipVelocityNormalized.y / wheelColliderData.WheelRadius); // maybe need divide by inertia, and maybe not here

            WheelAngularVelocity += tireFrictionForce.y / Radius;
        }
        else
        {
            CachedTireFrictionForce = float2.zero;
            tireSlipVelocity = float2.zero;
        }

        #region Brakes
        // Brake // add handbrake, can be unified by reusing resistance torque
        float maxBrakeTorque = BrakeValue * MaxBrakeTorque;
        float maxHandbrakeTorque = HandbrakeValue * MaxHandbrakeTorque;
        float brakesTorqueCapacity = (maxBrakeTorque + maxHandbrakeTorque) * deltaTime;

        float wheelAngularResistance = brakesTorqueCapacity;

        float appliedAngularResistance = math.min(math.abs(WheelAngularVelocity), wheelAngularResistance);
        WheelAngularVelocity -= appliedAngularResistance * math.sign(WheelAngularVelocity);
        #endregion
        #endregion

        #region Tire Calculations
        if (HasContact)
        {
            float2 tireGripForce = -tireFrictionForce * math.max(suspensionForce, 0.0f);

            float3 xPlane = (float3)wheelTransform.right - math.project(wheelTransform.right, hitResult.normal);
            float3 zPlane = (float3)wheelTransform.forward - math.project(wheelTransform.forward, hitResult.normal);

            float3 tireGripForceVector = (xPlane * tireGripForce.x) + (zPlane * tireGripForce.y);

            m_RigidBody.AddForceAtPosition(tireGripForceVector * deltaTime, wheelContactPoint, ForceMode.Impulse);
        }
        #endregion
    }

    public override void Stream(in float inputVelocity, in float inputTorque, out float outputVelocity, out float outputTorque)
    {
        float rawAcceleration = inputTorque / WheelInertia * Time.fixedDeltaTime;
        // Это точно не верно, перенести в коробку передач так как передаточные числа работают не верно
        // float dif = 
        float acceleration = Mathf.Min(Mathf.Abs(rawAcceleration), Mathf.Abs(inputVelocity) - Mathf.Abs(WheelAngularVelocity)) * Mathf.Sign(inputTorque); // test.

        wheelAngularAcceleration = acceleration;

        outputVelocity = WheelAngularVelocity;
        // outputTorque = WheelTorque;
        outputTorque = wheelAngularAcceleration;
    }

    [ContextMenu("Optimize wheel sized")]
    private void OptimizeWheelSizes()
    {
        GameObject rimObject = transform.Find("Rim").gameObject;
        GameObject tireObject = rimObject.transform.Find("Tire").gameObject;
        MeshFilter tireMeshFilter = tireObject.GetComponent<MeshFilter>();
        Bounds tireBounds = tireMeshFilter.sharedMesh.bounds;
        Radius = math.max(tireBounds.extents.z, tireBounds.extents.y);
        WheelWidth = tireBounds.size.x;
    }
#if UNITY_EDITOR

    private void OnValidate()
    {
        CurrentSuspensionLenght = MaxSuspensionLenght;
    }

    private void OnDrawGizmos()
    {
        Vector3 wheelCenterPoint = transform.position - (transform.up * CurrentSuspensionLenght);

        // Draw wheel bounds
        Gizmos.color = Color.green;
        GizmosLibrary.DrawWiredCylinder(wheelCenterPoint, transform.right, transform.up, Radius, WheelWidth);

        if (!Application.isPlaying)
            DrawPreplayModeGizmos();
        else
            DrawPlayModeGizmos();
    }

    private void DrawPreplayModeGizmos()
    {
        Vector3 wheelCenterPoint = transform.position - (transform.up * MaxSuspensionLenght);

        // Vizualize suspension lenght + wheel radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, wheelCenterPoint);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(wheelCenterPoint, wheelCenterPoint - (transform.up * Radius));
    }

    private void DrawPlayModeGizmos()
    {
        Gizmos.color = HasContact ? Color.Lerp(Color.green, Color.red, springCompressionValue) : Color.grey;
        Gizmos.DrawLine(transform.position, transform.position - (transform.up * CurrentSuspensionLenght));
        Gizmos.DrawWireSphere(transform.position - (transform.up * CurrentSuspensionLenght), Radius / 4f);

        // Draw tire grip force line gizmo
        /* Vector3 contactPoint = transform.position - (transform.up * (springLenght + WheelRadius));
        Vector3 lineDirection = (transform.right * -tireSlipVelocityNormalized.x) + (transform.forward * -tireSlipVelocityNormalized.y);
        Gizmos.color = Color.Lerp(Color.blue, Color.red, Mathf.Abs(Vector2.Dot(Vector2.right, tireSlipVelocityNormalized)));
        Gizmos.DrawLine(contactPoint, contactPoint + lineDirection); */
    }
#endif
}
