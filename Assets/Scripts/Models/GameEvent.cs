using System;
using System.Collections.Generic;
using UnityEngine;

namespace GaokaoLife.Models
{
    [Serializable]
    public class GameEvent
    {
        public string id;
        public string title;
        public string description;
        public string storyText;
        
        public GamePhase phase;
        public int triggerDayMin;
        public int triggerDayMax;
        public float triggerChance;
        
        public List<EventCondition> conditions;
        public List<EventChoice> choices;
        
        public EventCategory category;
        public EventEffect immediateEffect;
        
        public bool isRandom;
        public bool isUnique;
        public bool isHidden;
        public int priority;

        public GameEvent()
        {
            conditions = new List<EventCondition>();
            choices = new List<EventChoice>();
            triggerChance = 1f;
            isRandom = false;
            isUnique = false;
            isHidden = false;
            priority = 0;
        }

        public bool CanTrigger(PlayerState player)
        {
            if (isUnique && player.triggeredEventIds.Contains(id))
            {
                return false;
            }

            foreach (var condition in conditions)
            {
                if (!condition.IsMet(player))
                {
                    return false;
                }
            }

            if (player.studyDays < triggerDayMin || player.studyDays > triggerDayMax)
            {
                return false;
            }

            float randomValue = UnityEngine.Random.value;
            return randomValue <= triggerChance;
        }

        public void ExecuteChoice(PlayerState player, int choiceIndex)
        {
            if (choiceIndex < 0 || choiceIndex >= choices.Count)
            {
                Debug.LogWarning($"[GameEvent] Invalid choice index: {choiceIndex}");
                return;
            }

            var choice = choices[choiceIndex];
            
            player.AddStat("intelligence", choice.statEffects.intelligence);
            player.AddStat("physical", choice.statEffects.physical);
            player.AddStat("emotion", choice.statEffects.emotion);
            player.AddStat("social", choice.statEffects.social);
            player.AddStat("creativity", choice.statEffects.creativity);
            player.AddStat("luck", choice.statEffects.luck);
            player.AddStat("willpower", choice.statEffects.willpower);
            
            player.studyDays += choice.studyDaysEffect;
            player.stressLevel += choice.stressEffect;
            player.happiness += choice.happinessEffect;
            
            foreach (var talentUnlock in choice.talentUnlocks)
            {
                player.UnlockTalent(talentUnlock);
            }
            
            foreach (var memory in choice.memories)
            {
                player.AddMemory(memory);
            }
            
            if (!player.triggeredEventIds.Contains(id))
            {
                player.triggeredEventIds.Add(id);
            }
            
            Debug.Log($"[GameEvent] 执行选择: {choice.text}");
        }
    }

    public enum EventCategory
    {
        Study,
        Social,
        Family,
        Romance,
        Competition,
        Crisis,
        Fortune,
        Milestone,
        Random,
        Hidden
    }

    [Serializable]
    public class EventCondition
    {
        public ConditionType type;
        public string target;
        public ComparisonOperator operatorType;
        public int value;
        public string targetValue;

        public bool IsMet(PlayerState player)
        {
            int playerValue = 0;

            switch (type)
            {
                case ConditionType.Stat:
                    playerValue = player.GetStat(target);
                    break;
                case ConditionType.Talent:
                    return player.HasTalent(target) == (value == 1);
                case ConditionType.Item:
                    return player.ownedItemIds.Contains(target) == (value == 1);
                case ConditionType.Phase:
                    return player.currentPhase.ToString() == target;
                case ConditionType.StudyDays:
                    playerValue = player.studyDays;
                    break;
                case ConditionType.Stress:
                    playerValue = player.stressLevel;
                    break;
                case ConditionType.Happiness:
                    playerValue = player.happiness;
                    break;
                case ConditionType.Province:
                    return player.province.ToString() == target;
                case ConditionType.Gender:
                    return player.gender.ToString() == target;
            }

            return CompareValues(playerValue, value);
        }

        private bool CompareValues(int a, int b)
        {
            switch (operatorType)
            {
                case ComparisonOperator.Equal:
                    return a == b;
                case ComparisonOperator.NotEqual:
                    return a != b;
                case ComparisonOperator.GreaterThan:
                    return a > b;
                case ComparisonOperator.LessThan:
                    return a < b;
                case ComparisonOperator.GreaterOrEqual:
                    return a >= b;
                case ComparisonOperator.LessOrEqual:
                    return a <= b;
                default:
                    return false;
            }
        }
    }

    public enum ConditionType
    {
        Stat,
        Talent,
        Item,
        Phase,
        StudyDays,
        Stress,
        Happiness,
        Province,
        Gender
    }

    public enum ComparisonOperator
    {
        Equal,
        NotEqual,
        GreaterThan,
        LessThan,
        GreaterOrEqual,
        LessOrEqual
    }

    [Serializable]
    public class EventChoice
    {
        public string id;
        public string text;
        public string resultText;
        
        public StatEffects statEffects;
        public int studyDaysEffect;
        public int stressEffect;
        public int happinessEffect;
        
        public List<string> talentUnlocks;
        public List<string> memories;
        public List<string> itemRewards;
        public List<string> itemCosts;
        
        public int scoreBonus;
        public bool isOptimal;
        public float probability;

        public EventChoice()
        {
            statEffects = new StatEffects();
            talentUnlocks = new List<string>();
            memories = new List<string>();
            itemRewards = new List<string>();
            itemCosts = new List<string>();
            probability = 1f;
            isOptimal = false;
        }
    }

    [Serializable]
    public class StatEffects
    {
        public int intelligence;
        public int physical;
        public int emotion;
        public int social;
        public int creativity;
        public int luck;
        public int willpower;

        public StatEffects()
        {
            intelligence = 0;
            physical = 0;
            emotion = 0;
            social = 0;
            creativity = 0;
            luck = 0;
            willpower = 0;
        }

        public int GetTotal()
        {
            return Mathf.Abs(intelligence) + Mathf.Abs(physical) + Mathf.Abs(emotion) +
                   Mathf.Abs(social) + Mathf.Abs(creativity) + Mathf.Abs(luck) + Mathf.Abs(willpower);
        }
    }

    [Serializable]
    public class EventEffect
    {
        public StatEffects stats;
        public int stressChange;
        public int happinessChange;
        public List<string> unlockTalents;
        public List<string> addMemories;
        public int scoreModifier;

        public EventEffect()
        {
            stats = new StatEffects();
            unlockTalents = new List<string>();
            addMemories = new List<string>();
            scoreModifier = 0;
        }
    }
}
