using System;
using UnityEngine;

public static class static_LogTextRemodeller
{
    static int testParticipantNumber = -1;

    static int TestParticipantNumber
    {
        get
        {
            if (testParticipantNumber == -1)
            {
                UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
                testParticipantNumber = UnityEngine.Random.Range(5, 71);
            }
            return testParticipantNumber;
        }
    }


    public static string RemodelText(string text)
    {
        string[] strings = text.Split('[', ']');
        for (int i = 1; i < strings.Length; i+= 2)
        {
            strings[i] = RemodelWord(strings[i]);
        }
        return string.Join("", strings);
    }

    public static string RemodelWord(string word)
    {
        switch(word)
        {
            case "test_participant":
                return TestParticipantNumber.ToString();
            default:
                if (word.StartsWith("label_"))
                {
                    if (!script_LabelAssociationHandler.InstanceExists) return "[can't find labelAssociationHandler]";
                    return script_LabelAssociationHandler.Instance.FindAssociatedLabel(word.Replace("label_", ""))?.OriginalDisplayName;
                }
                if (word.StartsWith("remaining_labels: "))
                {
                    return RemainingLabels(word.Replace("remaining_labels: ", ""));
                }
                return word;
        }
    }

    static string RemainingLabels(string labelsWord)
    {
        if (!script_LabelAssociationHandler.InstanceExists) return labelsWord;

        string[] labels = labelsWord.Split(", ");

        string remainingLabels = "";

        foreach (string label in labels)
        {
            if (script_LabelAssociationHandler.Instance.HasAssociation(label)) continue;

            string labDisplayName = script_LabelAssociationHandler.Instance.FindAssociatedLabel(label).DisplayName;

            if (string.IsNullOrEmpty(remainingLabels))
                remainingLabels = labDisplayName;
            else
                string.Join(", ", remainingLabels, labDisplayName);
        }

        return remainingLabels;
    }
}
