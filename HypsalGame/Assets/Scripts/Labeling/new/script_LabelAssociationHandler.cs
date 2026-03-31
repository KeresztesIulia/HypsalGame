using System;
using System.Collections.Generic;
using UnityEngine;

public class script_LabelAssociationHandler : MonoBehaviour, interface_PersistentData
{
    public static script_LabelAssociationHandler Instance;
    public static bool InstanceExists => Instance != null;
    
    List<LabelAssociation> associations;
    bool initialized = false;

    public List<LabelAssociation> Associations => associations;

    private void Start()
    {
        if (!initialized) Initialize();
    }
    public void Initialize()
    {
        Instance = this;
        associations = new List<LabelAssociation>();
        initialized = true;
    }

    public void AddAssociation(Label label1, Label label2)
    {
        
        if (!AreAssociated(label1, label2))
        {
            DeleteAssociation(label1, label2, true);

            associations.Add(new(label1, label2));
        }
    }

    public void DeleteAssociation(Label label)
    {
        for (int i = 0; i < associations.Count; i++)
        {
            var association = associations[i];
            if (association.HasLabel(label))
            {
                associations.RemoveAt(i);
                i--;
            }
        }
    }

    public void DeleteAssociation(Label label1, Label label2, bool separate = false)
    {
        if (!separate) associations.Remove(new(label1, label2));
        else
        {
            for (int i = 0; i < associations.Count; i++)
            {
                var association = associations[i];
                if (association.HasLabel(label1))
                {
                    associations.RemoveAt(i);
                    i--;
                }
                else if (association.HasLabel(label2))
                {
                    associations.RemoveAt(i);
                    i--;
                }

            }
        }
    }

    public bool AreAssociated(Label label1, Label label2)
    {
        return associations.Contains(new(label1, label2));
    }

    public bool HasAssociation(Label label)
    {
        return FindAssociation(label) != null;
    }

    public LabelAssociation FindAssociation(Label label)
    {
        foreach (var association in associations)
        {
            if (association.HasLabel(label)) return association;
        }
        return null;
    }

    public Label FindAssociatedLabel(Label label)
    {
        return FindAssociation(label)?.AssociatedLabel(label);
    }
}

[System.Serializable]
public class LabelAssociation : IEquatable<LabelAssociation>
{
    public Label label1;
    public Label label2;

    public LabelAssociation(Label label1, Label label2)
    {
        this.label1 = label1;
        this.label2 = label2;
    }

    public bool HasLabel(Label label)
    {
        return label1 == label || label2 == label;
    }

    public Label AssociatedLabel(Label label)
    {
        if (!HasLabel(label)) return null;
        return label == label1 ? label2 : label1;
    }

    public static bool operator ==(LabelAssociation left, LabelAssociation right)
    {
        if (left is null && right is null) return true;
        if (left is null && right is not null) return false;
        if (left is not null && right is null) return false;

        return (left.label1 == right.label1 && left.label2 == right.label2)
            || (left.label1 == right.label2 && left.label2 == right.label1); 
    }

    public static bool operator !=(LabelAssociation left, LabelAssociation right)
    {
        return !(left == right);
    }

    public bool Equals(LabelAssociation other)
    {
        return this == other;
    }
}
