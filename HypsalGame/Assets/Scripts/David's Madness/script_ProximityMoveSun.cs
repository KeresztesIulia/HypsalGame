using UnityEngine;

public class ProximityLightRotation : MonoBehaviour
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

    [Header("Behavior")]
    [SerializeField] private bool lockAtFullProximity = true;
    [SerializeField] private bool forceDefaultRotationOnStart = true;

    [Header("Debug")]
    [SerializeField] private bool debugLogs = true;

    private Transform playerTransform;
    private Transform sunTransform;

    private Quaternion defaultRotation;
    private Quaternion desiredRotation;

    private bool lockedFinalRotation = false;

    private void Start()
    {
        lockedFinalRotation = false;
        desiredRotation = Quaternion.Euler(desiredRotationX, desiredRotationY, desiredRotationZ);

        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject == null)
        {
            Debug.LogError("[ProximityLightRotation] No active GameObject with tag 'Player' was found.", this);
            enabled = false;
            return;
        }

        playerTransform = playerObject.transform;

        GameObject sunObject = GameObject.FindWithTag("Sun");
        if (sunObject == null)
        {
            Debug.LogError("[ProximityLightRotation] No active GameObject with tag 'Sun' was found.", this);
            enabled = false;
            return;
        }

        sunTransform = sunObject.transform;

        if (forceDefaultRotationOnStart)
        {
            defaultRotation = Quaternion.Euler(defaultRotationX, defaultRotationY, defaultRotationZ);
            sunTransform.rotation = defaultRotation;
        }
        else
        {
            defaultRotation = sunTransform.rotation;
        }

        if (fullProximityDistance >= startTransitionDistance)
        {
            Debug.LogWarning("[ProximityLightRotation] fullProximityDistance should be smaller than startTransitionDistance.", this);
        }

        if (debugLogs)
        {
            Debug.Log($"[ProximityLightRotation] Player found: {playerObject.name}", this);
            Debug.Log($"[ProximityLightRotation] Sun found: {sunObject.name}", this);
            Debug.Log($"[ProximityLightRotation] Lock at full proximity: {lockAtFullProximity}", this);
            Debug.Log($"[ProximityLightRotation] Force default rotation on start: {forceDefaultRotationOnStart}", this);
        }
    }

    private void Update()
    {
        if (playerTransform == null || sunTransform == null)
            return;

        if (lockedFinalRotation)
            return;

        float distance = Vector3.Distance(playerTransform.position, transform.position);

        float t;

        if (distance >= startTransitionDistance)
        {
            t = 0f;
        }
        else if (distance <= fullProximityDistance)
        {
            t = 1f;

            if (lockAtFullProximity)
            {
                sunTransform.rotation = desiredRotation;
                lockedFinalRotation = true;

                if (debugLogs)
                {
                    Debug.Log("[ProximityLightRotation] Full proximity reached. Final rotation locked.", this);
                }

                return;
            }
        }
        else
        {
            t = Mathf.InverseLerp(startTransitionDistance, fullProximityDistance, distance);
        }

        float curvedT = Mathf.Clamp01(distanceResponseCurve.Evaluate(t));
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