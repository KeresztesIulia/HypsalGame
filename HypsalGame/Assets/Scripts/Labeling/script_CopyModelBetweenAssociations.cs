using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class script_CopyModelBetweenAssociations : MonoBehaviour
{
    [SerializeField] script_so_LabelList _labelList;

    [SerializeField, Tooltip("Label to take model from.")]
    string _sourceLabel;

    [SerializeField, Tooltip("Label to change model for.")]
    string _destinationLabel;

    [SerializeField, Tooltip("If source label doesn't have a model or association, don't change the destination label's model.")]
    bool _keepOriginalOnEmpty = true;

    [SerializeField] bool _copyOnce = true;
    bool copied = false;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_copyOnce && copied) return;
        if (!other.CompareTag("Player")) return;
        if (script_LabelAssociationHandler.Instance == null) return;

        var model = script_LabelAssociationHandler.Instance.FindRepresentingModel(_sourceLabel);
        if (!_keepOriginalOnEmpty)
        {
            script_LabelAssociationHandler.Instance.ChangeModel(_destinationLabel, model);
        }
        else if (model != null)
        {
            script_LabelAssociationHandler.Instance.ChangeModel(_destinationLabel, model);

        }

        copied = true;
    }
}