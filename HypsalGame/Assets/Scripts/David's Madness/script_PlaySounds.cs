using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class script_PlaySounds : MonoBehaviour
{
    [Serializable]
    private class SoundSubtitlePair
    {
        [Header("Sound")]
        public EventReference sound;
        public float delayBeforeSound = 0f;

        [Header("Subtitle Optional")]
        [TextArea]
        public string subtitleText;
        public float subtitleDelay = 0f;
        public float subtitleDuration = 3f;

        [Tooltip("If true, the subtitle will stay visible while this FMOD event is actually playing. This does not rely only on FMOD's reported event length.")]
        public bool useSoundLengthForSubtitleDuration = false;

        [Header("Sequence Timing")]
        [Tooltip("If true, the next entry starts after this FMOD event reports that it has stopped.")]
        public bool waitForSoundToFinish = true;

        [Tooltip("Used when Wait For Sound To Finish is false. This controls how long to wait before starting the next entry.")]
        public float secondsBeforeNextSound = 0f;

        [Tooltip("Extra pause after this entry finishes, before the next entry starts.")]
        public float delayAfterSound = 0f;
    }

    [Header("Playback")]
    [SerializeField] private SoundSubtitlePair[] sequence;

    [Tooltip("Starts the sequence automatically when the scene begins. Internally this runs from Start, not Awake, so the Subtitle Manager has time to initialize.")]
    [SerializeField] private bool playOnAwake;

    [SerializeField] private bool playOnce;
    [SerializeField] private bool playOnTrigger = true;
    [SerializeField] private bool stopOnDisable;

    [Header("Subtitles")]
    [Tooltip("Optional. If this is left empty, the script will use SubtitleManager.Instance. Assign this manually if subtitles are not appearing.")]
    [SerializeField] private SubtitleManager subtitleManager;

    [Tooltip("Used if Use Sound Length For Subtitle Duration is enabled, but the script cannot monitor the FMOD event.")]
    [SerializeField] private float fallbackSubtitleDuration = 3f;

    [Tooltip("Maximum time a subtitle is allowed to wait for a sound to finish. This prevents infinite subtitles on looping FMOD events.")]
    [SerializeField] private float soundLengthSubtitleMaximumDuration = 600f;

    [Tooltip("When Use Sound Length is enabled, the script refreshes the subtitle before the Subtitle Manager can expire it early.")]
    [SerializeField] private bool refreshSubtitleWhileSoundPlays = true;

    [Tooltip("How often the subtitle is refreshed while the sound is still playing. Use a smaller value if subtitles still disappear too early.")]
    [SerializeField] private float subtitleRefreshInterval = 1f;

    [Tooltip("The duration sent to the Subtitle Manager on each refresh. This should be longer than Subtitle Refresh Interval.")]
    [SerializeField] private float subtitleRefreshDuration = 5f;

    [Tooltip("Prints helpful subtitle warnings in the Console.")]
    [SerializeField] private bool debugSubtitleMessages = false;

    private bool soundPlayed = false;
    private Coroutine sequenceCoroutine;
    private EventInstance currentSoundInstance;
    private SubtitleManager currentSubtitleManager;
    private int activeSubtitleToken = 0;

    private readonly List<EventInstance> activeSoundInstances = new List<EventInstance>();

    private IEnumerator Start()
    {
        // Wait one frame so singleton managers have a chance to initialize first.
        yield return null;

        if (playOnAwake)
        {
            PlayThisSound();
        }
    }

    public void PlayThisSound()
    {
        if (playOnce && soundPlayed)
        {
            return;
        }

        soundPlayed = true;

        StopSound();
        sequenceCoroutine = StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        if (sequence == null || sequence.Length == 0)
        {
            sequenceCoroutine = null;
            yield break;
        }

        for (int i = 0; i < sequence.Length; i++)
        {
            SoundSubtitlePair entry = sequence[i];

            if (entry == null)
            {
                continue;
            }

            if (entry.delayBeforeSound > 0f)
            {
                yield return new WaitForSeconds(entry.delayBeforeSound);
            }

            EventInstance soundInstance = default;
            bool soundStarted = false;

            try
            {
                soundInstance = RuntimeManager.CreateInstance(entry.sound);
                soundInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
                soundInstance.start();
                currentSoundInstance = soundInstance;
                soundStarted = true;
                RegisterSoundInstance(soundInstance);
            }
            catch (Exception exception)
            {
                UnityEngine.Debug.LogWarning($"Could not play FMOD event on {gameObject.name}: {exception.Message}", this);
            }

            TryShowSubtitle(entry, soundInstance, soundStarted, i);

            if (soundStarted && entry.waitForSoundToFinish)
            {
                yield return WaitForSoundToStop(soundInstance, soundLengthSubtitleMaximumDuration);
                ReleaseSoundInstance(soundInstance);
            }
            else
            {
                if (entry.secondsBeforeNextSound > 0f)
                {
                    yield return new WaitForSeconds(entry.secondsBeforeNextSound);
                }

                if (soundStarted)
                {
                    StartCoroutine(ReleaseSoundWhenStopped(soundInstance, soundLengthSubtitleMaximumDuration));
                }
            }

            currentSoundInstance = default;

            if (entry.delayAfterSound > 0f)
            {
                yield return new WaitForSeconds(entry.delayAfterSound);
            }
        }

        sequenceCoroutine = null;
    }

    private void TryShowSubtitle(SoundSubtitlePair entry, EventInstance soundInstance, bool soundStarted, int sequenceIndex)
    {
        if (string.IsNullOrWhiteSpace(entry.subtitleText))
        {
            if (debugSubtitleMessages)
            {
                UnityEngine.Debug.Log($"Subtitle skipped on {gameObject.name}, sequence entry {sequenceIndex}: Subtitle Text is empty.", this);
            }

            return;
        }

        SubtitleManager manager = GetSubtitleManager();

        if (manager == null)
        {
            UnityEngine.Debug.LogWarning($"Subtitle could not be shown on {gameObject.name}, sequence entry {sequenceIndex}: no SubtitleManager was found. Assign the Subtitle Manager field manually, or make sure SubtitleManager.Instance is set before this sound plays.", this);
            return;
        }

        currentSubtitleManager = manager;

        if (entry.useSoundLengthForSubtitleDuration && soundStarted)
        {
            StartCoroutine(ShowSubtitleWhileSoundPlays(manager, entry.subtitleText, entry.subtitleDelay, soundInstance, sequenceIndex));
            return;
        }

        float duration = GetSubtitleDuration(entry, soundInstance, soundStarted, sequenceIndex);
        StartCoroutine(ShowSubtitleWithDelay(manager, entry.subtitleText, duration, entry.subtitleDelay));
    }

    private SubtitleManager GetSubtitleManager()
    {
        if (subtitleManager != null)
        {
            return subtitleManager;
        }

        if (SubtitleManager.Instance != null)
        {
            subtitleManager = SubtitleManager.Instance;
            return subtitleManager;
        }

        return null;
    }

    private float GetSubtitleDuration(SoundSubtitlePair entry, EventInstance soundInstance, bool soundStarted, int sequenceIndex)
    {
        float duration = Mathf.Max(0.1f, entry.subtitleDuration);

        if (!entry.useSoundLengthForSubtitleDuration)
        {
            return duration;
        }

        if (!soundStarted)
        {
            return Mathf.Max(0.1f, fallbackSubtitleDuration);
        }

        float soundLength = GetSoundLengthInSeconds(soundInstance);

        if (soundLength > 0f)
        {
            return Mathf.Max(0.1f, soundLength - entry.subtitleDelay);
        }

        float fallbackDuration = entry.subtitleDuration > 0f ? entry.subtitleDuration : fallbackSubtitleDuration;

        if (debugSubtitleMessages)
        {
            UnityEngine.Debug.LogWarning($"FMOD sound length could not be read on {gameObject.name}, sequence entry {sequenceIndex}. The subtitle is using a fallback duration of {fallbackDuration} seconds.", this);
        }

        return Mathf.Max(0.1f, fallbackDuration);
    }

    private float GetSoundLengthInSeconds(EventInstance soundInstance)
    {
        if (!soundInstance.isValid())
        {
            return 0f;
        }

        FMOD.RESULT descriptionResult = soundInstance.getDescription(out EventDescription eventDescription);

        if (descriptionResult != FMOD.RESULT.OK || !eventDescription.isValid())
        {
            return 0f;
        }

        FMOD.RESULT lengthResult = eventDescription.getLength(out int lengthMilliseconds);

        if (lengthResult != FMOD.RESULT.OK || lengthMilliseconds <= 0)
        {
            return 0f;
        }

        return lengthMilliseconds / 1000f;
    }

    private IEnumerator ShowSubtitleWithDelay(SubtitleManager manager, string text, float duration, float delay)
    {
        if (delay > 0f)
        {
            yield return new WaitForSeconds(delay);
        }

        if (manager != null)
        {
            activeSubtitleToken++;
            manager.ShowSubtitle(text, duration);
        }
    }

    private IEnumerator ShowSubtitleWhileSoundPlays(SubtitleManager manager, string text, float delay, EventInstance soundInstance, int sequenceIndex)
    {
        if (delay > 0f)
        {
            yield return new WaitForSeconds(delay);
        }

        if (manager == null || string.IsNullOrWhiteSpace(text))
        {
            yield break;
        }

        if (!soundInstance.isValid() || IsSoundStopped(soundInstance))
        {
            yield break;
        }

        int mySubtitleToken = ++activeSubtitleToken;
        float elapsed = 0f;
        float maximumDuration = Mathf.Max(1f, soundLengthSubtitleMaximumDuration);
        float refreshInterval = Mathf.Max(0.05f, subtitleRefreshInterval);
        float refreshDuration = Mathf.Max(refreshInterval + 0.05f, subtitleRefreshDuration);

        if (debugSubtitleMessages)
        {
            UnityEngine.Debug.Log($"Subtitle on {gameObject.name}, sequence entry {sequenceIndex}, is now following the actual FMOD playback state.", this);
        }

        manager.ShowSubtitle(text, refreshSubtitleWhileSoundPlays ? refreshDuration : maximumDuration);

        while (elapsed < maximumDuration)
        {
            if (mySubtitleToken != activeSubtitleToken)
            {
                yield break;
            }

            if (!soundInstance.isValid() || IsSoundStopped(soundInstance))
            {
                break;
            }

            if (refreshSubtitleWhileSoundPlays)
            {
                manager.ShowSubtitle(text, refreshDuration);
            }

            yield return new WaitForSeconds(refreshInterval);
            elapsed += refreshInterval;
        }

        if (elapsed >= maximumDuration && debugSubtitleMessages)
        {
            UnityEngine.Debug.LogWarning($"Subtitle on {gameObject.name}, sequence entry {sequenceIndex}, reached Sound Length Subtitle Maximum Duration. The FMOD event may be looping or not reporting a stopped state.", this);
        }

        if (mySubtitleToken == activeSubtitleToken)
        {
            ClearSubtitle(manager);
        }
    }

    private IEnumerator WaitForSoundToStop(EventInstance soundInstance, float maximumWaitTime)
    {
        float elapsed = 0f;
        float maxWait = Mathf.Max(1f, maximumWaitTime);

        while (elapsed < maxWait)
        {
            if (!soundInstance.isValid() || IsSoundStopped(soundInstance))
            {
                yield break;
            }

            yield return null;
            elapsed += Time.deltaTime;
        }
    }

    private IEnumerator ReleaseSoundWhenStopped(EventInstance soundInstance, float maximumWaitTime)
    {
        yield return WaitForSoundToStop(soundInstance, maximumWaitTime);
        ReleaseSoundInstance(soundInstance);
    }

    private bool IsSoundStopped(EventInstance soundInstance)
    {
        if (!soundInstance.isValid())
        {
            return true;
        }

        FMOD.RESULT result = soundInstance.getPlaybackState(out PLAYBACK_STATE playbackState);

        if (result != FMOD.RESULT.OK)
        {
            return true;
        }

        return playbackState == PLAYBACK_STATE.STOPPED;
    }

    private void RegisterSoundInstance(EventInstance soundInstance)
    {
        if (!soundInstance.isValid())
        {
            return;
        }

        activeSoundInstances.Add(soundInstance);
    }

    private void ReleaseSoundInstance(EventInstance soundInstance)
    {
        if (soundInstance.isValid())
        {
            soundInstance.release();
        }

        activeSoundInstances.RemoveAll(instance => !instance.isValid() || instance.Equals(soundInstance));
    }

    private void ClearSubtitle(SubtitleManager manager)
    {
        if (manager == null)
        {
            return;
        }

        if (TryCallSubtitleMethod(manager, "HideSubtitle"))
        {
            return;
        }

        if (TryCallSubtitleMethod(manager, "ClearSubtitle"))
        {
            return;
        }

        manager.ShowSubtitle(string.Empty, 0.1f);
    }

    private bool TryCallSubtitleMethod(SubtitleManager manager, string methodName)
    {
        MethodInfo method = manager.GetType().GetMethod(
            methodName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            null,
            Type.EmptyTypes,
            null
        );

        if (method == null)
        {
            return false;
        }

        try
        {
            method.Invoke(manager, null);
            return true;
        }
        catch (Exception exception)
        {
            if (debugSubtitleMessages)
            {
                UnityEngine.Debug.LogWarning($"Tried to call {methodName} on SubtitleManager, but it failed: {exception.Message}", this);
            }

            return false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!playOnTrigger)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            PlayThisSound();
        }
    }

    public void StopSound()
    {
        activeSubtitleToken++;

        if (sequenceCoroutine != null)
        {
            StopCoroutine(sequenceCoroutine);
            sequenceCoroutine = null;
        }

        if (currentSubtitleManager != null)
        {
            ClearSubtitle(currentSubtitleManager);
            currentSubtitleManager = null;
        }

        for (int i = activeSoundInstances.Count - 1; i >= 0; i--)
        {
            EventInstance soundInstance = activeSoundInstances[i];

            if (soundInstance.isValid())
            {
                soundInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                soundInstance.release();
            }
        }

        activeSoundInstances.Clear();
        currentSoundInstance = default;
    }

    private void OnDisable()
    {
        if (stopOnDisable)
        {
            StopSound();
        }
    }

    private void OnDestroy()
    {
        StopSound();
    }
}
