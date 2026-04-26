using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class script_LabelAssociationHandler : MonoBehaviour, interface_PersistentData
{
    public static script_LabelAssociationHandler Instance;
    public static bool InstanceExists => Instance != null;
    public static UnityEvent OnAnyAssociation = new();

    Dictionary<string, AssociationData> associations;
    bool initialized = false;

    public Dictionary<string, AssociationData> Associations => associations;

    public int AssociationCount => associations.Count / 2;

    private void Awake()
    {
        if (!initialized) Initialize();
    }
    public void Initialize()
    {
        if (initialized) return;
        Instance = this;
        associations = new Dictionary<string, AssociationData>();
        initialized = true;
    }

    public void AddAssociation(Label label1, Label label2, GameObject representingModel = null)
    {
        if (!AreAssociated(label1, label2))
        {
            DeleteAssociation(label1);
            DeleteAssociation(label2);

            var associationData = new AssociationData();
            associationData.associatedLabel = label2;
            associationData.representingModel = representingModel;

            associations.Add(label1, associationData);
            label1.Associated(label2);

            if (label1 == label2) return;

            associationData.associatedLabel = label1;

            associations.Add(label2, associationData);
            label2.Associated(label1);
        }
        OnAnyAssociation?.Invoke();
    }

    public void DeleteAssociation(Label label)
    {
        if (associations.ContainsKey(label))
        {
            var association = associations[label].associatedLabel;

            associations.Remove(label);
            label.Unlabeled?.Invoke();

            associations.Remove(association);
            association.Unlabeled?.Invoke();
        }

    }

    public void DeleteAssociation(Label label1, Label label2)
    {
        DeleteAssociation(label1);
        DeleteAssociation(label2);
    }

    public bool AreAssociated(string label1, string label2)
    {
        return associations.ContainsKey(label1) && associations[label1].associatedLabel == label2;
    }



    public bool HasAssociation(string label)
    {
        return associations.ContainsKey(label) && associations[label].associatedLabel is not null;
    }

    public AssociationData FindAssociation(string label)
    {
        return associations.ContainsKey(label) ? associations[label] : new AssociationData();
    }

    public Label FindAssociatedLabel(string label)
    {
        return FindAssociation(label).associatedLabel;
    }

    public GameObject FindRepresentingModel(string label)
    {
        return FindAssociation(label).representingModel;
    }   
}

public struct AssociationData
{
    public Label associatedLabel;
    public GameObject representingModel;
}
