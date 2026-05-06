using System;
using System.Collections;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.Events;

public class PlaySoundOrLine : MonoBehaviour
{
    [SerializeField] private EventReference sound;

    [Header("Playback options")]
    [SerializeField, Tooltip("Overwritten by ConversationObjects")] private bool playOnAwake;
    [SerializeField, Tooltip("Overwritten by ConversationObjects")] private bool playOnce;
    [SerializeField, Tooltip("Overwritten by ConversationObjects")] private bool playOnTrigger = true;
    [SerializeField] private bool stopOnDisable;
    [SerializeField] bool uninterruptable = false;

    private bool soundPlayed = false;

    private EventInstance soundInstance;

    [Header("Subtitle")]
    [SerializeField] script_so_LabelList _specialSubtitlesLabelList;
    [SerializeField] private string subtitleText;
    [SerializeField] private float subtitleDuration = 3f;
    [SerializeField] private float subtitleDelay = 0f; // Delay before the subtitle appears
    [SerializeField] SubtitleManager.SubtitleType subtitleType;

    [Header("Conversation control")]
    [SerializeField, Tooltip("Which sound and subtitle should be played next? (overwritten by Conversation objects)")] PlaySoundOrLine nextSound;
    [SerializeField, Tooltip("Should this close the conversation in the log? (can be true even if nextSound is set)")] bool endConversation;
    [SerializeField] bool dontLog = false;


    public UnityEvent OnPlaybackEnd;

    static Action startPlaying;

    private void Awake()
    {
        startPlaying += InterruptFrom;
        if (playOnAwake)
        {
            PlayThisSound();
        }
    }

    public void PlayThisSound()
    {
        startPlaying.Invoke();

        try
        {
            soundInstance = RuntimeManager.CreateInstance(sound);
            soundInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
            soundInstance.start();
        }
        catch
        {

        }

        if (!soundPlayed && script_EndGameLogger.Instance != null)
        {
            if (soundInstance.isValid())
            {
                if (soundInstance.getDescription(out var desc) == FMOD.RESULT.OK)
                {

                    if (desc.getPath(out var path) == FMOD.RESULT.OK)
                        script_EndGameLogger.Instance.AddTriggeredSound(path.Replace("event:/", ""));
                }
            }
        }

        if (!string.IsNullOrEmpty(subtitleText) && SubtitleManager.Instance != null)
        {
            StartCoroutine(ShowSubtitleWithDelay());
            if (nextSound != null) Invoke(nameof(PlayNextSound), subtitleDelay + subtitleDuration - 0.05f);
        }

        StartCoroutine(InvokeEndEvent());
    }

    private IEnumerator ShowSubtitleWithDelay()
    {
        yield return new WaitForSeconds(subtitleDelay); // Wait before showing the subtitle
        string remodelledSubtitleText = static_LogTextRemodeller.RemodelText(subtitleText, _specialSubtitlesLabelList);
        if (!dontLog && script_ui_LabelLog.Instance != null) script_ui_LabelLog.LogSubtitle(remodelledSubtitleText, subtitleType, endConversation);
        SubtitleManager.Instance.ShowSubtitle(subtitleType, remodelledSubtitleText, subtitleDuration);
    }

    private IEnumerator InvokeEndEvent()
    {
        yield return new WaitForSeconds(subtitleDelay + subtitleDuration);
        OnPlaybackEnd?.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!playOnTrigger) return;

        if (playOnce && soundPlayed)
        {
            return;
        }
        else if (other.CompareTag("Player")) // Ensure the player has the tag "Player"
        {
            PlayThisSound();
            soundPlayed = true;
        }
    }

    void InterruptFrom()
    {
        if (!uninterruptable) StopAll();
    }

    void StopAll()
    {
        StopSound();
        StopAllCoroutines();
        CancelInvoke();
    }

    public void StopSound()
    {
        if (soundInstance.isValid())
        {
            soundInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            soundInstance.release();
        }
    }

    public void SetNextSound(PlaySoundOrLine nextSound, bool endConversation = false)
    {
        this.nextSound = nextSound;
        this.endConversation = endConversation;
    }

    public void DisablePlayTriggers()
    {
        playOnAwake = false;
        playOnTrigger = false;
    }

    public void SetPlayOnce(bool playOnce)
    {
        this.playOnce = playOnce;
    }

    void PlayNextSound()
    {
        nextSound.PlayThisSound();
    }


    private void OnDisable()
    {
        if (stopOnDisable) StopSound();
    }

    private void OnDestroy()
    {
        StopSound();
    }

}
