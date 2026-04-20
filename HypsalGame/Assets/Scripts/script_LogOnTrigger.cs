using UnityEngine;

public class script_LogOnTrigger : MonoBehaviour
{
    [SerializeField] string[] _textsToLog;
    [SerializeField] script_ui_LabelLog.LogType[] _logTypes;
    [SerializeField] bool _triggerOnce = true;
    [SerializeField] bool _popUpLog = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (_textsToLog == null || _textsToLog.Length == 0) return;

        for (int i = 0; i < _textsToLog.Length; i++)
        {
            if (string.IsNullOrEmpty(_textsToLog[i])) continue;
            script_ui_LabelLog.LogExchangeElement(_logTypes[i], _textsToLog[i]);
        }

        script_ui_LabelLog.CloseExchange(_popUpLog);

        if (_triggerOnce) enabled = false;
    }
}
