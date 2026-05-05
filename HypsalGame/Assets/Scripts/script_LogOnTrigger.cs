using UnityEngine;

public class script_LogOnTrigger : MonoBehaviour
{
    [SerializeField, Tooltip("(optional) Only if you use special logging for remaining labels.")] script_so_LabelList _specialLogLabelList;

    [SerializeField] string[] _textsToLog;
    [SerializeField] script_ui_LabelLog.LogType[] _logTypes;
    [SerializeField] bool _triggerOnce = true;
    [SerializeField] bool _popUpLog = true;

    bool triggered = false;

    public void LogTexts()
    {
        if (_textsToLog == null || _textsToLog.Length == 0) return;

        for (int i = 0; i < _textsToLog.Length; i++)
        {
            if (string.IsNullOrEmpty(_textsToLog[i])) continue;
            script_ui_LabelLog.LogExchangeElement(_logTypes[i], static_LogTextRemodeller.RemodelText(_textsToLog[i], _specialLogLabelList));
        }

        script_ui_LabelLog.CloseExchange(_popUpLog);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (_textsToLog == null || _textsToLog.Length == 0) return;
        if (_triggerOnce && triggered) return;

        LogTexts();
        triggered = true;
    }
}
