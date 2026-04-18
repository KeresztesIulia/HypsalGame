using UnityEngine;

public class script_Conversation : MonoBehaviour
{
    [SerializeField] PlaySound[] _lines;
    [SerializeField] bool _playOnce = true;

    private void Awake()
    {
        if (_lines == null || _lines.Length == 0)
        {
            _lines = GetComponentsInChildren<PlaySound>();    
        }

        _lines[0].SetPlayOnce(_playOnce);
        _lines[_lines.Length - 1].SetNextSound(null, true);

        for (int i = 0; i < _lines.Length; i++)
        {
            if (i != 0) _lines[i].DisablePlayTriggers();

            if (i == _lines.Length - 1) continue;
            _lines[i].SetNextSound(_lines[i + 1]);
        }
    }
}
