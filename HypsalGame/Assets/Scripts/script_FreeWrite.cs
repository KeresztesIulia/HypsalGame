using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class script_FreeWrite : script_Interactable
{
    [SerializeField] script_FreeWriteField _freeWriteField;
    [SerializeField] FreeWriteInfo _freeWriteInfo;

    [SerializeField] UnityEvent CorrectPasswordEntered;
    [SerializeField] UnityEvent WrongPasswordEntered;

    public override void Interact(Vector3 playerPosition)
    {
        script_FreeWriteField field = _freeWriteField; // == null ? _freeWriteField : StaticPopup
        // activate inputField/Popup
        if (_freeWriteField != null)
        {
            _freeWriteField.Activate(OnSubmit, StopLogging);
        }
        else
        {
            // activate FreeWritePopup
        }

        StartCoroutine(StartLogging());
    }

    public void DisableWriting()
    {
        StopLogging();
        enabled = false;
    }

    void OnSubmit(string enteredPassword)
    {
        StopLogging();
        if (string.IsNullOrEmpty(_freeWriteInfo.password) || enteredPassword == _freeWriteInfo.password) CorrectPasswordEntered?.Invoke();
        else WrongPasswordEntered?.Invoke();
        if (!_freeWriteInfo.allowMultipleTries) DisableWriting();
    }

    IEnumerator StartLogging()
    {
        if (_freeWriteInfo.logOnce && _freeWriteInfo.logged) yield break;
        yield return new WaitForSeconds(_freeWriteInfo.loggingDelay);
        script_ui_LabelLog.LogAIText(_freeWriteInfo.textToLogAfterTimer, true);
        _freeWriteInfo.logged = true;
    }

    public void StopLogging()
    {
        StopAllCoroutines();
    }

    [System.Serializable]
    public struct FreeWriteInfo
    {
        public bool allowMultipleTries;
        [Tooltip("Leave empty to disable")] public string password;
        public string textToLogAfterTimer;
        [Tooltip("Time in seconds before the text to log appears; -1 to disable")] public float loggingDelay;
        public bool logOnce;
        [HideInInspector] public bool logged;
    }
}


