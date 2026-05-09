using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SunRotationManager : MonoBehaviour
{
    public static SunRotationManager Instance { get; private set; }

    [Header("Lookup")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string sunTag = "Sun";

    [Header("Sun Rotation At Game Start")]
    [SerializeField] private float gameStartRotationX;
    [SerializeField] private float gameStartRotationY;
    [SerializeField] private float gameStartRotationZ;
    [SerializeField] private bool forceGameStartRotationOnPlay = true;

    [Header("Sun Rotation In Editor Scene View")]
    [SerializeField] private float editorSceneRotationX;
    [SerializeField] private float editorSceneRotationY;
    [SerializeField] private float editorSceneRotationZ;
    [SerializeField] private bool previewEditorSceneRotationInEditMode = true;

    [Header("Locked Zone Behavior")]
    [SerializeField] private bool allowSeamlessOverrideOfLockedZones = true;

    [Header("Debug")]
    [SerializeField] private bool debugLogs = true;

    private readonly List<SunRotationZone> zones = new List<SunRotationZone>();

    private Transform playerTransform;
    private Transform sunTransform;
    private Light sunLight;

    private Quaternion gameStartRotation;
    private Quaternion editorSceneRotation;

    private Color gameStartEmissionFilter;
    private float gameStartEmissionTemperature;
    private bool gameStartUseColorTemperature;

    private SunRotationZone lockedZone;

    private void Awake()
    {
        if (Application.isPlaying)
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("[SunRotationManager] Duplicate manager found. Destroying this one.", this);
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        CacheRotations();
    }

    private void OnEnable()
    {
        CacheRotations();

        if (!Application.isPlaying && previewEditorSceneRotationInEditMode)
        {
            TryFindSunInEditor();
            ApplyEditorSceneRotation();
        }
    }

    private void OnValidate()
    {
        CacheRotations();

        if (!Application.isPlaying && previewEditorSceneRotationInEditMode)
        {
            TryFindSunInEditor();
            ApplyEditorSceneRotation();
        }
    }

    private void Start()
    {
        if (!Application.isPlaying)
            return;

        GameObject playerObject = GameObject.FindWithTag(playerTag);
        if (playerObject == null)
        {
            Debug.LogError($"[SunRotationManager] No active GameObject with tag '{playerTag}' was found.", this);
            enabled = false;
            return;
        }

        playerTransform = playerObject.transform;

        GameObject sunObject = GameObject.FindWithTag(sunTag);
        if (sunObject == null)
        {
            Debug.LogError($"[SunRotationManager] No active GameObject with tag '{sunTag}' was found.", this);
            enabled = false;
            return;
        }

        ResolveSunReferences(sunObject);

        if (sunTransform == null)
        {
            Debug.LogError("[SunRotationManager] Sun object was found, but no valid transform could be resolved.", this);
            enabled = false;
            return;
        }

        CacheGameStartEmission();

        if (forceGameStartRotationOnPlay)
        {
            sunTransform.rotation = gameStartRotation;
        }

        SunRotationZone[] discoveredZones = FindObjectsByType<SunRotationZone>(FindObjectsSortMode.None);
        for (int i = 0; i < discoveredZones.Length; i++)
        {
            RegisterZone(discoveredZones[i]);
        }

        if (debugLogs)
        {
            Debug.Log($"[SunRotationManager] Player found: {playerObject.name}", this);
            Debug.Log($"[SunRotationManager] Sun found: {sunObject.name}", this);
            Debug.Log($"[SunRotationManager] Registered zones: {zones.Count}", this);

            if (sunLight == null)
            {
                Debug.LogWarning("[SunRotationManager] No Light component found on the Sun object or its children. Emission filter and temperature will not be controlled.", this);
            }
        }
    }

    private void Update()
    {
        if (!Application.isPlaying)
            return;

        if (playerTransform == null || sunTransform == null)
            return;

        Vector3 playerPosition = playerTransform.position;

        Quaternion baseRotation = gameStartRotation;
        Color baseEmissionFilter = gameStartEmissionFilter;
        float baseEmissionTemperature = gameStartEmissionTemperature;
        bool baseUsesColorTemperature = gameStartUseColorTemperature;

        int minimumPriority = int.MinValue;

        if (lockedZone != null)
        {
            baseRotation = lockedZone.DesiredRotation;
            minimumPriority = lockedZone.Priority;

            if (lockedZone.ControlSunEmission)
            {
                baseEmissionFilter = lockedZone.DesiredEmissionFilter;
                baseEmissionTemperature = lockedZone.DesiredEmissionTemperature;
                baseUsesColorTemperature = true;
            }
        }

        SunRotationZone bestZone = null;
        float bestInfluence = 0f;
        int bestPriority = int.MinValue;

        for (int i = 0; i < zones.Count; i++)
        {
            SunRotationZone zone = zones[i];
            if (zone == null || !zone.isActiveAndEnabled)
                continue;

            if (zone == lockedZone)
                continue;

            float influence = zone.GetInfluence(playerPosition);
            if (influence <= 0f)
                continue;

            if (lockedZone != null)
            {
                if (!allowSeamlessOverrideOfLockedZones)
                    continue;

                if (zone.Priority <= minimumPriority)
                    continue;
            }

            if (bestZone == null)
            {
                bestZone = zone;
                bestInfluence = influence;
                bestPriority = zone.Priority;
                continue;
            }

            bool higherInfluence = influence > bestInfluence;
            bool sameInfluenceHigherPriority = Mathf.Approximately(influence, bestInfluence) && zone.Priority > bestPriority;

            if (higherInfluence || sameInfluenceHigherPriority)
            {
                bestZone = zone;
                bestInfluence = influence;
                bestPriority = zone.Priority;
            }
        }

        if (bestZone == null)
        {
            sunTransform.rotation = baseRotation;
            ApplySunEmission(baseEmissionFilter, baseEmissionTemperature, baseUsesColorTemperature);
            return;
        }

        sunTransform.rotation = Quaternion.Slerp(baseRotation, bestZone.DesiredRotation, bestInfluence);

        if (bestZone.ControlSunEmission)
        {
            Color blendedFilter = Color.Lerp(baseEmissionFilter, bestZone.DesiredEmissionFilter, bestInfluence);
            float blendedTemperature = Mathf.Lerp(baseEmissionTemperature, bestZone.DesiredEmissionTemperature, bestInfluence);

            ApplySunEmission(blendedFilter, blendedTemperature, true);
        }
        else
        {
            ApplySunEmission(baseEmissionFilter, baseEmissionTemperature, baseUsesColorTemperature);
        }

        if (bestZone.LockAtFullProximity && bestZone.IsPlayerAtFullProximity(playerPosition))
        {
            LockToZone(bestZone);
        }
    }

    private void LockToZone(SunRotationZone zone)
    {
        if (lockedZone != null && lockedZone != zone)
        {
            lockedZone.SetLocked(false);

            if (debugLogs)
            {
                Debug.Log(
                    $"[SunRotationManager] Zone '{zone.name}' replaced locked zone '{lockedZone.name}'.",
                    this
                );
            }
        }

        lockedZone = zone;
        lockedZone.SetLocked(true);
        sunTransform.rotation = lockedZone.DesiredRotation;

        if (lockedZone.ControlSunEmission)
        {
            ApplySunEmission(lockedZone.DesiredEmissionFilter, lockedZone.DesiredEmissionTemperature, true);
        }

        if (debugLogs)
        {
            Debug.Log($"[SunRotationManager] Locked to zone: {lockedZone.name} (priority {lockedZone.Priority})", this);
        }
    }

    public void RegisterZone(SunRotationZone zone)
    {
        if (zone == null)
            return;

        if (!zones.Contains(zone))
        {
            zones.Add(zone);
        }
    }

    public void UnregisterZone(SunRotationZone zone)
    {
        if (zone == null)
            return;

        if (lockedZone == zone)
        {
            lockedZone.SetLocked(false);
            lockedZone = null;
        }

        zones.Remove(zone);
    }

    public void ClearAllLocks()
    {
        if (lockedZone != null)
        {
            lockedZone.SetLocked(false);
            lockedZone = null;
        }

        for (int i = 0; i < zones.Count; i++)
        {
            if (zones[i] != null)
            {
                zones[i].SetLocked(false);
            }
        }

        if (sunTransform != null)
        {
            sunTransform.rotation = gameStartRotation;
        }

        ApplySunEmission(gameStartEmissionFilter, gameStartEmissionTemperature, gameStartUseColorTemperature);

        if (debugLogs)
        {
            Debug.Log("[SunRotationManager] All locks cleared.", this);
        }
    }

    private void CacheRotations()
    {
        gameStartRotation = Quaternion.Euler(gameStartRotationX, gameStartRotationY, gameStartRotationZ);
        editorSceneRotation = Quaternion.Euler(editorSceneRotationX, editorSceneRotationY, editorSceneRotationZ);
    }

    private void TryFindSunInEditor()
    {
        if (string.IsNullOrWhiteSpace(sunTag))
            return;

        try
        {
            GameObject sunObject = GameObject.FindWithTag(sunTag);
            if (sunObject != null)
            {
                ResolveSunReferences(sunObject);
            }
        }
        catch (UnityException)
        {
            sunTransform = null;
            sunLight = null;
        }
    }

    private void ResolveSunReferences(GameObject sunObject)
    {
        if (sunObject == null)
        {
            sunTransform = null;
            sunLight = null;
            return;
        }

        sunTransform = sunObject.transform;

        sunLight = sunObject.GetComponent<Light>();

        if (sunLight == null)
        {
            sunLight = sunObject.GetComponentInChildren<Light>(true);
        }
    }

    private void CacheGameStartEmission()
    {
        if (sunLight == null)
            return;

        gameStartEmissionFilter = sunLight.color;
        gameStartEmissionTemperature = sunLight.colorTemperature;
        gameStartUseColorTemperature = sunLight.useColorTemperature;
    }

    private void ApplySunEmission(Color emissionFilter, float emissionTemperature, bool useColorTemperature)
    {
        if (sunLight == null)
            return;

        sunLight.color = emissionFilter;
        sunLight.colorTemperature = emissionTemperature;
        sunLight.useColorTemperature = useColorTemperature;
    }

    private void ApplyEditorSceneRotation()
    {
        if (sunTransform == null)
            return;

        sunTransform.rotation = editorSceneRotation;
    }
}