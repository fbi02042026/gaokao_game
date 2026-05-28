using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PlayerState
{
    public Gender gender = Gender.Male;
    public int intellect = 50;
    public int mental = 50;
    public int social = 50;
    public int health = 50;
    public int grade = 1;
    public int gaokaoScore = 0;
    public string province = "浙江";
    public string admittedCollegeId = "";
    public string admittedMajorId = "";
    public string graduationChoice = "";
    public string careerPath = "";
    public int monthlyIncome = 0;
    public string familyStatus = "";
    public int satisfaction = 3;
    public int currentAge = 22;
    public GamePhase currentPhase = GamePhase.Home;
    public System.DateTime lastPlayTime = System.DateTime.Now;
    public List<string> completedEvents = new List<string>();
    public List<StringPair> choiceHistoryList = new List<StringPair>();
    public List<string> ownedItemIds = new List<string>();
    
    public int intelligence = 50;
    public int physical = 50;
    public int emotion = 50;
    public int creativity = 50;
    public int luck = 50;
    public int willpower = 50;
    public int happiness = 50;
    public int lifeSatisfaction = 50;
    public int careerLevel = 1;
    public float totalScore = 0;
    public int studyDays = 0;
    public PersonalityType personality;
    public bool hasPastLifeMemory = false;
    public Dictionary<string, int> subjectScores = new Dictionary<string, int>();
    public List<string> unlockedTalents = new List<string>();

    [System.Serializable]
    public class StringPair
    {
        public string key;
        public string value;
    }

    public void ApplyChanges(Dictionary<string, int> changes)
    {
        foreach (var kv in changes)
        {
            switch (kv.Key)
            {
                case "intellect": intellect = Mathf.Clamp(intellect + kv.Value, 0, 100); break;
                case "mental": mental = Mathf.Clamp(mental + kv.Value, 0, 100); break;
                case "social": social = Mathf.Clamp(social + kv.Value, 0, 100); break;
                case "health": health = Mathf.Clamp(health + kv.Value, 0, 100); break;
            }
        }
    }

    public void RecordChoice(string eventId, string choiceId)
    {
        choiceHistoryList.Add(new StringPair { key = eventId, value = choiceId });
    }

    public string GetChoice(string eventId)
    {
        var pair = choiceHistoryList.Find(p => p.key == eventId);
        return pair?.value;
    }

    public void AddStat(string statName, int value)
    {
        switch (statName)
        {
            case "intelligence": intelligence = Mathf.Clamp(intelligence + value, 0, 100); break;
            case "physical": physical = Mathf.Clamp(physical + value, 0, 100); break;
            case "emotion": emotion = Mathf.Clamp(emotion + value, 0, 100); break;
            case "social": social = Mathf.Clamp(social + value, 0, 100); break;
            case "creativity": creativity = Mathf.Clamp(creativity + value, 0, 100); break;
            case "luck": luck = Mathf.Clamp(luck + value, 0, 100); break;
            case "willpower": willpower = Mathf.Clamp(willpower + value, 0, 100); break;
            case "happiness": happiness = Mathf.Clamp(happiness + value, 0, 100); break;
            case "intellect": intellect = Mathf.Clamp(intellect + value, 0, 100); break;
            case "mental": mental = Mathf.Clamp(mental + value, 0, 100); break;
            case "health": health = Mathf.Clamp(health + value, 0, 100); break;
        }
    }

    public int GetAverageStats()
    {
        return (int)((intelligence + physical + emotion + social + creativity + luck + willpower) / 7f);
    }

    public void UnlockTalent(string talentId)
    {
        if (!unlockedTalents.Contains(talentId))
        {
            unlockedTalents.Add(talentId);
        }
    }

    public PlayerState Clone()
    {
        var clone = new PlayerState();
        clone.gender = gender;
        clone.intellect = intellect;
        clone.mental = mental;
        clone.social = social;
        clone.health = health;
        clone.intelligence = intelligence;
        clone.physical = physical;
        clone.emotion = emotion;
        clone.creativity = creativity;
        clone.luck = luck;
        clone.willpower = willpower;
        clone.happiness = happiness;
        clone.lifeSatisfaction = lifeSatisfaction;
        clone.careerLevel = careerLevel;
        clone.totalScore = totalScore;
        clone.gaokaoScore = gaokaoScore;
        clone.personality = personality;
        return clone;
    }
}