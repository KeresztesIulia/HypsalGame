using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class script_TimerTrigger : MonoBehaviour
{
    [SerializeField, Tooltip("Time given in seconds")] float _eventDelay;
    [SerializeField, Tooltip("Whether the delay should be based on scaled game time or realtime.")] bool _realtimeDelay;
    [SerializeField] UnityEvent _eventToPerform;

    [Header("Triggers")]
    [SerializeField, Tooltip("Entering one of these triggers will start the timer. Leave the list empty to start the timer from the start of the game.")]
    Collider[] _timerStarterTriggers;

    [SerializeField, Tooltip("(optional) Entering one of these triggers will interrupt the timer, stopping the connected event from being performed.")]
    Collider[] _interruptionTriggers;

    [SerializeField, Tooltip("(optional) Entering one of these triggers will interrupt the timer, instantly performing the connected event instead.")]
    Collider[] _instantEventTriggers;


    bool timerStarted = false;

    bool timerOver = false;

    private void Start()
    {
        timerOver = false;

        foreach (var trigger in _timerStarterTriggers)
        {
            trigger.isTrigger = true;
            var connector = trigger.AddComponent<script_TriggerEventConnector>();
            connector.TriggerEntered += () => StartCoroutine(Timer());
        }

        foreach (var trigger in _interruptionTriggers)
        {
            trigger.isTrigger = true;
            var connector = trigger.AddComponent<script_TriggerEventConnector>();
            connector.TriggerEntered += InterruptTimer;
        }

        foreach (var trigger in _instantEventTriggers)
        {
            trigger.isTrigger = true;
            var connector = trigger.AddComponent<script_TriggerEventConnector>();
            connector.TriggerEntered += InstantTrigger;
        }
    }

    void InterruptTimer()
    {
        if (!timerStarted || timerOver) return;
        StopAllCoroutines();
        timerOver = true;
    }

    void InstantTrigger()
    {
        if (!timerStarted || timerOver) return;
        InterruptTimer();
        _eventToPerform?.Invoke();
    }

    IEnumerator Timer()
    {
        if (timerStarted || timerOver) yield break;
        
        timerStarted = true;

        if (_realtimeDelay) yield return new WaitForSecondsRealtime(_eventDelay);
        else yield return new WaitForSeconds(_eventDelay);

        _eventToPerform?.Invoke();

        timerOver = true;
    }
}

