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
    public bool hasTransitionedToStress = false;
    public GamePhase currentPhase = GamePhase.Home;
    public System.DateTime lastPlayTime = System.DateTime.Now;
    public List<string> completedEvents = new List<string>();
    public List<StringPair> choiceHistoryList = new List<StringPair>();
    public List<string> ownedItemIds = new List<string>();

    public int lifeSatisfaction = 50;
    public int careerLevel = 1;
    public float totalScore = 0;
    public int studyDays = 0;
    public PersonalityType personality;
    public bool hasPastLifeMemory = false;
    public Dictionary<string, int> subjectScores = new Dictionary<string, int>();
    public List<string> unlockedTalents = new List<string>();
    public List<string> selectedSubjects = new List<string>();

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
                case "satisfaction": satisfaction = Mathf.Clamp(satisfaction + kv.Value, 1, 5); break;
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
            case "intellect": intellect = Mathf.Clamp(intellect + value, 0, 100); break;
            case "mental": mental = Mathf.Clamp(mental + value, 0, 100); break;
            case "social": social = Mathf.Clamp(social + value, 0, 100); break;
            case "health": health = Mathf.Clamp(health + value, 0, 100); break;
        }
    }

    public int GetAverageStats()
    {
        return (int)((intellect + mental + social + health) / 4f);
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
        clone.lifeSatisfaction = lifeSatisfaction;
        clone.careerLevel = careerLevel;
        clone.totalScore = totalScore;
        clone.gaokaoScore = gaokaoScore;
        clone.personality = personality;
        clone.province = province;
        clone.graduationChoice = graduationChoice;
        clone.careerPath = careerPath;
        clone.monthlyIncome = monthlyIncome;
        clone.familyStatus = familyStatus;
        clone.satisfaction = satisfaction;
        clone.currentAge = currentAge;
        clone.hasPastLifeMemory = hasPastLifeMemory;
        clone.admittedCollegeId = admittedCollegeId;
        clone.admittedMajorId = admittedMajorId;
        return clone;
    }
}