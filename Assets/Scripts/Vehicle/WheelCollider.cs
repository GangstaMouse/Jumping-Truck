using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TraceType { Line, SemiCylinder }

// Надо бы переделать, и сделать именно колесом: Оставить текущее название, скрипт должен находиться в самом колесе, и добавить 2 точки крепления - рычаг(мост) и точка подвески(то чем сейчас является)
// Переименовать в Suspension
public class WheelCollider : MonoBehaviour
{
    // Static
    private Rigidbody body;

    [Header("Spring")]
    public float MaxLenght;
    public float Stiffnes;
    public float DamperStiffnes;
    // Заменить кривую
    public AnimationCurve SpringCurve;
    public TraceType TraceType;

    // Dynamic
    public float lenght { get; private set; }
    private float unclampedLenght;
    private float previousLength;
    private float suspensionForce;

    private float hardpointDepth;
    private float hardpointPrevDepth;

    [Header("Wheel")]
    /* [Range(-45f, 45f)]
    public float Camber; */
    [Range(0f, 45f)]
    
    public float SteerAngle; // > MaxSteeringAngle
    public float brakeStrenght; // > MaxBrakeTorque
    public float handbrakeStrenght; // > MaxHandbrakeTorque
    public bool DriveAxle; // Больше не нужно
    public float Mass = 10f;

    // Если Semi cylinder trace
    [Range(3, 32)]
    public int BoxesCount = 16;
    [Range(0f, 360f)]
    public float Angle = 45f;
    [Range(0f, 1f)]
    public float AngleOffset;
    // ---
    // Это мера для измежания проблем с потерей касания колеса в SemiCylinderTrace (Но как я полагаю в этом больше нету смысла из за изменения метода касания колеса)
    // Зато можно будет использовать для увеличения силы трения шины
    // Реализовать в LineTrace
    public float MaxDepthPenetration = 0.04f;

    // private float planeWidth => ((radius / (360f / Angle)) * Mathf.PI) / BoxesCount;
    // private Vector2 planeS => new Vector2(width, (radius * Mathf.PI) / BoxesCount);
    
    // Input
    public float steerValue { get; private set; }
    public float brakeValue { get; private set; }
    public float handbrakeValue { get; private set; }

    // Static
    private GameObject rimObject;
    private GameObject tireObject;

    public float radius;
    public float width;
    // public float contactPatch; Реализовать, но как нибудь потом
    public float inertia { get; private set; }

    // Dynamic
    public float wheelSteerAngle { get; private set; } // > steer(ing)Angle
    // private RaycastHit hitResult;
    public bool grounded { get; private set; }
    private Vector3 linearVelocity;
    // Заменить на нижнюю переменную
    public float inputTorque;
    // public float driveTorque; Удалить
    public float angularVelocity { get; private set; }
    private float forwardSlipVelocity;

    private float brakeTorque;
    private float handbrakeTorque;

    // Tire
    [Header("Tire")]
    [Range(0f, 2f)]
    public float RightStiffnes = 0.7f; // Не реализовано
    [Range(0f, 2f)]
    public float ForwardStiffnes = 0.7f; // Не реализовано
    /* public AnimationCurve TireCurve; // Не реализовано
    public float CurveMultiply = 1f; */

    // public event Action OnVisualRefresh;
    public event Action OnPostPhysics;

    // Dynamic
    // Возможно лучше брать направление от центра колеса, либо делать проверку в OnDrawGizmosSelected что бы не было глюков в отображении
    private Vector3 contactPoint;
    private Vector3 contactNormal;
    // private float surfaceFriction;
    private PhysicMaterial surfaceMaterial;
    private Vector2 tireSlip;
    public Vector2 TireSlip => tireSlip;
    private Vector2 tireSlipNormalized;
    // Добавить переменную с силой покрышки, т.е. tireGrip отделить от силы подвески для доступа к трению покрышки в от поверхности. Кажется бурда это какая-то
    private Vector2 tireGrip;
    // public Vector2 TireGrip => tireGrip;

    private Vector3 newtireGripForce;

    // Other varibles
    // private float deltaTime;

    private float slipX;
    public float rlenght = 1f;

    private TireTrack tireMark;
    private ParticleSystem tirePar;
    // Возможно лучше реализовать в подвеске, а не контроллере
    private AudioSource tireAud;
    // private TireMark tireTrack;

