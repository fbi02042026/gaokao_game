using System;
using System.Collections.Generic;
using UnityEngine;

namespace GaokaoLife.Models
{
    [Serializable]
    public class PlayerState
    {
        public string playerId;
        public string playerName;
        public Gender gender;
        public Province province;
        public GamePhase currentPhase;
        
        public int intelligence;
        public int physical;
        public int emotion;
        public int social;
        public int creativity;
        public int luck;
        public int willpower;
        
        public int studyDays;
        public int stressLevel;
        public int happiness;
        public float totalScore;
        
        public PersonalityType personality;
        public List<string> unlockedTalentIds;
        public List<string> ownedItemIds;
        public List<string> triggeredEventIds;
        public List<string> memories;
        
        public Dictionary<string, int> npcImpression;
        public Dictionary<string, int> subjectScores;
        
        public int gaokaoScore;
        public string admittedCollegeId;
        public string admittedMajorId;
        public int careerLevel;
        public int lifeSatisfaction;
        
        public int playthroughCount;
        public bool hasPastLifeMemory;
        
        public DateTime createTime;
        public DateTime lastPlayTime;
        public int totalPlayMinutes;

        public PlayerState()
        {
            playerId = Guid.NewGuid().ToString();
            playerName = "考生";
            gender = Gender.Male;
            province = Province.Beijing;
            currentPhase = GamePhase.Home;
            
            intelligence = 50;
            physical = 50;
            emotion = 50;
            social = 50;
            creativity = 50;
            luck = 50;
            willpower = 50;
            
            studyDays = 0;
            stressLevel = 0;
            happiness = 50;
            totalScore = 0f;
            
            personality = PersonalityType.Balanced;
            unlockedTalentIds = new List<string>();
            ownedItemIds = new List<string>();
            triggeredEventIds = new List<string>();
            memories = new List<string>();
            
            npcImpression = new Dictionary<string, int>();
            subjectScores = new Dictionary<string, int>()
            {
                { "chinese", 0 },
                { "math", 0 },
                { "english", 0 },
                { "physics", 0 },
                { "chemistry", 0 },
                { "biology", 0 },
                { "politics", 0 },
                { "history", 0 },
                { "geography", 0 }
            };
            
            gaokaoScore = 0;
            admittedCollegeId = "";
            admittedMajorId = "";
            careerLevel = 1;
            lifeSatisfaction = 50;
            
            playthroughCount = 0;
            hasPastLifeMemory = false;
            
            createTime = DateTime.Now;
            lastPlayTime = DateTime.Now;
            totalPlayMinutes = 0;
        }

        public PlayerState Clone()
        {
            return new PlayerState
            {
                playerId = this.playerId,
                playerName = this.playerName,
                gender = this.gender,
                province = this.province,
                currentPhase = this.currentPhase,
                intelligence = this.intelligence,
                physical = this.physical,
                emotion = this.emotion,
                social = this.social,
                creativity = this.creativity,
                luck = this.luck,
                willpower = this.willpower,
                studyDays = this.studyDays,
                stressLevel = this.stressLevel,
                happiness = this.happiness,
                totalScore = this.totalScore,
                personality = this.personality,
                unlockedTalentIds = new List<string>(this.unlockedTalentIds),
                ownedItemIds = new List<string>(this.ownedItemIds),
                triggeredEventIds = new List<string>(this.triggeredEventIds),
                memories = new List<string>(this.memories),
                npcImpression = new Dictionary<string, int>(this.npcImpression),
                subjectScores = new Dictionary<string, int>(this.subjectScores),
                gaokaoScore = this.gaokaoScore,
                admittedCollegeId = this.admittedCollegeId,
                admittedMajorId = this.admittedMajorId,
                careerLevel = this.careerLevel,
                lifeSatisfaction = this.lifeSatisfaction,
                playthroughCount = this.playthroughCount,
                hasPastLifeMemory = this.hasPastLifeMemory,
                createTime = this.createTime,
                lastPlayTime = DateTime.Now,
                totalPlayMinutes = this.totalPlayMinutes
            };
        }

        public void AddStat(string statName, int value)
        {
            switch (statName.ToLower())
            {
                case "intelligence":
                case "int":
                    intelligence = Mathf.Clamp(intelligence + value, 0, 100);
                    break;
                case "physical":
                case "phy":
                    physical = Mathf.Clamp(physical + value, 0, 100);
                    break;
                case "emotion":
                case "emo":
                    emotion = Mathf.Clamp(emotion + value, 0, 100);
                    break;
                case "social":
                case "soc":
                    social = Mathf.Clamp(social + value, 0, 100);
                    break;
                case "creativity":
                case "cre":
                    creativity = Mathf.Clamp(creativity + value, 0, 100);
                    break;
                case "luck":
                case "luk":
                    luck = Mathf.Clamp(luck + value, 0, 100);
                    break;
                case "willpower":
                case "wil":
                    willpower = Mathf.Clamp(willpower + value, 0, 100);
                    break;
            }
        }

        public int GetStat(string statName)
        {
            switch (statName.ToLower())
            {
                case "intelligence":
                case "int":
                    return intelligence;
                case "physical":
                case "phy":
                    return physical;
                case "emotion":
                case "emo":
                    return emotion;
                case "social":
                case "soc":
                    return social;
                case "creativity":
                case "cre":
                    return creativity;
                case "luck":
                case "luk":
                    return luck;
                case "willpower":
                case "wil":
                    return willpower;
                default:
                    return 0;
            }
        }

        public int GetAverageStats()
        {
            return (intelligence + physical + emotion + social + creativity + luck + willpower) / 7;
        }

        public bool HasTalent(string talentId)
        {
            return unlockedTalentIds.Contains(talentId);
        }

        public void UnlockTalent(string talentId)
        {
            if (!unlockedTalentIds.Contains(talentId))
            {
                unlockedTalentIds.Add(talentId);
                Debug.Log($"[PlayerState] 解锁天赋: {talentId}");
            }
        }

        public void AddMemory(string memory)
        {
            if (!memories.Contains(memory))
            {
                memories.Add(memory);
            }
        }

        public bool HasMemory(string memory)
        {
            return memories.Contains(memory);
        }
    }
}
