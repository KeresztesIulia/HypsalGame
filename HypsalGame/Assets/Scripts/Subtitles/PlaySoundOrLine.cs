using System;
using System.Collections;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

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
    [SerializeField] private string subtitleText;
    [SerializeField] private float subtitleDuration = 3f;
    [SerializeField] private float subtitleDelay = 0f; // Delay before the subtitle appears
    [SerializeField] SubtitleManager.SubtitleType subtitleType;

    [Header("Conversation control")]
    [SerializeField, Tooltip("Which sound and subtitle should be played next? (overwritten by Conversation objects)")] PlaySoundOrLine nextSound;
    [SerializeField, Tooltip("Should this close the conversation in the log? (can be true even if nextSound is set)")] bool endConversation;



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
        

        if (!string.IsNullOrEmpty(subtitleText) && SubtitleManager.Instance != null)
        {
            StartCoroutine(ShowSubtitleWithDelay());
            if (nextSound != null) Invoke(nameof(PlayNextSound), subtitleDelay + subtitleDuration - 0.05f);
        }
    }

    private IEnumerator ShowSubtitleWithDelay()
    {
        yield return new WaitForSeconds(subtitleDelay); // Wait before showing the subtitle
        if (script_ui_LabelLog.Instance != null) script_ui_LabelLog.LogSubtitle(subtitleText, subtitleType, endConversation);
        SubtitleManager.Instance.ShowSubtitle(subtitleType, subtitleText, subtitleDuration);
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