    private float xOffset;
    private float fx;

    public Vector2 wheelSlip;

    [SerializeField] private LayerMask layerMask = -1;
    
    public void Initialize()
    {
        body = GetComponentInParent<Rigidbody>();

        GetWheel();
        AlignWheelToGround();
    }


    // Перенести в низ
    private void GetWheel()
    {
        rimObject = GetComponentInChildren<MeshRenderer>().gameObject;
        tireObject = rimObject.GetComponentInChildren<MeshRenderer>().gameObject;

        inertia = Mathf.Pow(radius, 2f) * Mass * 0.5f;
    }

    public void SetInput(float steerValue, float brakeValue, float handbrakeValue)
    {
        this.steerValue = steerValue;
        this.brakeValue = brakeValue;
        this.handbrakeValue = handbrakeValue;

        wheelSteerAngle = SteerAngle * this.steerValue;
        brakeTorque = brakeStrenght * this.brakeValue;
        handbrakeTorque = handbrakeStrenght * this.handbrakeValue;
    }

    // Вызывается в Initialize, и в других случаях, пример: Смена модели колеса (Тюнинг)
    // Перенести в низ
    public void RefreshWheel()
    {
        rimObject = GetComponentInChildren<MeshRenderer>().gameObject;
        // tireGO = wheelGO.GetComponentInChildren<MeshRenderer>().gameObject;
        tireObject = rimObject.GetComponentsInChildren<MeshRenderer>()[1].gameObject;
        // Debug.Log(tireGO);

        Bounds newBounds = tireObject.GetComponent<MeshFilter>().sharedMesh.bounds;
        // Старое
        // radius = tireGO.GetComponent<MeshFilter>().sharedMesh.bounds.extents.y;
        // width = tireGO.GetComponent<MeshFilter>().sharedMesh.bounds.extents.x;

        // Среднее значение
        // Перенести в редактор
        // radius = (newBounds.extents.y + newBounds.extents.z) / 2f;
        // Максимальное значение (не реализовано)

        width = newBounds.extents.x;
        xOffset = newBounds.center.x;

        inertia = Mathf.Pow(radius, 2f) * Mass * 0.5f;

        // Добавить обновление параметров системы частиц
        // tireMark.Width = width;
    }

    private void LineTrace()
    {
        Ray ray = new Ray(transform.position, -transform.up);

        float traceDistance = MaxLenght + radius;

        List<RaycastHit> lineHits = new List<RaycastHit>(Physics.RaycastAll(ray, traceDistance, layerMask, QueryTriggerInteraction.Ignore));
        RaycastHit closestHit = GetClosestHit(lineHits);

        if (closestHit.collider != null)
        {
            grounded = true;
            // V1 Cliping wheel throught body
            // lenght = Mathf.Clamp(closestHit.distance - (radius - MaxDepthPenetration), 0f, MaxLenght);
            // V2
            // lenght = Mathf.Clamp(closestHit.distance - (radius - MaxDepthPenetration), radius, MaxLenght);
            lenght = Mathf.Clamp(closestHit.distance - (radius - MaxDepthPenetration), radius, MaxLenght);
            // Test
            unclampedLenght = closestHit.distance - (radius - MaxDepthPenetration);
            contactPoint = closestHit.point;
            contactNormal = closestHit.normal;
            // surfaceFriction = closestHit.
            surfaceMaterial = closestHit.collider.material;
        }
        else
        {
            grounded = false;
            lenght = Mathf.Min(lenght + radius, MaxLenght);
            contactPoint = Vector3.zero;
            contactNormal = Vector3.zero;
            surfaceMaterial = null;
        }
    }

