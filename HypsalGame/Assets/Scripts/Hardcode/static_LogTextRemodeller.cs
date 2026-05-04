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


    public static string RemodelText(string text, script_so_LabelList labelList)
    {
        string[] strings = text.Split('[', ']');
        for (int i = 1; i < strings.Length; i+= 2)
        {
            strings[i] = RemodelWord(strings[i], labelList);
        }
        return string.Join("", strings);
    }

    public static string RemodelWord(string word, script_so_LabelList labelList)
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
                    return RemainingLabels(word.Replace("remaining_labels: ", ""), labelList);
                }
                return word;
        }
    }

    static string RemainingLabels(string labelsWord, script_so_LabelList labelList)
    {
        if (!script_LabelAssociationHandler.InstanceExists) return labelsWord;
        if (labelList == null) return labelsWord;

        string[] labels = labelsWord.Split(", ");

        string remainingLabels = "";

        foreach (string label in labels)
        {
            if (script_LabelAssociationHandler.Instance.HasAssociation(label)) continue;


            string labDisplayName = labelList.GetLabel(label).OriginalDisplayName;

            if (string.IsNullOrEmpty(remainingLabels))
                remainingLabels = labDisplayName;
            else
                remainingLabels = string.Join(", ", remainingLabels, labDisplayName);
        }

        return remainingLabels;
    }
}
