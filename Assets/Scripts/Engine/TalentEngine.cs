using System;
using System.Collections.Generic;
using UnityEngine;
using GaokaoLife.Models;

namespace GaokaoLife.Engine
{
    public class TalentEngine : MonoBehaviour
    {
        public static TalentEngine Instance { get; private set; }

        private TalentData talentData;
        private Dictionary<string, Talent> activeTalents;

        public event Action<Talent> OnTalentUnlocked;
        public event Action<Talent, int> OnTalentLevelUp;
        public event Action<float, string> OnTalentEffectApplied;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            activeTalents = new Dictionary<string, Talent>();
            Debug.Log("[TalentEngine] 天赋引擎初始化完成");
        }

        public void LoadTalents(TalentData data)
        {
            talentData = data;
            Debug.Log($"[TalentEngine] 加载了 {data.talents.Count} 个天赋配置");
        }

        public void InitializePlayerTalents(PlayerState player)
        {
            foreach (var talentId in player.unlockedTalentIds)
            {
                var talent = talentData?.GetTalent(talentId);
                if (talent != null)
                {
                    activeTalents[talentId] = talent;
                }
            }
            
            Debug.Log($"[TalentEngine] 玩家拥有 {activeTalents.Count} 个天赋");
        }

        public bool UnlockTalent(string talentId, PlayerState player)
        {
            if (activeTalents.ContainsKey(talentId))
            {
                Debug.LogWarning($"[TalentEngine] 天赋已解锁: {talentId}");
                return false;
            }

            var talent = talentData?.GetTalent(talentId);
            if (talent == null)
            {
                Debug.LogWarning($"[TalentEngine] 找不到天赋配置: {talentId}");
                return false;
            }

            if (talent.maxLevel == 1)
            {
                talent.Unlock();
            }

            activeTalents[talentId] = talent;
            player.UnlockTalent(talentId);
            
            OnTalentUnlocked?.Invoke(talent);
            
            Debug.Log($"[TalentEngine] 解锁天赋: {talent.name} ({talent.rarity})");
            return true;
        }

        public bool LevelUpTalent(string talentId, PlayerState player)
        {
            if (!activeTalents.ContainsKey(talentId))
            {
                Debug.LogWarning($"[TalentEngine] 天赋未解锁: {talentId}");
                return false;
            }

            var talent = activeTalents[talentId];
            if (talent.currentLevel >= talent.maxLevel)
            {
                Debug.LogWarning($"[TalentEngine] 天赋已达最大等级: {talentId}");
                return false;
            }

            talent.LevelUp();
            OnTalentLevelUp?.Invoke(talent, talent.currentLevel);
            
            return true;
        }

        public float CalculateStatBonus(string statName, PlayerState player)
        {
            float totalBonus = 0f;

            foreach (var kvp in activeTalents)
            {
                var talent = kvp.Value;
                if (!talent.IsAvailableInPhase(player.currentPhase))
                {
                    continue;
                }

                foreach (var effect in talent.effects)
                {
                    if (effect.type == "stat_boost" && effect.target == statName)
                    {
                        float bonus = effect.GetModifiedValue() * talent.currentLevel;
                        totalBonus += bonus;
                        
                        OnTalentEffectApplied?.Invoke(bonus, talent.name);
                    }
                }
            }

            return totalBonus;
        }

        public int CalculateStudyEfficiency(PlayerState player)
        {
            int baseEfficiency = 100;
            
            float talentBonus = CalculateStatBonus("study_efficiency", player);
            float intelligenceBonus = player.intelligence * 0.5f;
            
            return (int)(baseEfficiency + talentBonus + intelligenceBonus);
        }

        public float CalculateExamStability(PlayerState player)
        {
            float baseStability = 0.5f;
            
            float talentBonus = CalculateStatBonus("exam_stability", player);
            float emotionFactor = player.emotion * 0.005f;
            float willpowerFactor = player.willpower * 0.003f;
            
            return Mathf.Clamp(baseStability + talentBonus + emotionFactor + willpowerFactor, 0f, 1f);
        }

        public float CalculateLuckBonus(PlayerState player)
        {
            float baseLuck = player.luck / 100f;
            float talentBonus = CalculateStatBonus("luck", player) / 100f;
            
            return Mathf.Clamp(baseLuck + talentBonus, 0f, 1f);
        }

        public int CalculateDailyStudyTime(PlayerState player)
        {
            int baseTime = 8;
            float talentBonus = CalculateStatBonus("daily_study_time", player);
            
            return (int)(baseTime + talentBonus / 60f);
        }

        public bool HasSpecialAbility(string abilityName, PlayerState player)
        {
            foreach (var kvp in activeTalents)
            {
                var talent = kvp.Value;
                if (!talent.IsAvailableInPhase(player.currentPhase))
                {
                    continue;
                }

                foreach (var effect in talent.effects)
                {
                    if (effect.type == "special_ability" && effect.target == abilityName)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public List<Talent> GetActiveTalents()
        {
            return new List<Talent>(activeTalents.Values);
        }

        public Talent GetTalent(string talentId)
        {
            return talentData?.GetTalent(talentId);
        }

        public List<Talent> GetAvailableTalentsForUnlock(PlayerState player)
        {
            var available = new List<Talent>();
            
            if (talentData == null) return available;

            foreach (var talent in talentData.talents)
            {
                if (activeTalents.ContainsKey(talent.id))
                {
                    continue;
                }

                if (!talent.IsAvailableInPhase(player.currentPhase))
                {
                    continue;
                }

                if (CheckUnlockCondition(talent.unlockCondition, player))
                {
                    available.Add(talent);
                }
            }

            return available;
        }

        private bool CheckUnlockCondition(string condition, PlayerState player)
        {
            if (string.IsNullOrEmpty(condition)) return true;

            switch (condition)
            {
                case "auto_unlock_early":
                    return player.studyDays < 100;
                case "random_chance":
                    return UnityEngine.Random.value < 0.1f;
                case "complete_one_playthrough":
                    return player.playthroughCount > 0;
                case "story_event":
                    return player.triggeredEventIds.Count >= 5;
                case "high_stats":
                    return player.GetAverageStats() >= 70;
                case "low_stats":
                    return player.GetAverageStats() <= 40;
                default:
                    return true;
            }
        }

        public Dictionary<string, float> GetAllActiveEffects(PlayerState player)
        {
            var effects = new Dictionary<string, float>();
            
            foreach (var kvp in activeTalents)
            {
                var talent = kvp.Value;
                if (!talent.IsAvailableInPhase(player.currentPhase))
                {
                    continue;
                }

                foreach (var effect in talent.effects)
                {
                    if (!effects.ContainsKey(effect.target))
                    {
                        effects[effect.target] = 0f;
                    }
                    effects[effect.target] += effect.GetModifiedValue() * talent.currentLevel;
                }
            }

            return effects;
        }

        public void Reset()
        {
            activeTalents.Clear();
            Debug.Log("[TalentEngine] 天赋引擎已重置");
        }
    }
}
