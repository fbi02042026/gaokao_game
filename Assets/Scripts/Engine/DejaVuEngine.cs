using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class DejaVuLevel
{
    public string id;
    public string name;
    public string icon;
    public int hintLevel;
    public string description;
}

[System.Serializable]
public class PlaythroughMemory
{
    public int generation;
    public string talentId;
    public List<ChoiceRecord> choiceRecords;
    public string finalMajor;
    public string finalCollege;
    public List<IntPair> finalStatsList;
    public string endingType;
}

[System.Serializable]
public class ChoiceRecord
{
    public string eventId;
    public string choiceId;
    public string outcomeNarrative;
    public List<IntPair> statChangesList;
}

[System.Serializable]
public class IntPair
{
    public string key;
    public int value;
}

public class DejaVuResult
{
    public string eventId;
    public string previousChoice;
    public string hint;
    public string highlightChoiceId;
    public bool talentChanged;
    public string previousTalentName;
}

public class DejaVuEngine : MonoBehaviour
{
    public static DejaVuEngine Instance { get; private set; }

    private List<PlaythroughMemory> memories = new List<PlaythroughMemory>();

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    public void LoadMemories(List<PlaythroughMemory> mems)
    {
        memories = mems ?? new List<PlaythroughMemory>();
        Debug.Log($"[DejaVuEngine] 加载 {memories.Count} 条前世记忆");
    }

    public DejaVuResult CheckDejaVu(string eventId, int maxLevel)
    {
        PlaythroughMemory bestMatch = null;
        for (int i = memories.Count - 1; i >= 0; i--)
        {
            var rec = memories[i].choiceRecords?.Find(r => r.eventId == eventId);
            if (rec != null) { bestMatch = memories[i]; break; }
        }

        if (bestMatch == null)
        {
            Debug.Log($"[DejaVuEngine] 事件 {eventId} 无前世记忆");
            return null;
        }

        var choice = bestMatch.choiceRecords.Find(r => r.eventId == eventId);
        int level = Mathf.Min(maxLevel, 4);

        var result = new DejaVuResult
        {
            eventId = eventId,
            previousChoice = choice.choiceId,
            hint = GenerateHint(choice, level),
            highlightChoiceId = level >= 4 ? choice.choiceId : null,
            talentChanged = false,
            previousTalentName = ""
        };

        Debug.Log($"[DejaVuEngine] 触发既视感: {eventId} -> {choice.choiceId} (等级{level})");
        return result;
    }

    private string GenerateHint(ChoiceRecord choice, int level)
    {
        return level switch
        {
            1 => $"上次好像选了{choice.choiceId}……",
            2 => $"上次选了{choice.choiceId}，好像结果{Truncate(choice.outcomeNarrative, 10)}……",
            3 => $"上次选了{choice.choiceId}，{choice.outcomeNarrative}",
            4 => "",
            _ => ""
        };
    }

    private string Truncate(string s, int maxLen)
    {
        if (string.IsNullOrEmpty(s)) return "";
        return s.Length <= maxLen ? s : s.Substring(0, maxLen) + "...";
    }

    public void RecordMemory(PlaythroughMemory memory)
    {
        memories.Add(memory);
        Debug.Log($"[DejaVuEngine] 记录第{memory.generation}代记忆, 事件数={memory.choiceRecords?.Count ?? 0}");
    }

    public int GetMemoryCount()
    {
        return memories.Count;
    }
}