    // Примечание - 1) Колесо постоянно теряет контакт с поверхностью. 2) Физика поведения очень сильно отличается от линейной трассировки, тут определённо есть некоторые ошибки.
    // Добавить выталкивающую силу покрышки для смягчения подвески/отскоков
    private void SemiCylinderTrace()
    {
        // Wheel gravity force / decompress force
        lenght = Mathf.Min(lenght + radius, MaxLenght);

        Vector3 center = transform.position - (transform.up * lenght);
        Vector3 boxSize = new Vector3(width, 0f, (radius * Mathf.PI) / BoxesCount);

        float minAngle = Mathf.Min(Angle, 360f - (360f / BoxesCount));
        float normalizedAngle = minAngle / 2f;
        float anglePerPlane = minAngle / (BoxesCount - 1);

        float traceDistance = radius;

        List<RaycastHit> totalHits = new List<RaycastHit>();
        grounded = false;

        contactPoint = Vector3.zero;
        contactNormal = Vector3.zero;

        float wheelOffset = 0f;

        for (int i = 0; i < BoxesCount; i++)
        {
            float segmentAngle = anglePerPlane * i;
            Quaternion quaternion = transform.rotation * Quaternion.AngleAxis((-normalizedAngle + segmentAngle) - ((anglePerPlane / 2f) * AngleOffset), Vector3.right);

            List<RaycastHit> boxHits = new List<RaycastHit>(Physics.BoxCastAll(center, boxSize, quaternion * -Vector3.up, quaternion, traceDistance, layerMask, QueryTriggerInteraction.Ignore));
            RaycastHit closestHit = GetClosestHit(boxHits);

            if (closestHit.collider != null)
            {
                totalHits.Add(closestHit);
                grounded = true;
                contactPoint += center + ((quaternion * -Vector3.up) * closestHit.distance);
                // Можно заменить closestHit.normal на raycast.normal (center, quaternion - vector3.up) Для получения точной нормали под точкой касания
                // И реализовать это лучше одним лучем, после получения средней точки касания для оптимизации не влияющей на качество физики
                contactNormal += closestHit.normal;

                float penetrationDepth = Mathf.Max(radius - (closestHit.distance + MaxDepthPenetration), 0f);
                wheelOffset = Mathf.Max(wheelOffset, penetrationDepth);
            }
        }

        // Попробовать объединить
        if (grounded)
        {
            // Реализовать глобально, это замена hitResult.point
            // Дополнительно возможно реализовать и длину до .. поверхности?
            contactPoint = contactPoint / Mathf.Max(totalHits.Count, 1f);
            contactNormal = contactNormal / Mathf.Max(totalHits.Count, 1f);
        }
        else
        {
            contactPoint = Vector3.zero;
            contactNormal = Vector3.zero;
        }

        // Поправить, убножить wheelOffset на dotproduct(contact point normal, transform.up)
        // Но к этому ещё нужно добавить отталкивающую силу колеса, contact force * на остаток от dotproduct
        lenght = Mathf.Max(lenght - wheelOffset, 0f);
    }

    private RaycastHit GetClosestHit(List<RaycastHit> hits)
    {
        RaycastHit closestHit = new RaycastHit();

        foreach (var hit in hits)
            if (!hit.transform.IsChildOf(transform.root))
                if (hit.distance < closestHit.distance || closestHit.collider == null)
                    closestHit = hit;

        return closestHit;
    }

