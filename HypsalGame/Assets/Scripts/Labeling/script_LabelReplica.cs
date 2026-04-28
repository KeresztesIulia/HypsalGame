using Unity.VisualScripting;
using UnityEngine;

public class script_LabelReplica : MonoBehaviour
{
    [SerializeField] script_so_LabelList _labelList;

    [SerializeField] Transform[] _replicaParents;
    [SerializeField] string[] _replicatedLabels;
    [SerializeField, Tooltip("Only works if the parent objects are LabelRepresentatives.")]
    bool _replicateLabelRepresentation;
    [SerializeField, Tooltip("If 'Replicate Label Representation' is set to true, this determines whether the replica will represent the replicated label, or the label associated with it.")]
    bool _representAssociatedLabel;

    [SerializeField] Collider[] _triggerAreas;
    [SerializeField] bool _triggerOnce = true;

    bool triggered = false;


    private void Start()
    {
        foreach (var triggerArea in _triggerAreas)
        {
            var replicationTrigger = triggerArea.GetComponent<script_TriggerEventConnector>();

            if (replicationTrigger != null) continue;

            replicationTrigger = triggerArea.AddComponent<script_TriggerEventConnector>();
            replicationTrigger.TriggerEntered += ReplicateItems;
        }
    }

    public void ReplicateItems()
    {
        if (_triggerOnce && triggered) return;
        if (!script_LabelAssociationHandler.InstanceExists) return;

        int currentParent = 0;
        foreach (string replicatedLabel in _replicatedLabels)
        {
            if (currentParent >= _replicaParents.Length) break;

            if (script_LabelAssociationHandler.Instance.HasAssociation(replicatedLabel))
            {
                var currentParentTransform = _replicaParents[currentParent];
                GameObject representingModel = script_LabelAssociationHandler.Instance.FindRepresentingModel(replicatedLabel);
                Instantiate(representingModel, currentParentTransform).transform.localPosition = Vector3.zero;

                if (_replicateLabelRepresentation)
                {
                    var currentLabelRepresentative = currentParentTransform.GetComponent<script_LabelRepresentative>();
                    if (currentLabelRepresentative != null)
                    {
                        currentLabelRepresentative.SetRepresentingModel(representingModel);
                        if (_representAssociatedLabel)
                        {
                            currentLabelRepresentative.SetRepresentedLabel(script_LabelAssociationHandler.Instance.FindAssociatedLabel(replicatedLabel));
                        }
                        else
                        {
                            currentLabelRepresentative.SetRepresentedLabel(replicatedLabel);
                        }
                    }
                }

                currentParent++;
            }
        }
        triggered = true;
    }


}
