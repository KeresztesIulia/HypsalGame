using UnityEngine;

public class script_LabelListResetter : MonoBehaviour
{
    [SerializeField] script_so_LabelList[] _listsToReset;

    void Awake()
    {
        foreach (var list in _listsToReset) list.Reset();
    }
}
