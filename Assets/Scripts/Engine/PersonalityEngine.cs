using System;
using System.Collections.Generic;
using UnityEngine;
using GaokaoLife.Models;

namespace GaokaoLife.Engine
{
    public class PersonalityEngine : MonoBehaviour
    {
        public static PersonalityEngine Instance { get; private set; }

        private PersonalityData personalityData;
        private Personality currentPersonality;

        public event Action<Personality> OnPersonalityChanged;

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

            Debug.Log("[PersonalityEngine] 人格引擎初始化完成");
        }

        public void LoadPersonalities(PersonalityData data)
        {
            personalityData = data;
            Debug.Log($"[PersonalityEngine] 加载了 {data.personalities.Count} 个人格配置");
        }

        public Personality CalculatePersonality(PlayerState player)
        {
            if (personalityData == null)
            {
                return CalculateDefaultPersonality(player);
            }

            currentPersonality = personalityData.CalculatePersonality(player);
            currentPersonality.ModifyPlayerState(player);
            
            OnPersonalityChanged?.Invoke(currentPersonality);
            
            Debug.Log($"[PersonalityEngine] 计算人格: {currentPersonality.name}");
            return currentPersonality;
        }

        private Personality CalculateDefaultPersonality(PlayerState player)
        {
            var personality = new Personality
            {
                id = player.personality.ToString(),
                name = player.personality.ToString(),
                description = "根据属性计算的人格"
            };

            switch (player.personality)
            {
                case PersonalityType.Academic:
                    personality.name = "学霸型";
                    personality.description = "擅长学习，智力突出";
                    break;
                case PersonalityType.Creative:
                    personality.name = "创意型";
                    personality.description = "富有创造力，想象力丰富";
                    break;
                case PersonalityType.Social:
                    personality.name = "社交型";
                    personality.description = "善于交际，人脉广泛";
                    break;
                case PersonalityType.Athletic:
                    personality.name = "运动型";
                    personality.description = "身体健康，体能优秀";
                    break;
                case PersonalityType.Balanced:
                    personality.name = "均衡型";
                    personality.description = "各项能力均衡发展";
                    break;
                case PersonalityType.Perfectionist:
                    personality.name = "完美型";
                    personality.description = "追求完美，注重细节";
                    break;
                case PersonalityType.Rebel:
                    personality.name = "叛逆型";
                    personality.description = "独立思考，不随波逐流";
                    break;
                case PersonalityType.LateBloomer:
                    personality.name = "大器晚成型";
                    personality.description = "厚积薄发，后发制人";
                    break;
            }

            return personality;
        }

        public Personality GetCurrentPersonality()
        {
            return currentPersonality;
        }

        public Personality GetPersonalityByType(PersonalityType type)
        {
            return personalityData?.GetPersonalityByType(type);
        }

        public List<string> GetSuitableMajors(PlayerState player)
        {
            if (currentPersonality == null)
            {
                CalculatePersonality(player);
            }

            return currentPersonality?.suitableMajors ?? new List<string>();
        }

        public List<string> GetSuitableCareers(PlayerState player)
        {
            if (currentPersonality == null)
            {
                CalculatePersonality(player);
            }

            return currentPersonality?.suitableCareers ?? new List<string>();
        }

        public float GetPersonalityModifier(string statType)
        {
            if (currentPersonality == null) return 1f;

            switch (currentPersonality.type)
            {
                case PersonalityType.Academic:
                    if (statType == "intelligence") return 1.2f;
                    if (statType == "creativity") return 1.1f;
                    break;
                case PersonalityType.Creative:
                    if (statType == "creativity") return 1.3f;
                    if (statType == "intelligence") return 1.1f;
                    break;
                case PersonalityType.Social:
                    if (statType == "social") return 1.3f;
                    if (statType == "emotion") return 1.1f;
                    break;
                case PersonalityType.Athletic:
                    if (statType == "physical") return 1.3f;
                    if (statType == "willpower") return 1.1f;
                    break;
                case PersonalityType.Balanced:
                    return 1.05f;
                case PersonalityType.Perfectionist:
                    if (statType == "willpower") return 1.2f;
                    if (statType == "intelligence") return 1.1f;
                    break;
                case PersonalityType.Rebel:
                    if (statType == "luck") return 1.2f;
                    if (statType == "creativity") return 1.2f;
                    break;
                case PersonalityType.LateBloomer:
                    if (statType == "willpower") return 1.3f;
                    if (statType == "luck") return 1.1f;
                    break;
            }

            return 1f;
        }

