using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class script_ModelGenerationSequence : MonoBehaviour
{
    private enum TriggerCenterMode
    {
        ThisTransformPosition,
        RendererBoundsCenter,
        CustomOffsetFromTransform
    }

    [Header("Sequence Settings")]
    [Tooltip("Total time from the first visible version to the final visible version.")]
    [SerializeField, Min(0.01f)] private float totalDuration = 3f;

    [Tooltip("Small overlap where the next version appears before the previous version disappears.")]
    [SerializeField, Min(0f)] private float overlapTime = 0f;

    [Tooltip("If true, all child model versions are hidden when the game starts.")]
    [SerializeField] private bool hideChildrenOnGameStart = true;

    [Tooltip("If true, the final version stays visible at the end.")]
    [SerializeField] private bool keepFinalVersionActive = true;

    [Header("Player Distance Trigger")]
    [SerializeField] private bool usePlayerDistanceTrigger = true;

    [Tooltip("The player must be within this distance of the chosen trigger center before the sequence begins.")]
    [SerializeField, Min(0f)] private float playerTriggerDistance = 5f;

    [Tooltip("Determines where the distance field is centered.")]
    [SerializeField] private TriggerCenterMode triggerCenterMode = TriggerCenterMode.RendererBoundsCenter;

    [Tooltip("Used only when Trigger Center Mode is set to Custom Offset From Transform.")]
    [SerializeField] private Vector3 customTriggerCenterOffset = Vector3.zero;

    [Tooltip("The tag used to find the player object.")]
    [SerializeField] private string playerTag = "Player";

    [Tooltip("How often the script checks the player's distance. Lower values are more responsive, higher values are cheaper.")]
    [SerializeField, Min(0.01f)] private float playerCheckInterval = 0.1f;

    [Header("Scene Visualization")]
    [SerializeField] private bool showTriggerDistanceGizmo = true;

    [Tooltip("If true, the trigger distance is only drawn when this object is selected.")]
    [SerializeField] private bool onlyShowGizmoWhenSelected = true;

    [Tooltip("Color of the player trigger distance sphere in the Scene view.")]
    [SerializeField] private Color triggerDistanceGizmoColor = new Color(0f, 1f, 1f, 0.75f);

    private readonly List<GameObject> modelVersions = new List<GameObject>();

    private Transform playerTransform;
    private Coroutine sequenceCoroutine;

    private bool sequenceHasStarted;
    private bool sequenceHasFinished;
    private bool warnedMissingPlayerTag;
    private bool warnedMissingPlayerObject;

    private float playerCheckTimer;

    private void Awake()
    {
        BuildModelVersionListFromHierarchy();

        if (hideChildrenOnGameStart)
        {
            SetAllModelVersionsActive(false);
        }
    }

    private void Update()
    {
        if (!usePlayerDistanceTrigger)
        {
            return;
        }

        if (sequenceHasStarted || sequenceHasFinished)
        {
            return;
        }

        playerCheckTimer -= Time.deltaTime;

        if (playerCheckTimer > 0f)
        {
            return;
        }

        playerCheckTimer = playerCheckInterval;

        CheckPlayerDistanceTrigger();
    }

    public void PlaySequence()
    {
        if (sequenceHasStarted || sequenceHasFinished)
        {
            return;
        }

        if (modelVersions.Count == 0)
        {
            Debug.LogWarning($"{name}: No child model versions were found for the generation sequence.", this);
            return;
        }

        sequenceHasStarted = true;
        sequenceCoroutine = StartCoroutine(PlayModelGenerationSequence());
    }

    private void CheckPlayerDistanceTrigger()
    {
        if (!TryCachePlayerTransform())
        {
            return;
        }

        Vector3 triggerCenter = GetTriggerCenter();
        Vector3 headingToPlayer = playerTransform.position - triggerCenter;

        float squaredDistanceToPlayer = headingToPlayer.sqrMagnitude;
        float squaredTriggerDistance = playerTriggerDistance * playerTriggerDistance;

        if (squaredDistanceToPlayer <= squaredTriggerDistance)
        {
            PlaySequence();
        }
    }

    private bool TryCachePlayerTransform()
    {
        if (playerTransform != null)
        {
            return true;
        }

        GameObject playerObject = null;

        try
        {
            playerObject = GameObject.FindWithTag(playerTag);
        }
        catch (UnityException)
        {
            if (!warnedMissingPlayerTag)
            {
                warnedMissingPlayerTag = true;
                Debug.LogWarning($"{name}: The tag \"{playerTag}\" is not defined. Please add it in the Unity Tag Manager or change the Player Tag field.", this);
            }

            return false;
        }

        if (playerObject == null)
        {
            if (!warnedMissingPlayerObject)
            {
                warnedMissingPlayerObject = true;
                Debug.LogWarning($"{name}: No active GameObject with the tag \"{playerTag}\" was found in the scene.", this);
            }

            return false;
        }

        playerTransform = playerObject.transform;
        return true;
    }

    private IEnumerator PlayModelGenerationSequence()
    {
        SetAllModelVersionsActive(false);

        if (modelVersions.Count == 1)
        {
            modelVersions[0].SetActive(true);
            sequenceCoroutine = null;
            sequenceHasFinished = true;
            yield break;
        }

        float secondsBetweenVersions = totalDuration / (modelVersions.Count - 1);
        GameObject previousVersion = null;

        for (int i = 0; i < modelVersions.Count; i++)
        {
            GameObject currentVersion = modelVersions[i];

            if (currentVersion == null)
            {
                continue;
            }

            currentVersion.SetActive(true);

            if (previousVersion != null)
            {
                if (overlapTime > 0f)
                {
                    yield return new WaitForSeconds(overlapTime);
                }

                previousVersion.SetActive(false);
            }

            previousVersion = currentVersion;

            if (i < modelVersions.Count - 1)
            {
                float remainingWaitTime = Mathf.Max(0f, secondsBetweenVersions - overlapTime);

                if (remainingWaitTime > 0f)
                {
                    yield return new WaitForSeconds(remainingWaitTime);
                }
                else
                {
                    yield return null;
                }
            }
        }

        if (!keepFinalVersionActive && previousVersion != null)
        {
            previousVersion.SetActive(false);
        }

        sequenceCoroutine = null;
        sequenceHasFinished = true;
    }

    private void BuildModelVersionListFromHierarchy()
    {
        modelVersions.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);

            if (child == null)
            {
                continue;
            }

            modelVersions.Add(child.gameObject);
        }
    }

    private void SetAllModelVersionsActive(bool active)
    {
        for (int i = 0; i < modelVersions.Count; i++)
        {
            if (modelVersions[i] != null)
            {
                modelVersions[i].SetActive(active);
            }
        }
    }

    private Vector3 GetTriggerCenter()
    {
        switch (triggerCenterMode)
        {
            case TriggerCenterMode.RendererBoundsCenter:
                return GetRendererBoundsCenter();

            case TriggerCenterMode.CustomOffsetFromTransform:
                return transform.TransformPoint(customTriggerCenterOffset);

            case TriggerCenterMode.ThisTransformPosition:
            default:
                return transform.position;
        }
    }

    private Vector3 GetRendererBoundsCenter()
    {
        Renderer[] childRenderers = GetComponentsInChildren<Renderer>(true);

        if (childRenderers.Length == 0)
        {
            return transform.position;
        }

        bool hasBounds = false;
        Bounds combinedBounds = new Bounds(transform.position, Vector3.zero);

        for (int i = 0; i < childRenderers.Length; i++)
        {
            Renderer childRenderer = childRenderers[i];

            if (childRenderer == null)
            {
                continue;
            }

            if (!hasBounds)
            {
                combinedBounds = childRenderer.bounds;
                hasBounds = true;
            }
            else
            {
                combinedBounds.Encapsulate(childRenderer.bounds);
            }
        }

        if (!hasBounds)
        {
            return transform.position;
        }

        return combinedBounds.center;
    }

    private void OnDrawGizmos()
    {
        if (onlyShowGizmoWhenSelected)
        {
            return;
        }

        DrawTriggerDistanceGizmo();
    }

    private void OnDrawGizmosSelected()
    {
        if (!onlyShowGizmoWhenSelected)
        {
            return;
        }

        DrawTriggerDistanceGizmo();
    }

    private void DrawTriggerDistanceGizmo()
    {
        if (!showTriggerDistanceGizmo || !usePlayerDistanceTrigger)
        {
            return;
        }

        Gizmos.color = triggerDistanceGizmoColor;
        Gizmos.DrawWireSphere(GetTriggerCenter(), playerTriggerDistance);
    }
}