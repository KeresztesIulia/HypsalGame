using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class script_FreeWriteField : MonoBehaviour
{
    [SerializeField] TMP_InputField _inputField;

    public TMP_InputField InputField => _inputField;

    PlayerInput playerInput;

    bool listenersAdded = false;

    private void Start()
    {
        playerInput = FindFirstObjectByType<PlayerInput>();
        _inputField.resetOnDeActivation = false;
    }

    public virtual void Activate(System.Action<string> OnSubmitCallback, System.Action OnEscCallback)
    {
        DisableControls();
        _inputField.Select();
        AddListeners(OnSubmitCallback, OnEscCallback);
    }

    void AddListeners(System.Action<string> OnSubmitCallback, System.Action OnEscCallback)
    {
        if (listenersAdded) return;
        _inputField.onSubmit.AddListener((fieldContent) => OnSubmitCallback(fieldContent));
        _inputField.onEndEdit.AddListener((cancelString) => { OnEscCallback(); Deactivate(); });
        listenersAdded = true;
    }

    void DisableControls()
    {
        gameObject.SetActive(true);
        playerInput.enabled = false;

        script_PlayerInteraction.Instance?.ResetTarget();
        script_PlayerInteraction.Instance?.SetActive(false);
    }

    public void Deactivate(bool disableObject = false)
    {
        playerInput.enabled = true;
        EventSystem.current?.SetSelectedGameObject(null);

        gameObject.SetActive(!disableObject);
        script_PlayerInteraction.Instance.SetActive(true);
    }

    public void CopyInputContent(TMP_InputField inputField)
    {
        _inputField.text = inputField.text;
    }
}
