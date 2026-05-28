using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class GameEvent
{
    public string id;
    public string name;
    public string stage;
    public string[] tags;
    public string illustrationId;
    public TriggerCondition triggerCondition;
    public string interactionType;
    public ChoiceContent choiceContent;
    public MergeContent mergeContent;
    public SortContent sortContent;
    public SliderContent sliderContent;
    public TimingContent timingContent;
    public DialogContent dialogContent;
    public SerializableOutcome[] outcomeList;

    public Dictionary<string, EventOutcome> GetOutcomeDict()
    {
        if (outcomeList == null) return new Dictionary<string, EventOutcome>();
        var dict = new Dictionary<string, EventOutcome>();
        foreach (var o in outcomeList) dict[o.key] = o.value;
        return dict;
    }
}

[System.Serializable]
public class SerializableOutcome
{
    public string key;
    public EventOutcome value;
}

[System.Serializable]
public class TriggerCondition
{
    public int minGrade;
    public int minIntellect;
    public string requiredEvent;
}

[System.Serializable]
public class EventOutcome
{
    public string[] statKeys;
    public int[] statValues;
    public string[] unlockEvents;
    public string narrative;

    public Dictionary<string, int> GetStatChanges()
    {
        var dict = new Dictionary<string, int>();
        if (statKeys != null && statValues != null)
            for (int i = 0; i < statKeys.Length; i++) dict[statKeys[i]] = statValues[i];
        return dict;
    }
}

[System.Serializable]
public class ChoiceContent
{
    public string description;
    public ChoiceOption[] choices;
}

[System.Serializable]
public class ChoiceOption
{
    public string id;
    public string text;
    public string icon;
}

[System.Serializable]
public class MergeContent
{
    public string description;
    public int gridRows;
    public int gridCols;
    public string[] availableItems;
    public string[] mergeRuleKeys;
    public string[] mergeRuleValues;
    public string targetItem;
    public int movesLimit;
}

[System.Serializable]
public class SortContent
{
    public string description;
    public string[] items;
    public bool showProbability;
}

[System.Serializable]
public class SliderContent
{
    public string description;
    public SliderCategory[] categories;
    public int totalPoints;
}

[System.Serializable]
public class SliderCategory
{
    public string name;
    public string icon;
}

[System.Serializable]
public class TimingContent
{
    public string description;
    public float duration;
    public int targetCount;
}

[System.Serializable]
public class DialogContent
{
    public string description;
    public string npcId;
    public string[] lines;
}