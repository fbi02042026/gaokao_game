using System;
using System.Collections.Generic;
using UnityEngine;

namespace GaokaoLife.Models
{
    [Serializable]
    public class Talent
    {
        public string id;
        public string name;
        public string description;
        public TalentRarity rarity;
        public TalentType type;
        
        public List<TalentEffect> effects;
        public List<string> triggerPhases;
        public string unlockCondition;
        public string iconPath;
        
        public int maxLevel;
        public int currentLevel;
        public bool isUnlocked;

        public Talent()
        {
            effects = new List<TalentEffect>();
            triggerPhases = new List<string>();
            maxLevel = 1;
            currentLevel = 0;
            isUnlocked = false;
        }

        public void Unlock()
        {
            isUnlocked = true;
            currentLevel = 1;
            Debug.Log($"[Talent] 解锁天赋: {name} ({rarity})");
        }

        public void LevelUp()
        {
            if (currentLevel < maxLevel)
            {
                currentLevel++;
                Debug.Log($"[Talent] 天赋升级: {name} -> Level {currentLevel}");
            }
        }

        public float GetEffectValue(string effectType)
        {
            foreach (var effect in effects)
            {
                if (effect.type == effectType)
                {
                    return effect.value * currentLevel;
                }
            }
            return 0f;
        }

        public bool IsAvailableInPhase(GamePhase phase)
        {
            return triggerPhases.Contains(phase.ToString()) || triggerPhases.Contains("All");
        }
    }

    public enum TalentRarity
    {
        Common,
        Rare,
        Epic,
        Legendary
    }

    public enum TalentType
    {
        Passive,
        Active,
        Trigger,
        Permanent
    }

    [Serializable]
    public class TalentEffect
    {
        public string type;
        public string target;
        public float value;
        public string description;
        public bool isPercentage;

        public TalentEffect()
        {
            isPercentage = false;
        }

        public float GetModifiedValue()
        {
            return isPercentage ? value / 100f : value;
        }
    }

    [Serializable]
    public class TalentData
    {
        public List<Talent> talents;

        public TalentData()
        {
            talents = new List<Talent>();
        }

        public Talent GetTalent(string id)
        {
            return talents.Find(t => t.id == id);
        }

        public List<Talent> GetTalentsByRarity(TalentRarity rarity)
        {
            return talents.FindAll(t => t.rarity == rarity);
        }

        public List<Talent> GetTalentsByType(TalentType type)
        {
            return talents.FindAll(t => t.type == type);
        }
    }
}
