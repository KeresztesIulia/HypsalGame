using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "so_NameList", menuName = "Scriptable Objects/Namelist")]
public class script_so_NameList : ScriptableObject
{
    [SerializeField] LabelableWord[] words;
    Dictionary<string, LabelableWord> wordsDictionary; // maybe save by part-of-speech too

    public string[] wordStrings
    {
        get
        {
            if (wordsDictionary != null) return wordsDictionary.Keys.ToArray<string>(); 
            return words.Select(word => word.originalName).ToArray();
             

        }
    }

    public int wordCount => words.Length;

    public void Init()
    {
        if (words == null || words.Length == 0) return;

        wordsDictionary = new Dictionary<string, LabelableWord>();
        foreach (var word in words)
        {
            word.Init();
            wordsDictionary.Add(word.originalName, word);
        }
    }

    public LabelableWord GetWord(string mainName)
    {
        if (wordsDictionary == null) Init();
        return wordsDictionary.ContainsKey(mainName) ? wordsDictionary[mainName] : null;
    }

    public LabelableWord GetWord(int index)
    {
        if (words == null || index < 0 || index >= words.Length) return null;
        return words[index];
    }

    public int GetWordIndex(LabelableWord word)
    {
        var idx = Array.IndexOf(words, word);
        return idx;
    }
}

[System.Serializable]
public class LabelableWord : IEquatable<LabelableWord>
{
    public enum PartOfSpeech
    {
        Noun, Verb, Adjective // could be other things but are we really labeling anything else?
    }

    [SerializeField] string _mainName;
    [SerializeField] List<string> _synonyms;
    [SerializeField] PartOfSpeech _type;
    [SerializeField] bool _renameable = false; // is this per-word or rather for a word-list? Is it both?
    string givenLabel;

    public string originalName => _mainName;
    public PartOfSpeech type => _type;
    public bool named => !string.IsNullOrEmpty(givenLabel);
    public string displayName => named ? givenLabel : "(unnamed)";
    public string forcedDisplayName => named ? givenLabel : originalName; // for when we must display something, ex. in dialogue before naming stuff.

    bool hasSynonyms => _synonyms != null && _synonyms.Count > 0;

    public void Init()
    {
        _mainName = _mainName.ToLowerInvariant();

        if (!hasSynonyms) return;

        for (int i = 0; i < _synonyms.Count; i++)
        {
            string word = _synonyms[i];
            _synonyms[i] = word.ToLowerInvariant();
        }
    }

    public void LabelWord(string label)
    {
        if (_renameable || !named)
        {
            // for now, culture-invariant, but if we allow languages later on, then maybe ask AI here and use ToLower if another language is recognized
            givenLabel = label.ToLowerInvariant();
        }
    }

    public bool LabelMatchesOriginal(string label = null)
    {
        if (string.IsNullOrEmpty(label)) label = givenLabel;
        if (string.IsNullOrEmpty(label)) return false;

        if (label == originalName) return true;
        if (!hasSynonyms) return false;
        foreach (string word in _synonyms)
            if (label == word) return true;

        return false;
    }

    public static bool operator ==(LabelableWord word1,  LabelableWord word2)
    {
        return word1.originalName == word2.originalName;
    }

    public static bool operator !=(LabelableWord word1, LabelableWord word2)
    {
        return !(word1==word2);
    }

    public bool Equals(LabelableWord other)
    {
        return this == other;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_mainName, _type);
    }
}
