using UnityEngine;

public class script_ProximityMoveSun : MonoBehaviour
{
    [Header("Default Light Rotation")]
    [SerializeField] private float defaultRotationX;
    [SerializeField] private float defaultRotationY;
    [SerializeField] private float defaultRotationZ;

    [Header("Desired Light Rotation At Full Proximity")]
    [SerializeField] private float desiredRotationX;
    [SerializeField] private float desiredRotationY;
    [SerializeField] private float desiredRotationZ;

    [Header("Distance Settings")]
    [SerializeField] private float startTransitionDistance = 15f;
    [SerializeField] private float fullProximityDistance = 2f;

    [Header("Rotation Response")]
    [SerializeField] private AnimationCurve distanceResponseCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    [Header("Debug")]
    [SerializeField] private bool debugLogs = true;

    private Transform playerTransform;
    private Transform sunTransform;

    private Quaternion defaultRotation;
    private Quaternion desiredRotation;

    private bool lockedFinalRotation = false;

    private void Start()
    {
        defaultRotation = Quaternion.Euler(defaultRotationX, defaultRotationY, defaultRotationZ);
        desiredRotation = Quaternion.Euler(desiredRotationX, desiredRotationY, desiredRotationZ);

        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject == null)
        {
            Debug.LogError("[script_ProximityMoveSun] No active GameObject with tag 'Player' was found.", this);
            enabled = false;
            return;
        }

        playerTransform = playerObject.transform;

        GameObject sunObject = GameObject.FindWithTag("Sun");
        if (sunObject == null)
        {
            Debug.LogError("[script_ProximityMoveSun] No active GameObject with tag 'Sun' was found.", this);
            enabled = false;
            return;
        }

        sunTransform = sunObject.transform;
        sunTransform.rotation = defaultRotation;

        if (debugLogs)
        {
            Debug.Log($"[script_ProximityMoveSun] Player found: {playerObject.name}", this);
            Debug.Log($"[script_ProximityMoveSun] Sun found: {sunObject.name}", this);
            Debug.Log($"[script_ProximityMoveSun] Start transition distance: {startTransitionDistance}", this);
            Debug.Log($"[script_ProximityMoveSun] Full proximity distance: {fullProximityDistance}", this);
        }

        if (fullProximityDistance >= startTransitionDistance)
        {
            Debug.LogWarning("[script_ProximityMoveSun] fullProximityDistance should be smaller than startTransitionDistance.", this);
        }
    }

    private void Update()
    {
        if (lockedFinalRotation || playerTransform == null || sunTransform == null)
            return;

        float distance = Vector3.Distance(playerTransform.position, transform.position);

        if (distance <= fullProximityDistance)
        {
            sunTransform.rotation = desiredRotation;
            lockedFinalRotation = true;

            if (debugLogs)
            {
                Debug.Log("[script_ProximityMoveSun] Full proximity reached. Final rotation locked.", this);
            }

            return;
        }

        if (distance >= startTransitionDistance)
        {
            sunTransform.rotation = defaultRotation;
            return;
        }

        float normalizedDistance = Mathf.InverseLerp(startTransitionDistance, fullProximityDistance, distance);
        float curvedT = Mathf.Clamp01(distanceResponseCurve.Evaluate(normalizedDistance));

        sunTransform.rotation = Quaternion.Slerp(defaultRotation, desiredRotation, curvedT);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, startTransitionDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, fullProximityDistance);
    }
}