        public int CalculateExamBonus(PlayerState player)
        {
            if (currentPersonality == null)
            {
                CalculatePersonality(player);
            }

            return currentPersonality?.gaokaoBonus ?? 0;
        }

        public int CalculateCareerBonus(PlayerState player)
        {
            if (currentPersonality == null)
            {
                CalculatePersonality(player);
            }

            return currentPersonality?.careerBonus ?? 0;
        }

        public void Reset()
        {
            currentPersonality = null;
            Debug.Log("[PersonalityEngine] 人格引擎已重置");
        }
    }

    public class InheritEngine : MonoBehaviour
    {
        public static InheritEngine Instance { get; private set; }

        private Dictionary<string, float> inheritedTraits;
        private List<InheritItem> inheritedItems;

        public event Action<PlayerState> OnInheritanceApplied;

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

            inheritedTraits = new Dictionary<string, float>();
            inheritedItems = new List<InheritItem>();
            
            Debug.Log("[InheritEngine] 传承引擎初始化完成");
        }

        public void PrepareInheritance(PlayerState previousPlayer)
        {
            inheritedTraits.Clear();
            inheritedItems.Clear();

            if (previousPlayer == null) return;

            inheritedTraits["intelligence"] = previousPlayer.intelligence * 0.1f;
            inheritedTraits["physical"] = previousPlayer.physical * 0.1f;
            inheritedTraits["emotion"] = previousPlayer.emotion * 0.15f;
            inheritedTraits["social"] = previousPlayer.social * 0.1f;
            inheritedTraits["creativity"] = previousPlayer.creativity * 0.1f;
            inheritedTraits["luck"] = previousPlayer.luck * 0.2f;
            inheritedTraits["willpower"] = previousPlayer.willpower * 0.15f;

            if (previousPlayer.HasTalent("genetic_good_health"))
            {
                inheritedItems.Add(new InheritItem
                {
                    itemId = "genetic_good_health",
                    name = "健康基因",
                    description = "遗传自父母的健康体质"
                });
            }

            if (previousPlayer.HasTalent("family_tradition"))
            {
                inheritedItems.Add(new InheritItem
                {
                    itemId = "family_tradition",
                    name = "家族传承",
                    description = "家族几代人的智慧结晶"
                });
            }

            Debug.Log($"[InheritEngine] 准备传承: {inheritedTraits.Count} 项特质, {inheritedItems.Count} 项物品");
        }

        public void ApplyInheritance(PlayerState newPlayer)
        {
            foreach (var kvp in inheritedTraits)
            {
                int statValue = (int)kvp.Value;
                newPlayer.AddStat(kvp.Key, statValue);
            }

            foreach (var item in inheritedItems)
            {
                if (!newPlayer.ownedItemIds.Contains(item.itemId))
                {
                    newPlayer.ownedItemIds.Add(item.itemId);
                }
            }

            newPlayer.playthroughCount++;
            newPlayer.hasPastLifeMemory = true;

            OnInheritanceApplied?.Invoke(newPlayer);
            
            Debug.Log("[InheritEngine] 传承应用完成");
        }

        public Dictionary<string, float> GetInheritedTraits()
        {
            return new Dictionary<string, float>(inheritedTraits);
        }

        public List<InheritItem> GetInheritedItems()
        {
            return new List<InheritItem>(inheritedItems);
        }

        public void Reset()
        {
            inheritedTraits.Clear();
            inheritedItems.Clear();
            Debug.Log("[InheritEngine] 传承引擎已重置");
        }
    }

    [Serializable]
    public class InheritItem
    {
        public string itemId;
        public string name;
        public string description;
    }
}
