using System;
using TMPro;
using UnityEngine;

public class script_FreeWritePopup : script_FreeWriteField, interface_PersistentData
{
    public static script_FreeWritePopup Instance;


    public override void Activate(Action<string> OnSubmitCallback, Action OnEscCallback)
    {
        base.Activate(OnSubmitCallback, OnEscCallback);
        script_ui_LabelLog.Instance?.SetLogVisibility(true);
    }

    public override void Deactivate(bool disableObject = false)
    {
        script_ui_LabelLog.Instance?.SetLogVisibility(false);
        base.Deactivate(true);

    }

    private void OnDisable()
    {
        gameObject.SetActive(false);
    }

    public void Initialize()
    {
        Instance = this;
    }
}
