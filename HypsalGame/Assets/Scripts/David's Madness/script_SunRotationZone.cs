using UnityEngine;

public class SunRotationZone : MonoBehaviour
{
    [Header("Desired Sun Rotation At Full Proximity")]
    [SerializeField] private float desiredRotationX;
    [SerializeField] private float desiredRotationY;
    [SerializeField] private float desiredRotationZ;

    [Header("Distance Settings")]
    [SerializeField] private float startTransitionDistance = 15f;
    [SerializeField] private float fullProximityDistance = 2f;

    [Header("Rotation Response")]
    [SerializeField] private AnimationCurve distanceResponseCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    [Header("Behavior")]
    [SerializeField] private bool lockAtFullProximity = true;
    [SerializeField] private int priority = 0;

    private Quaternion desiredRotation;
    private bool isLocked;

    public Quaternion DesiredRotation => desiredRotation;
    public bool LockAtFullProximity => lockAtFullProximity;
    public int Priority => priority;
    public bool IsLocked => isLocked;

    private void Awake()
    {
        desiredRotation = Quaternion.Euler(desiredRotationX, desiredRotationY, desiredRotationZ);

        if (fullProximityDistance >= startTransitionDistance)
        {
            Debug.LogWarning(
                $"[SunRotationZone] {name}: fullProximityDistance should be smaller than startTransitionDistance.",
                this
            );
        }
    }

    public float GetInfluence(Vector3 playerPosition)
    {
        float distance = Vector3.Distance(playerPosition, transform.position);

        if (distance >= startTransitionDistance)
            return 0f;

        if (distance <= fullProximityDistance)
            return 1f;

        float normalized = Mathf.InverseLerp(startTransitionDistance, fullProximityDistance, distance);
        return Mathf.Clamp01(distanceResponseCurve.Evaluate(normalized));
    }

    public bool IsPlayerAtFullProximity(Vector3 playerPosition)
    {
        float distance = Vector3.Distance(playerPosition, transform.position);
        return distance <= fullProximityDistance;
    }

    public void SetLocked(bool value)
    {
        isLocked = value;
    }

    private void OnEnable()
    {
        if (SunRotationManager.Instance != null)
        {
            SunRotationManager.Instance.RegisterZone(this);
        }
    }

    private void Start()
    {
        if (SunRotationManager.Instance != null)
        {
            SunRotationManager.Instance.RegisterZone(this);
        }
    }

    private void OnDisable()
    {
        if (SunRotationManager.Instance != null)
        {
            SunRotationManager.Instance.UnregisterZone(this);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, startTransitionDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, fullProximityDistance);
    }
}