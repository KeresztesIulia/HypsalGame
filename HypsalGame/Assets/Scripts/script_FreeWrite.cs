using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class script_FreeWrite : script_Interactable
{
    [SerializeField] script_FreeWriteField _freeWriteField;
    [SerializeField] FreeWriteInfo _freeWriteInfo;

    [SerializeField] UnityEvent CorrectPasswordEntered;
    [SerializeField] UnityEvent WrongPasswordEntered;

    bool submitClosing = false;
    bool correctAnswer = false;

    public override void Interact(Vector3 playerPosition)
    {
        submitClosing = false;

        ActivateField();

        StartCoroutine(StartLogging());
    }

    void ActivateField()
    {
        script_FreeWriteField field = _freeWriteField; // == null ? _freeWriteField : StaticPopup
        _freeWriteField.Activate(OnSubmit, OnClose);
    }

    public void DisableWriting()
    {
        StopLogging();
        enabled = false;
    }

    void OnSubmit(string enteredPassword)
    {
        Debug.Log("Submitted");
        submitClosing = true;
        //StopLogging(); // don't stop if keepOpen && allowMultiple -- stop only on correct
        if (string.IsNullOrEmpty(_freeWriteInfo.password) || enteredPassword == _freeWriteInfo.password)
        {
            CorrectPasswordEntered?.Invoke();
            correctAnswer = true;
        }
        else
        {
            WrongPasswordEntered?.Invoke();
            correctAnswer = false;
        }

        if (_freeWriteInfo.logAnswer) script_ui_LabelLog.LogPlayerText(enteredPassword);
    }

    void OnClose()
    {
        if (submitClosing)
        {
            Debug.Log("submit Closing");
            if (!_freeWriteInfo.allowMultipleTries) DisableWriting();
            else if (_freeWriteInfo.keepOpenOnSubmit)
            {
                if (!correctAnswer)
                {
                    // delete current text
                    ActivateField();
                }
            }
        }

        submitClosing = false;
        StopLogging();
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
        [Tooltip("Should the free-writing field stay selected on submit?")] public bool keepOpenOnSubmit;
        public bool logAnswer;
        [Tooltip("Leave empty to disable")] public string password;
        public string textToLogAfterTimer;
        [Tooltip("Time in seconds before the text to log appears; -1 to disable")] public float loggingDelay;
        public bool logOnce;
        [HideInInspector] public bool logged;
    }
}