    // Сильно не хватает тяги на склонах, найти и пофиксить проблему
    public void UpdatePhysics(float deltaTime)
    {
        #region Suspension
        
        // New 1 off
        // Ray ray = new Ray(transform.position, -transform.up);
        // -----
        float compressionLenght = 0f;
        float springForce = 0f;
        float springVelocity = 0f;
        float damperForce = 0f;

        // New 1
        switch (TraceType)
        {
            default:
                LineTrace();
                break;

            case TraceType.SemiCylinder:
                SemiCylinderTrace();
                break;
        }
        // -----

        // Test
        // List<RaycastHit> hits = new List<RaycastHit>(Physics.SphereCastAll(ray, width, MaxLenght + (radius - width), layerMask, QueryTriggerInteraction.Ignore));
        /* List<RaycastHit> hits = new List<RaycastHit>(Physics.SphereCastAll(ray, radius, MaxLenght, layerMask, QueryTriggerInteraction.Ignore));
        bool pass = false;

        RaycastHit h = hits[0];
        foreach (var hit in hits)
            if (hit.collider.transform.root != transform.root)
            {
                pass = true;
                if (hit.distance < h.distance)
                {
                    h = hit;
                    // hitResult = hit;
                }
                // break;
            }

        hitResult = h;

        if (pass) */
        // -----

        // Можно объединить подвеску, колесо, и шину в 1 бранч, для оптимизации
        // New 1 off
        // if (Physics.Raycast(ray, out hitResult, MaxLenght + radius, layerMask, QueryTriggerInteraction.Ignore))
        // -----
        // if (Physics.SphereCast(ray, radius, out hitResult, MaxLenght))
        // if (Physics.SphereCast(ray, width, out hitResult, MaxLenght + (radius - width), layerMask, QueryTriggerInteraction.Ignore))

        // New 1
        if (grounded)
        {
            // New 1 off
            // grounded = true;
            // lenght = Mathf.Max(hitResult.distance - radius, 0f);
            // -----
            // Test
            // lenght = Mathf.Max(hitResult.distance - (radius - width), 0f);
            // lenght = Mathf.Max(hitResult.distance, 0f);
            // -----
            // V1 Cliping wheel throught body
            // compressionLenght = MaxLenght - lenght;
            // V2
            compressionLenght = lenght / MaxLenght;

            // Testing hardpoint
            // float hardpointValue = Mathf.Max(radius - unclampedLenght, 0f);
            hardpointDepth = Math.Max(radius - unclampedLenght, 0f);
            // float hardpointValue = Mathf.Max((radius - unclampedLenght) / radius, 0f);
            // float velocity = Math.Max(hardpointPrevDepth - hardpointDepth, 0f) / deltaTime;
            float velocity = Math.Max(hardpointPrevDepth - hardpointDepth, 0f);
            // Debug.LogWarning($"Velocity {velocity}");
            float hardpointValue = hardpointDepth / radius;
            // float hardpointValue = hardpointDepth;
            hardpointPrevDepth = hardpointDepth;
            // float hardpointForce = 1000 * hardpointValue;
            // float hardpointForce = body.mass * 10f * hardpointValue;
            // float hardpointForce = body.mass / 10f * hardpointValue;
            // float hardpointForce = body.mass * hardpointValue;
            // Реализовать
            // Заменить velocity на обратную скорость, скорость сжатия, mathf.min
            // Добавить Сжатие покрышки, и её силу
            // Ещё можно было бы от длины подвески отнять половину глубину hardpoint, типо так покрышка сдавливается
            float additionalHardpointValue = hardpointValue * (velocity / radius);
            float hardpointForce = body.mass * (hardpointValue - (velocity / radius));
            // ---
            springForce = compressionLenght * (Stiffnes * SpringCurve.Evaluate(Mathf.Lerp(1f, 0f, lenght / MaxLenght)));

            // compressionLenght = (MaxLenght - radius) - lenght;
            // springForce = compressionLenght * (Stiffnes * SpringCurve.Curve.Evaluate(Mathf.Lerp(1f, 0f, lenght / (MaxLenght - radius))));
            springVelocity = (previousLength - lenght) / deltaTime;
            damperForce = (DamperStiffnes * Mathf.Max(springVelocity, 0f));
            previousLength = lenght;
            // suspensionForce = (springForce + damperForce) * 100f;
            suspensionForce = (springForce + damperForce + hardpointForce) * 100f;
            // New 1
            // Внимание! Реализовать выталкивание физических объектов находящихся под колёсами
            // Внимание! Реализовать относительную скорость колеса, относительно скорости объекта с которым соприкасается колесо, по типу вращающейся платформы
            // body.AddForceAtPosition(-ray.direction * suspensionForce, transform.position);
            body.AddForceAtPosition(transform.up * suspensionForce, transform.position);
            // -----
        }
        else
        {
            // New 1 off
            // grounded = false;
            // Add wheel gravity
            // lenght = MaxLenght;
            // -----
            previousLength = lenght;
            suspensionForce = 0f;
        }

        rimObject.transform.localPosition = -Vector3.up * lenght;

        #endregion

        #region Wheel

        transform.localRotation = Quaternion.Euler(Vector3.up * wheelSteerAngle);
        
        // float newAngularVelocity = angularVelocity + inputTorque;
        // float tireAngularFriction = 0f;

        if (grounded)
        {
            // New 1
            // linearVelocity = transform.InverseTransformDirection(body.GetPointVelocity(hitResult.point));
            linearVelocity = transform.InverseTransformDirection(body.GetPointVelocity(contactPoint));
            // tireAngularFriction = (((newAngularVelocity * radius) - linearVelocity.z) / radius) * deltaTime * 0.5f; //* 10f; этот множитель уменьшаят погречность/разницу линейной скорости от скорости двигателя
            // angularVelocity = linearVelocity.z / radius;
        }
        else
        {
            linearVelocity = Vector3.zero;
        }

        // Wheel Acceleration
        float frictionTorque = tireGrip.y * radius;

        float angularAcceleration = (inputTorque - frictionTorque) / inertia;
        angularVelocity += angularAcceleration * deltaTime;

        float longSlipVelocity = (angularVelocity * radius) - linearVelocity.z;
        // Идентично 5

        // Brake
        float totalBrakeTorque = Mathf.Max(brakeTorque, handbrakeTorque);
        // 0.01f - Множитель сопротивления качения
        float rollResistanceTorqeu = suspensionForce * radius * 0.01f;
        float angularDeacceleration = ((totalBrakeTorque + rollResistanceTorqeu) * -Mathf.Sign(angularVelocity)) / inertia;

        // Заменить if на (true ? 1 : 0)
        if (Mathf.Sign(angularVelocity) == Mathf.Sign(angularVelocity + (angularDeacceleration * deltaTime)))
            angularVelocity += angularDeacceleration * deltaTime;
        else
            angularVelocity = 0f;
            // angularVelocity += Mathf.Max(angularDeacceleration, angularVelocity) * deltaTime;

        // Почти индентично
        // Готово 5

        // Get TireSlip y
        float targetAngularVelocity = linearVelocity.z / radius;
        float targetAngularAcceleration = (angularVelocity - targetAngularVelocity) / deltaTime;
        float targetTorque = targetAngularAcceleration * inertia;

        float maxFrictionTorque = suspensionForce * radius * 1f;

        /* // float sx = 0f;
        if (suspensionForce == 0f)
        {
            // sx = 0f;
            tireSlip.y = 0f;
        }
        else
        {
            // sx = targetTorque / maxFrictionTorque;
            // tireSlip.y = Mathf.Clamp(targetTorque / maxFrictionTorque, -1f, 1f);
            tireSlip.y = Mathf.Clamp(targetTorque / maxFrictionTorque, -100f, 100f);
        } */

        // И тут наверное тоже можно убрать ограничение, хм.. да!
        tireSlip.y = (suspensionForce != 0f ? Mathf.Clamp(targetTorque / maxFrictionTorque, -100f, 100f) : 0f);
        // Идентично 5

        rimObject.transform.Rotate(Vector3.right * ((Mathf.Rad2Deg * angularVelocity) * deltaTime), Space.Self);

        #endregion

        #region Tire

        if (grounded)
        {
            float surfaceFriction = surfaceMaterial.dynamicFriction;
            // Нужно создать отдельную переменную для контролирования ...
            // Vector2 tireFrictionCoef = new Vector2(RightStiffnes, ForwardStiffnes) * surfaceFriction;

            // Temp off
            // Можно убрать ограничение
            // tireSlip.x = Mathf.Clamp(-linearVelocity.x, -1f, 1f);
            tireSlip.x = -linearVelocity.x;

            /* // Test 21.10.20
            float slipAngle = (linearVelocity.z != 0f ? Mathf.Rad2Deg * Mathf.Atan(-linearVelocity.x / Mathf.Abs(linearVelocity.z)) : 0f);
            float slipPeak = 8f;
            tireSlip.x = slipAngle / slipPeak;
            // --- */

            // tireSlipNormalized = tireSlip.normalized * tireSlip.magnitude;
            /* if (tireSlip.magnitude > 1)
            {
                tireSlipNormalized = tireSlip.normalized;
            }
            else
            {
                tireSlipNormalized = tireSlip;
            } */
            // tireSlipNormalized = tireSlip.normalized * Mathf.Min(tireSlip.magnitude, 1f);
            tireSlipNormalized = (tireSlip.magnitude > 1f ? tireSlip.normalized : tireSlip);

            // Не работает, слишком мелкий коэффициент
            float penL = 2f * Mathf.Sqrt(MaxDepthPenetration * ((2f * radius) - MaxDepthPenetration));
            float penO = 2f * Mathf.Acos((radius - MaxDepthPenetration) / radius);
            float penS = (penO * radius);
            // Debug.LogError(penL);
            // Debug.LogError(penS);
            float tirePatchContact = (width * 2f) * penL;
            // ---

            float tirePressurePBar = 30f;
            float testPressure = tirePatchContact * tirePressurePBar;

            // Ограничить силу подвески что бы не кидало при сильных ударах
            // tireGrip = tireSlipNormalized * (Mathf.Max(suspensionForce, 0f) * surfaceFriction);
            tireGrip = tireSlipNormalized * (Mathf.Max(suspensionForce, 0f) * (testPressure * surfaceFriction));

            Vector3 rightPlane = Vector3.ProjectOnPlane(transform.right, contactNormal).normalized;
            Vector3 forwardPlane = Vector3.ProjectOnPlane(transform.forward, contactNormal).normalized;

            Vector3 gripForce = (rightPlane * tireGrip.x) + (forwardPlane * tireGrip.y);

            body.AddForceAtPosition(gripForce, contactPoint);
        }
        else
        {
            tireSlip = Vector2.zero;
            tireGrip = Vector2.zero;
        }
            
        #endregion

        OnPostPhysics?.Invoke();
    }

