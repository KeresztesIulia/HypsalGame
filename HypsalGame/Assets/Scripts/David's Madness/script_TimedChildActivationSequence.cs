using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedChildActivationSequence : MonoBehaviour
{
    [Header("Trigger Settings")]
    [Tooltip("Optional. If assigned, the timed sequence begins only when this trigger object detects the Player.")]
    [SerializeField] private GameObject triggerObject;

    [Header("Sequence Settings")]
    [Tooltip("If true, all included child objects start active, then become inactive over time. If false, they start inactive, then become active over time.")]
    [SerializeField] private bool startChildrenActive = true;

    [Tooltip("If true, objects are processed in the order they are listed. If false, they are processed in a random order.")]
    [SerializeField] private bool useListedOrder = true;

    [Tooltip("Total time, in seconds, for all included child objects to finish activating or deactivating.")]
    [Min(0)]
    [SerializeField] private int durationSeconds = 5;

    [Tooltip("Controls how quickly objects are processed across the duration. Use a curve that starts at 0 and ends at 1.")]
    [SerializeField] private AnimationCurve activationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Children To Include")]
    [Tooltip("Add child objects from this parent here. Empty parent objects are ignored, but their descendants are still included.")]
    [SerializeField] private List<GameObject> childObjects = new List<GameObject>();

    private readonly List<GameObject> resolvedTargets = new List<GameObject>();
    private bool sequenceStarted = false;

    private void Start()
    {
        ResolveTargets();
        ApplyStateToAllResolvedTargets(startChildrenActive);

        if (resolvedTargets.Count == 0)
        {
            return;
        }

        if (triggerObject == null)
        {
            BeginSequence();
        }
        else
        {
            SetupTriggerRelay();
        }
    }

    public void BeginSequence()
    {
        if (sequenceStarted)
        {
            return;
        }

        sequenceStarted = true;
        StartCoroutine(RunTimedSequence(!startChildrenActive));
    }

    private void SetupTriggerRelay()
    {
        TriggerRelay relay = triggerObject.GetComponent<TriggerRelay>();

        if (relay == null)
        {
            relay = triggerObject.AddComponent<TriggerRelay>();
        }

        relay.Initialize(this);
    }

    private void ResolveTargets()
    {
        resolvedTargets.Clear();
        HashSet<GameObject> seen = new HashSet<GameObject>();

        for (int i = 0; i < childObjects.Count; i++)
        {
            GameObject entry = childObjects[i];

            if (entry == null)
            {
                continue;
            }

            if (entry == gameObject)
            {
                continue;
            }

            if (!entry.transform.IsChildOf(transform))
            {
                Debug.LogWarning($"'{entry.name}' is not a child of '{name}' and will be ignored.", this);
                continue;
            }

            CollectRecursively(entry.transform, seen);
        }
    }

    private void CollectRecursively(Transform current, HashSet<GameObject> seen)
    {
        if (current == null)
        {
            return;
        }

        if (current == transform)
        {
            foreach (Transform child in current)
            {
                CollectRecursively(child, seen);
            }

            return;
        }

        bool isEmptyParentObject = IsEmptyParentObject(current.gameObject);

        if (!isEmptyParentObject && seen.Add(current.gameObject))
        {
            resolvedTargets.Add(current.gameObject);
        }

        foreach (Transform child in current)
        {
            CollectRecursively(child, seen);
        }
    }

    private bool IsEmptyParentObject(GameObject go)
    {
        if (go.transform.childCount == 0)
        {
            return false;
        }

        Component[] components = go.GetComponents<Component>();

        // Only a Transform means it is acting as an empty grouping object.
        return components.Length == 1;
    }

    private void ApplyStateToAllResolvedTargets(bool activeState)
    {
        for (int i = 0; i < resolvedTargets.Count; i++)
        {
            if (resolvedTargets[i] != null)
            {
                resolvedTargets[i].SetActive(activeState);
            }
        }
    }

    private IEnumerator RunTimedSequence(bool finalState)
    {
        int totalTargets = resolvedTargets.Count;

        if (!useListedOrder)
        {
            Shuffle(resolvedTargets);
        }

        if (durationSeconds <= 0)
        {
            ApplyStateToAllResolvedTargets(finalState);
            yield break;
        }

        int changedCount = 0;
        float elapsed = 0f;

        float curveStart = activationCurve != null ? activationCurve.Evaluate(0f) : 0f;
        float curveEnd = activationCurve != null ? activationCurve.Evaluate(1f) : 1f;

        while (elapsed < durationSeconds && changedCount < totalTargets)
        {
            float linearProgress = Mathf.Clamp01(elapsed / durationSeconds);
            float curvedValue = activationCurve != null ? activationCurve.Evaluate(linearProgress) : linearProgress;

            float normalizedProgress = Mathf.Approximately(curveStart, curveEnd)
                ? linearProgress
                : Mathf.InverseLerp(curveStart, curveEnd, curvedValue);

            int shouldBeChangedByNow = Mathf.Clamp(Mathf.FloorToInt(normalizedProgress * totalTargets), 0, totalTargets);

            while (changedCount < shouldBeChangedByNow)
            {
                GameObject target = resolvedTargets[changedCount];

                if (target != null)
                {
                    target.SetActive(finalState);
                }

                changedCount++;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        while (changedCount < totalTargets)
        {
            GameObject target = resolvedTargets[changedCount];

            if (target != null)
            {
                target.SetActive(finalState);
            }

            changedCount++;
        }
    }

    private void Shuffle(List<GameObject> list)
    {
        for (int i = 0; i < list.Count - 1; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }

    [ContextMenu("Populate With All Direct Children")]
    private void PopulateWithAllDirectChildren()
    {
        childObjects.Clear();

        foreach (Transform child in transform)
        {
            childObjects.Add(child.gameObject);
        }
    }
}

public class TriggerRelay : MonoBehaviour
{
    private TimedChildActivationSequence owner;

    public void Initialize(TimedChildActivationSequence newOwner)
    {
        owner = newOwner;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (owner == null)
        {
            return;
        }

        if (other.gameObject.CompareTag("Player"))
        {
            owner.BeginSequence();
        }
    }
}