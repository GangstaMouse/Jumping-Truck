using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumetricWheelCollider : MonoBehaviour
{
    private Rigidbody body;

    [Header("Suspension")]
    public float MaxLenght = 0.5f;
    [SerializeField] private float depth = 0.04f;
    public float SpringStiffnes;
    public float DamperStiffnes;

    // Dynamic
    private List<RaycastHit> hitResults = new List<RaycastHit>();
    private float lenght;
    private float previousLength;
    private float suspensionForce;

    [Header ("Wheel")]
    public float Radius = 0.5f;
    public float Width = 0.2f;
    public int BoxesCount = 8;
    public float Angle = 45f;
    public float AngleOffset;

    // Dynamic
    public bool isTouches { get; private set; }
    public Vector3 touchVector { get; private set; }
    public Vector3 linearVelocity { get; private set; }

    private void Awake() => body = transform.root.GetComponent<Rigidbody>();

    private void Start()
    {
        RaycastHit empt = new RaycastHit();

        RaycastHit t;

        Physics.Raycast(transform.position, -transform.up, out t, 10f);

        Debug.Log(empt.distance);
        Debug.Log(t.distance);
        Debug.Log(t.distance < empt.distance);
        Debug.Log(empt.collider);
        Debug.Log(t.collider);
    }

    private void FixedUpdate()
    {
        float deltaTime = Time.fixedDeltaTime;

        Vector3 center = transform.position - (transform.up * lenght);
        Vector3 boxSize = new Vector3(Width, 0f, (Radius * Mathf.PI) / BoxesCount);

        float minAngle = Mathf.Min(Angle, 360f - (360f / BoxesCount));
        float normalizedAngle = minAngle / 2f;
        float anglePerPlane = minAngle / (BoxesCount - 1);

        hitResults.Clear();

        isTouches = false;
        touchVector = Vector3.zero;

        float wheelOffset = 0f;

        float distance = Radius + depth;
        float closestDistanceToSurface = Radius;

        for (int i = 0; i < BoxesCount; i++)
        {
            float segmentAngle = anglePerPlane * i;
            Quaternion quaternion = transform.rotation * Quaternion.AngleAxis(-normalizedAngle + segmentAngle, Vector3.right);

            // if (Physics.BoxCast(center, boxSize, quaternion * -Vector3.up, out RaycastHit hit, quaternion, distance))
            List<RaycastHit> boxHits = new List<RaycastHit>(Physics.BoxCastAll(center, boxSize, quaternion * -Vector3.up, quaternion, distance));
            RaycastHit hit = GetClosestHit(boxHits);

            if (hit.collider != null)
            {
                if (hit.distance <= Radius)
                {
                    isTouches = true;
                    touchVector += quaternion * -Vector3.up;
                }
                
                hitResults.Add(hit);

                float penetrationDepth = Mathf.Max(Radius - hit.distance, 0f);
                wheelOffset = Mathf.Max(wheelOffset, penetrationDepth);

                float distanceToSurface = Mathf.Max(hit.distance - Radius, 0f);
                closestDistanceToSurface = Mathf.Min(closestDistanceToSurface, distanceToSurface);
            }
        }

        touchVector = touchVector.normalized;
        // float dToSurface = Mathf.Max(closestDistanceToSurface, depth);

        #region Suspension

        // Попробовать объединить
        // Compression
        if (isTouches)
        {
            lenght = Mathf.Max(lenght - wheelOffset, 0f);
            float compressionLenght = MaxLenght - lenght;
            float springVelocity = (previousLength - lenght) / deltaTime;
            float damperForce = (DamperStiffnes * Mathf.Max(springVelocity, 0f));
            float springForce = compressionLenght * (SpringStiffnes * Mathf.Lerp(1f, 0f, lenght / MaxLenght));
            suspensionForce = (springForce + damperForce) * 100f;
            
            body.AddForceAtPosition(transform.up * suspensionForce, transform.position);
        }
        // Decompression
        else
            lenght = Mathf.Min(lenght + closestDistanceToSurface, MaxLenght);

        previousLength = lenght;

        #endregion

        #region Wheel

        if (isTouches)
            linearVelocity = transform.InverseTransformDirection(body.GetPointVelocity(center + touchVector));
        else
            linearVelocity = Vector3.zero;

        #endregion
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

    #if UNITY_EDITOR
    private void OnValidate()
    {
        lenght = MaxLenght;
    }

    private void OnDrawGizmos()
    {
        Vector3 center = transform.position - (transform.up * lenght);

        // Draw suspension lenght
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, center);

        // Draw wheel contact point - obsolete
        Gizmos.color = Color.HSVToRGB(37f / 360f, 0.83f, 0.96f);
        GizmosLibrary.DrawArc(center, transform.right, -transform.up, Radius, 22.5f, 2);

        // Draw width circles
        Vector3 offset = transform.right * Width;

        Gizmos.color = Color.cyan;
        GizmosLibrary.DrawCircle(center - offset, transform.right, -transform.up, Radius);
        GizmosLibrary.DrawCircle(center + offset, transform.right, -transform.up, Radius);

        // Draw contact points
        Gizmos.color = Color.white;
        foreach (var hit in hitResults)
            Gizmos.DrawSphere(hit.point, 0.1f);

        // Draw boxes planes
        Gizmos.color = Color.red;
        float minAngle = Mathf.Min(Angle, 360f - (360f / BoxesCount));
        float normalizedAngle = minAngle / 2f;
        float anglePerPlane = minAngle / (BoxesCount - 1);

        Vector2 planeSize = new Vector2(Width, (Radius * Mathf.PI) / BoxesCount);
        /* Vector2 planeSize = new Vector2(Width, Radius * (4f / BoxesCount));
        Debug.LogWarning($"PlaneSize {planeSize}"); */

        for (int i = 0; i < BoxesCount; i++)
        {
            float segmentAngle = anglePerPlane * i;
            Quaternion boxQuaternion = transform.rotation * Quaternion.AngleAxis(-normalizedAngle + segmentAngle, Vector3.right);
            Vector3 boxDirection = boxQuaternion * -Vector3.up;

            Gizmos.DrawLine(center, center + (boxDirection * Radius));
            GizmosLibrary.DrawWirePlane(center + (boxDirection * Radius), transform.right, boxQuaternion * Vector3.forward, planeSize);
        }

        // Draw tire force vector
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(center, center + touchVector);

        // Test draw quaternion direction
        /* Gizmos.color = Color.magenta;
        Quaternion rot = transform.rotation * Quaternion.AngleAxis(-157.5f, Vector3.right);
        Gizmos.DrawLine(transform.position, transform.position + (rot * -Vector3.up)); */
    }
    #endif
}
