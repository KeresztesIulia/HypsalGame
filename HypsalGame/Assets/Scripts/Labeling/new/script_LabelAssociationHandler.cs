using System;
using System.Collections.Generic;
using UnityEngine;

public class script_LabelAssociationHandler : MonoBehaviour, interface_PersistentData
{
    public static script_LabelAssociationHandler Instance;
    public static bool InstanceExists => Instance != null;

    Dictionary<string, Label> associations;
    bool initialized = false;

    public Dictionary<string, Label> Associations => associations;

    private void Start()
    {
        if (!initialized) Initialize();
    }
    public void Initialize()
    {
        Instance = this;
        associations = new Dictionary<string, Label>();
        initialized = true;
    }

    public void AddAssociation(Label label1, Label label2)
    {
        
        if (!AreAssociated(label1, label2))
        {
            DeleteAssociation(label1);
            DeleteAssociation(label2);

            associations.Add(label1, label2);

            if (label1 == label2) return;

            associations.Add(label2, label1);
        }
    }

    public void DeleteAssociation(Label label)
    {
        if (associations.ContainsKey(label))
        {
            var association = associations[label];

            associations.Remove(label);

            associations.Remove(association);
        }

    }

    public void DeleteAssociation(Label label1, Label label2)
    {
        DeleteAssociation(label1);
        DeleteAssociation(label2);
    }

    public bool AreAssociated(Label label1, Label label2)
    {
        return associations.ContainsKey(label1) && associations[label1] == label2;
    }

    public bool HasAssociation(Label label)
    {
        return associations.ContainsKey(label) && associations[label] is not null;
    }

    public Label FindAssociatedLabel(Label label)
    {
        return associations.ContainsKey(label) ? associations[label] : null;
    }
}