    private void Wheel(float deltaTime)
    {
        RaycastHit hitResult = new RaycastHit();

        #region Wheel

        wheelSteerAngle = (SteerAngle * steerValue);

        float newAngularVelocity = angularVelocity + inputTorque;
        float tireAngularFriction = 0f;
        
        if (grounded)
        {
            linearVelocity = transform.InverseTransformDirection(body.GetPointVelocity(hitResult.point));
            // tireAngularFriction = (((newAngularVelocity * radius) - linearVelocity.z) / radius) * deltaTime; //* 10f; этот множитель уменьшаят погречность/разницу линейной скорости от скорости двигателя
            tireAngularFriction = (((newAngularVelocity * radius) - linearVelocity.z) / radius) * deltaTime * 0.5f; //* 10f; этот множитель уменьшаят погречность/разницу линейной скорости от скорости двигателя
            //tireAngularFriction = ((((newAngularVelocity * radius) - linearVelocity.z) * ForwardStiffnes) / radius) * deltaTime * 10f;
        }
        else
        {
            linearVelocity = Vector3.zero;
        }

        //angularVelocity = ((driveTorque / inertia) * deltaTime) + angularVelocity;

        //float tireAngularFriction = (((newAngularVelocity * radius) - linearVelocity.z) / radius) * deltaTime * 10f;
        //float tireAngularFriction = ((((newAngularVelocity * radius) - linearVelocity.z) * ForwardStiffnes) / radius) * deltaTime * 10f;
        //float angularFreeFriction = inertia * angularVelocity * deltaTime * 0.1f;
        float angularFreeFriction = inertia * newAngularVelocity * deltaTime * 0.1f;
        // angularVelocity = newAngularVelocity - (tireAngularFriction + angularFreeFriction);
        // angularVelocity = newAngularVelocity - tireAngularFriction;

        // New Test ---------

        // angularVelocity = angularVelocity + ((inputTorque / inertia) * deltaTime);
        // angularVelocity = angularVelocity + (((inputTorque - (fx * radius)) / inertia) * deltaTime);
        angularVelocity = angularVelocity + (((inputTorque * 100 - (fx * radius)) / inertia) * deltaTime);
        // angularVelocity = angularVelocity + (((inputTorque - (fx * radius)) / inertia));

        float sx = 0f;
        if (grounded)
        {
            sx = Mathf.Clamp((((angularVelocity - (linearVelocity.z / radius)) / deltaTime) * inertia) / (suspensionForce * radius), -1f, 1f);
        }






        //angularVelocity = newAngularVelocity - (tireAngularFriction);
        //angularVelocity = newAngularVelocity - ((((newAngularVelocity * radius) - linearVelocity.z) / radius) * deltaTime * 10f);

        angularVelocity = (angularVelocity - ((Mathf.Sign(angularVelocity)) * Mathf.Min(Mathf.Max(brakeValue * brakeStrenght, handbrakeValue * handbrakeStrenght) * deltaTime, Mathf.Abs(angularVelocity))));

        forwardSlipVelocity = (angularVelocity * radius) - linearVelocity.z;
        
        #endregion



        #region Tire
        
        // Старое
        Vector2 tireSlipVelocity;
        Vector3 tireGripForce;

        if (grounded)
        {
            tireSlipVelocity = new Vector2(Mathf.Clamp(RightStiffnes * -linearVelocity.x, -1f, 1f), Mathf.Clamp(ForwardStiffnes * forwardSlipVelocity, -1f, 1f));

            // New
            // tireSlipVelocity.x = Mathf.Clamp(RightStiffnes * -linearVelocity.x, -1f, 1f);
            // tireSlipVelocity.y = Mathf.Clamp(ForwardStiffnes * forwardSlipVelocity, -1f, 1f);

            // Test
            // float coef = (Mathf.Abs(linearVelocity.x) / (rlenght * 0.01f)) * deltaTime;
            // float testslip = Mathf.Clamp(slipX + ((Mathf.Sign(linearVelocity.x * -1f) - slipX) * coef), -1f, 1f);
            // slipX = testslip;
            // tireSlipVelocity.x = Mathf.Clamp(RightStiffnes * testslip, -1f, 1f);


            // New Test
            float coefx = (Mathf.Sign(linearVelocity.x * -1f) - tireGrip.x) * ((Mathf.Abs(linearVelocity.x) / 0.01f) * deltaTime);
            tireGrip.x = Mathf.Clamp(tireGrip.x + (coefx * RightStiffnes), -1f, 1f);
            // tireGrip.y = Mathf.Clamp(ForwardStiffnes * forwardSlipVelocity, -1f, 1f);
            // Проблема с tireGrip.y - Свободные колёса создают большое, даже огромное сопротивление
            float coefy = (Mathf.Sign(forwardSlipVelocity) - tireGrip.y) * ((Mathf.Abs(forwardSlipVelocity) / 0.01f) * deltaTime);
            // tireGrip.y = Mathf.Clamp(tireGrip.y + (coefy * ForwardStiffnes), -1f, 1f);
            tireGrip.y = Mathf.Clamp(forwardSlipVelocity * ForwardStiffnes, -1f, 1f);
            // Добавить кривую трения шин
            // Добавить TireContact
            // Сцепление зависит от угла колеса относительно z, и нормали поверхности
            // Vector3 tireContactVector = Quaternion.AngleAxis(90f, Vector3.ProjectOnPlane(transform.forward, hitResult.normal)) * transform.right;
            // float tireContact = Mathf.Max(Vector3.Dot(tireContactVector, hitResult.normal), 0f);

            // tireGripForce = Vector3.ProjectOnPlane(transform.right, hitResult.normal).normalized * (tireSlipVelocity.x * Mathf.Max(suspensionForce, 0f)) + Vector3.ProjectOnPlane(transform.forward, hitResult.normal).normalized * (tireSlipVelocity.y * Mathf.Max(suspensionForce, 0f));
            tireGripForce = Vector3.ProjectOnPlane(transform.right, hitResult.normal).normalized * (tireGrip.x * Mathf.Max(suspensionForce, 0f)) + Vector3.ProjectOnPlane(transform.forward, hitResult.normal).normalized * (tireGrip.y * Mathf.Max(suspensionForce, 0f));

            // ---
            Vector2 newtireGrip;
            newtireGrip.x = Mathf.Clamp(linearVelocity.x * RightStiffnes * -1f, -1f, 1f);
            newtireGrip.y = sx;

            // Vector2 mtireGrip = (TireCurve.Curve.Evaluate(newtireGrip.magnitude) * newtireGrip.normalized) * Mathf.Max(suspensionForce, 0f);

            Vector3 tireContactVector = Quaternion.AngleAxis(90f, Vector3.ProjectOnPlane(transform.forward, hitResult.normal).normalized) * rimObject.transform.right;
            float tireContact = Mathf.Max(Vector3.Dot(tireContactVector, hitResult.normal), 0f);
        }
        #endregion
    }

