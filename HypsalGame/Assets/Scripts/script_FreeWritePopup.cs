using TMPro;
using UnityEngine;

public class script_FreeWritePopup : MonoBehaviour // will inherit FreeWriteField
{
    [SerializeField] TMP_InputField _freeWriteField;

    public void Popup()
    {
        // open log
        // open freewrite
    }

    private void OnDisable()
    {
        gameObject.SetActive(false);
    }
}
