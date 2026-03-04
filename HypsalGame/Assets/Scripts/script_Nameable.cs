using UnityEngine;

public class script_Nameable : MonoBehaviour
{
    [SerializeField] script_so_NameList _nameList;
    [SerializeField] LabelableWord _representedWord;

    public void GiveName(string name)
    {
        if (_representedWord == null) return;

        _representedWord.LabelWord(name);
    }
}
