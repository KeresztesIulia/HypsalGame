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
        _inputField.onEndEdit.AddListener((cancelString) => { Deactivate(); OnEscCallback(); });
        listenersAdded = true;
    }

    void DisableControls()
    {
        gameObject.SetActive(true);

        script_PlayerInteraction.Instance?.ResetTarget();
        script_PlayerInteraction.Instance?.SetActive(false);
        script_InputManager.SwitchInputMap(script_InputManager.map_uiMap);
    }

    public void Deactivate(bool disableObject = false)
    {
        Debug.Log("Deactivate");
        EventSystem.current?.SetSelectedGameObject(null);

        gameObject.SetActive(!disableObject);
        script_PlayerInteraction.Instance.SetActive(true);
        script_InputManager.SwitchInputMap(script_InputManager.map_PlayerMap);

    }

    public void CopyInputContent(TMP_InputField inputField)
    {
        _inputField.text = inputField.text;
    }
}