    public void SetupWheelParameters()
    {
        // GameObject rim = GetComponentInChildren<MeshRenderer>().gameObject;
        GameObject rim = transform.Find("Rim").gameObject;
        // GameObject tire = rim.GetComponentInChildren<MeshRenderer>().gameObject;
        GameObject tire = rim.transform.Find("Tire").gameObject;

        // Bounds rimBounds = rim.GetComponent<MeshFilter>().sharedMesh.bounds;
        if (tire.GetComponent<MeshFilter>() == null)
            return;

        Bounds tireBounds = tire.GetComponent<MeshFilter>().sharedMesh.bounds;

        radius = (tireBounds.extents.y + tireBounds.extents.z) / 2f;
        width = tireBounds.extents.x;
    }

    public void AssignWheelMeshes()
    {
        rimObject = transform.Find("Rim").gameObject;
        if (rimObject == null)
            return;
        tireObject = rimObject.transform.Find("Tire").gameObject;

        rimObject.transform.localScale = new Vector3(-Mathf.Sign(transform.localPosition.x), rimObject.transform.localScale.y, rimObject.transform.localScale.z);
        rimObject.transform.localPosition = -transform.up * lenght;

        AssignWheelParameters();
    }

    private void AssignWheelParameters()
    {
        if (tireObject == null)
            return;

        Bounds tireBounds = tireObject.GetComponent<MeshFilter>().sharedMesh.bounds;

        radius = (tireBounds.extents.y + tireBounds.extents.z) / 2f;
        width = tireBounds.extents.x;
    }

