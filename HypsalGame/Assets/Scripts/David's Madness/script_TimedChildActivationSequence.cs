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

    [Tooltip("If true, objects are processed in the exact order they are listed below. If false, they are processed randomly.")]
    [SerializeField] private bool useListedOrder = true;

    [Tooltip("Total time, in seconds, for all included child objects to finish activating or deactivating.")]
    [Min(0)]
    [SerializeField] private int durationSeconds = 5;

    [Tooltip("Controls how quickly objects are processed across the duration. Use a curve that starts at 0 and ends at 1.")]
    [SerializeField] private AnimationCurve activationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Children To Include")]
    [Tooltip("Objects are processed in this exact order when Use Listed Order is enabled. Empty parent objects are ignored, but their descendants are included at that position.")]
    [SerializeField] private List<GameObject> childObjects = new List<GameObject>();

    private readonly List<GameObject> resolvedTargets = new List<GameObject>();

    private bool sequenceStarted = false;
    private TriggerRelay registeredRelay;

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

    private void OnDisable()
    {
        if (registeredRelay != null)
        {
            registeredRelay.Unregister(this);
            registeredRelay = null;
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

        relay.Register(this);
        registeredRelay = relay;
    }

    private void ResolveTargets()
    {
        resolvedTargets.Clear();
        HashSet<GameObject> seen = new HashSet<GameObject>();

        for (int i = 0; i < childObjects.Count; i++)
        {
            GameObject listedObject = childObjects[i];

            if (listedObject == null)
            {
                continue;
            }

            if (listedObject == gameObject)
            {
                continue;
            }

            if (!listedObject.transform.IsChildOf(transform))
            {
                Debug.LogWarning($"'{listedObject.name}' is not a child of '{name}' and will be ignored.", this);
                continue;
            }

            AddListedObjectInInspectorOrder(listedObject.transform, seen);
        }
    }

    private void AddListedObjectInInspectorOrder(Transform listedTransform, HashSet<GameObject> seen)
    {
        if (listedTransform == null)
        {
            return;
        }

        GameObject listedObject = listedTransform.gameObject;

        if (IsEmptyParentObject(listedObject))
        {
            AddDescendantsFromEmptyParent(listedTransform, seen);
            return;
        }

        AddTargetIfValid(listedObject, seen);
    }

    private void AddDescendantsFromEmptyParent(Transform emptyParent, HashSet<GameObject> seen)
    {
        foreach (Transform child in emptyParent)
        {
            if (child == null)
            {
                continue;
            }

            GameObject childObject = child.gameObject;

            if (IsEmptyParentObject(childObject))
            {
                AddDescendantsFromEmptyParent(child, seen);
            }
            else
            {
                AddTargetIfValid(childObject, seen);
            }
        }
    }

    private void AddTargetIfValid(GameObject target, HashSet<GameObject> seen)
    {
        if (target == null)
        {
            return;
        }

        if (target == gameObject)
        {
            return;
        }

        if (!target.transform.IsChildOf(transform))
        {
            return;
        }

        if (seen.Add(target))
        {
            resolvedTargets.Add(target);
        }
    }

    private bool IsEmptyParentObject(GameObject go)
    {
        if (go == null)
        {
            return false;
        }

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

            int shouldBeChangedByNow = Mathf.Clamp(
                Mathf.FloorToInt(normalizedProgress * totalTargets),
                0,
                totalTargets
            );

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
    private readonly List<TimedChildActivationSequence> registeredSequences = new List<TimedChildActivationSequence>();

    public void Register(TimedChildActivationSequence sequence)
    {
        if (sequence == null)
        {
            return;
        }

        if (!registeredSequences.Contains(sequence))
        {
            registeredSequences.Add(sequence);
        }
    }

    public void Unregister(TimedChildActivationSequence sequence)
    {
        if (sequence == null)
        {
            return;
        }

        registeredSequences.Remove(sequence);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player"))
        {
            return;
        }

        StartAllRegisteredSequences();
    }

    private void StartAllRegisteredSequences()
    {
        for (int i = registeredSequences.Count - 1; i >= 0; i--)
        {
            TimedChildActivationSequence sequence = registeredSequences[i];

            if (sequence == null)
            {
                registeredSequences.RemoveAt(i);
                continue;
            }

            sequence.BeginSequence();
        }
    }
}