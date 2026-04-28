using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class script_ForceChangeAssociation : MonoBehaviour
{
    [SerializeField] script_so_LabelList _labelList;

    [SerializeField, Tooltip("Label to change association for.")]
    string _label;
    Label label;

    [SerializeField, Tooltip("(optional) New label to associate first one with. If left empty, it will simply change the model for the currently existing association, if it exists.")]
    string _associatedLabel;
    Label associatedLabel;

    [SerializeField] bool _fireAssociationEvents;

    [SerializeField, Tooltip("This includes keeping the model as none, if that is the original!")] bool _keepOriginalModel;
    [SerializeField, Tooltip("The new model to set for the association. If left empty, the model will be set to nothing as well!")]
    GameObject _newModel = null;

    [SerializeField, Tooltip("Should the association be reset every time the player passes the trigger(s)?")]
    bool _addOnce = true;

    Rigidbody rb;
    bool added = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeAll;

        label = _labelList?.GetLabel(_label);
        associatedLabel = _labelList?.GetLabel(_associatedLabel);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_addOnce && added) return;
        if (!other.CompareTag("Player")) return;

        if (script_LabelAssociationHandler.Instance == null) return;

        if (_keepOriginalModel)
        {
            _newModel = script_LabelAssociationHandler.Instance.FindRepresentingModel(_label);
            if (_newModel == null && _associatedLabel != null)
            {
                _newModel = script_LabelAssociationHandler.Instance.FindRepresentingModel(_associatedLabel);
            }
        }

        Debug.Log($"Changing {_label} and {_associatedLabel} association model to {_newModel}");

        script_LabelAssociationHandler.Instance.ChangeAssociation(label, _newModel, associatedLabel, _fireAssociationEvents);

        added = true;
    }
}