    public void AlignWheelToGround()
    {
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hitResult;

        if (Physics.Raycast(ray, out hitResult, MaxLenght + radius))
            lenght = Mathf.Max(hitResult.distance - radius, 0f);
        else
            lenght = MaxLenght;

        previousLength = lenght;
    }

    #if UNITY_EDITOR
    private void OnValidate()
    {
        // Добавить минимальную длину подвески для исправления бага с пересекающим кузов колесо
        lenght = MaxLenght * 0.8f;

        if (rimObject != null)
            rimObject.transform.localPosition = -transform.up * lenght;
    }

    private void OnDrawGizmosSelected()
    {
        // Suspension
        // Gizmos.color = Color.green;
        Vector3 center = transform.position - (transform.up * lenght);
        // Gizmos.DrawLine(transform.position, sus);
        GizmosLibrary.DrawTwoColoredLineByCurve(center, transform.position, SpringCurve, Color.green, Color.red, 8);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(center, center - (transform.up * radius));

        // Wheel
        Gizmos.color = Color.HSVToRGB(37f / 360f, 0.83f, 0.96f);
        GizmosLibrary.DrawArc(center, transform.right, -transform.up, radius, 45f, 2);
        Gizmos.color = Color.cyan;

        GizmosLibrary.DrawCircle(center - (transform.right * width), transform.right, -transform.up, radius);
        GizmosLibrary.DrawCircle(center + (transform.right * width), transform.right, -transform.up, radius);

        Gizmos.color = Color.HSVToRGB(37f / 360f, 0.83f, 0.96f);

        GizmosLibrary.DrawCircle(center - (transform.right * width), transform.right, -transform.up, radius - MaxDepthPenetration);
        GizmosLibrary.DrawCircle(center + (transform.right * width), transform.right, -transform.up, radius - MaxDepthPenetration);

        Gizmos.color = Color.blue;
        
        Gizmos.DrawLine(center, contactPoint);

        // New 1
        if (TraceType != TraceType.SemiCylinder)
            return;

        Gizmos.color = Color.red;

        float minAngle = Mathf.Min(Angle, 360f - (360f / BoxesCount));
        float normalizedAngle = minAngle / 2f;
        float anglePerPlane = minAngle / (BoxesCount - 1);

        Vector2 planeSize = new Vector2(width, (radius * Mathf.PI) / BoxesCount);
        // Vector2 planeSize = new Vector2(width, ((radius / (360f / Angle)) * Mathf.PI) / BoxesCount);

        for (int i = 0; i < BoxesCount; i++)
        {
            float segmentAngle = anglePerPlane * i;
            Quaternion boxQuaternion = transform.rotation * Quaternion.AngleAxis((-normalizedAngle + segmentAngle) - ((anglePerPlane / 2f) * AngleOffset), Vector3.right);
            Vector3 boxDirection = boxQuaternion * -Vector3.up;

            Gizmos.DrawLine(center, center + (boxDirection * radius));
            GizmosLibrary.DrawWirePlane(center + (boxDirection * radius), transform.right, boxQuaternion * Vector3.forward, planeSize);
        }
    }
    #endif
}
