using System;
using System.Collections;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    [SerializeField] private EventReference sound;

    [SerializeField] private bool playOnAwake;
    [SerializeField] private bool playOnce;
    [SerializeField] private bool playOnTrigger = true;
    [SerializeField] private bool stopOnDisable;
    private bool soundPlayed = false;

    private EventInstance soundInstance;

    [Header("Subtitle (Optional)")]
    [SerializeField] private string subtitleText;
    [SerializeField] private float subtitleDuration = 3f;
    [SerializeField] private float subtitleDelay = 0f; // Delay before the subtitle appears

    private void Awake()
    {
        if (playOnAwake)
        {
            PlayThisSound();
        }
    }

    public void PlayThisSound()
    {
        StopSound();

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
        }
    }

    private IEnumerator ShowSubtitleWithDelay()
    {
        yield return new WaitForSeconds(subtitleDelay); // Wait before showing the subtitle
        SubtitleManager.Instance.ShowSubtitle(subtitleText, subtitleDuration);
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

    public void StopSound()
    {
        if (soundInstance.isValid())
        {
            soundInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            soundInstance.release();
        }
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
