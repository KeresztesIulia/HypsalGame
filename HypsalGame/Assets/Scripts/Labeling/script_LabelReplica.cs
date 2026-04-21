using Unity.VisualScripting;
using UnityEngine;

public class script_LabelReplica : MonoBehaviour
{
    [SerializeField] script_so_LabelList _labelList;

    [SerializeField] Transform[] _replicaParents;
    [SerializeField] string[] _replicatedLabels;

    [SerializeField] Collider[] _triggerAreas;
    [SerializeField] bool _triggerOnce;

    bool triggered = false;


    private void Start()
    {
        foreach (var triggerArea in _triggerAreas)
        {
            var replicationTrigger = triggerArea.GetComponent<script_ReplicationTrigger>();

            if (replicationTrigger != null) continue;

            replicationTrigger = triggerArea.AddComponent<script_ReplicationTrigger>();
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
                GameObject representingModel = script_LabelAssociationHandler.Instance.FindRepresentingModel(replicatedLabel);
                Instantiate(representingModel, _replicaParents[currentParent]).transform.localPosition = Vector3.zero;

                currentParent++;
            }
        }
        triggered = true;
    }


}
