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

    public void AddAssociation(Label label1, Label label2, GameObject representingModel = null, bool fireEvents = true)
    {
        if (!AreAssociated(label1, label2))
        {
            DeleteAssociation(label1, label2, fireEvents);

            var associationData = new AssociationData();
            associationData.associatedLabel = label2;
            associationData.representingModel = representingModel;

            associations.Add(label1, associationData);
            if (fireEvents) label1.Associated(label2);

            if (label1 == label2) return;

            associationData.associatedLabel = label1;

            associations.Add(label2, associationData);
            if (fireEvents) label2.Associated(label1);
        }
        if (fireEvents) OnAnyAssociation?.Invoke();
    }


    public void DeleteAssociation(Label label, bool fireEvents = true)
    {
        if (associations.ContainsKey(label))
        {
            var association = associations[label].associatedLabel;

            associations.Remove(label);
            if (fireEvents) label.Unlabeled?.Invoke();

            associations.Remove(association);
            if (fireEvents) association.Unlabeled?.Invoke();
        }

    }

    public void DeleteAssociation(Label label1, Label label2, bool fireEvents = true)
    {
        DeleteAssociation(label1, fireEvents);
        DeleteAssociation(label2, fireEvents);
    }

    public void ChangeAssociation(Label label, GameObject newModel, Label associatedLabel = null, bool fireEvents = false)
    {
        if (!HasAssociation(label) && associatedLabel == null) return;

        if (associatedLabel == null)
        {
            ChangeModel(label, newModel);
        }
        else
        {
            if (AreAssociated(label, associatedLabel)) ChangeModel(label, associatedLabel, newModel);
            else
            {
                AddAssociation(label, associatedLabel, newModel, fireEvents);
            }
        }
    }

    public void ChangeModel(Label label, GameObject newModel)
    {
        if (!HasAssociation(label)) return;

        ChangeModel(label.InternalName, newModel);
    }

    public void ChangeModel(string label, GameObject newModel)
    {
        if (!HasAssociation(label)) return;

        ChangeModel(label, FindAssociatedLabel(label), newModel);
    }

    public void ChangeModel(Label label1, Label label2, GameObject newModel)
    {
        if (!AreAssociated(label1, label2)) return;

        ChangeModel(label1.InternalName, label2, newModel);
    }

    public void ChangeModel(string label1, string label2, GameObject newModel)
    {
        if (!AreAssociated(label1, label2)) return;

        var association = FindAssociation(label1);
        association.representingModel = newModel;
        associations[label1] = association;

        association = FindAssociation(label2);
        association.representingModel = newModel;
        associations[label2] = association;
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
