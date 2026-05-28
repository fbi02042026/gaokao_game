using System;
using System.Collections.Generic;
using UnityEngine;

public class LifeEngine : MonoBehaviour
    {
        public static LifeEngine Instance { get; private set; }

        private int currentLifeDay;
        private int maxLifeDays;
        private Dictionary<string, LifeEvent> lifeEvents;
        private List<string> achievedMilestones;

        public event Action<int> OnLifeDayChanged;
        public event Action<string> OnMilestoneAchieved;
        public event Action<LifeResult> OnLifeEnded;

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

            lifeEvents = new Dictionary<string, LifeEvent>();
            achievedMilestones = new List<string>();
            currentLifeDay = 0;
            maxLifeDays = 365 * 50;
            
            InitializeDefaultLifeEvents();
            
            Debug.Log("[LifeEngine] 人生引擎初始化完成");
        }

        private void InitializeDefaultLifeEvents()
        {
            AddLifeEvent(new LifeEvent
            {
                id = "first_job",
                day = 0,
                title = "第一份工作",
                description = "开启职业生涯",
                effects = new StatEffects { intelligence = 5, social = 5 }
            });

            AddLifeEvent(new LifeEvent
            {
                id = "first_raise",
                day = 365,
                title = "第一次加薪",
                description = "努力工作得到回报",
                effects = new StatEffects { emotion = 10, luck = 5 }
            });

            AddLifeEvent(new LifeEvent
            {
                id = "promotion",
                day = 730,
                title = "晋升",
                description = "成为部门主管",
                effects = new StatEffects { intelligence = 10, social = 10, willpower = 5 }
            });

            AddLifeEvent(new LifeEvent
            {
                id = "marriage",
                day = 1460,
                title = "结婚",
                description = "人生大事",
                effects = new StatEffects { emotion = 20, social = 10 }
            });

            AddLifeEvent(new LifeEvent
            {
                id = "child_born",
                day = 1825,
                title = "孩子出生",
                description = "家庭新成员",
                effects = new StatEffects { emotion = 15, willpower = 10 }
            });

            AddLifeEvent(new LifeEvent
            {
                id = "career_peak",
                day = 3650,
                title = "事业巅峰",
                description = "成为行业精英",
                effects = new StatEffects { intelligence = 15, social = 15, luck = 10 }
            });

            AddLifeEvent(new LifeEvent
            {
                id = "retirement",
                day = 10950,
                title = "退休",
                description = "安享晚年",
                effects = new StatEffects { emotion = 20, willpower = 10 }
            });
        }

        public void AddLifeEvent(LifeEvent evt)
        {
            if (!lifeEvents.ContainsKey(evt.id))
            {
                lifeEvents[evt.id] = evt;
            }
        }

        public void StartLifeSimulation(PlayerState player)
        {
            currentLifeDay = 0;
            achievedMilestones.Clear();
            
            Debug.Log("[LifeEngine] 开始人生模拟");
        }

        public void AdvanceLifeDays(int days, PlayerState player)
        {
            currentLifeDay += days;
            
            CheckAndTriggerLifeEvents(player);
            
            CalculateCareerProgress(days, player);
            
            OnLifeDayChanged?.Invoke(currentLifeDay);
            
            if (currentLifeDay >= maxLifeDays)
            {
                EndLifeSimulation(player);
            }
        }

        private void CheckAndTriggerLifeEvents(PlayerState player)
        {
            foreach (var kvp in lifeEvents)
            {
                var evt = kvp.Value;
                if (currentLifeDay >= evt.day && !achievedMilestones.Contains(evt.id))
                {
                    TriggerLifeEvent(evt, player);
                    achievedMilestones.Add(evt.id);
                }
            }
        }

        private void TriggerLifeEvent(LifeEvent evt, PlayerState player)
        {
            player.AddStat("intelligence", evt.effects.intelligence);
            player.AddStat("physical", evt.effects.physical);
            player.AddStat("emotion", evt.effects.emotion);
            player.AddStat("social", evt.effects.social);
            player.AddStat("creativity", evt.effects.creativity);
            player.AddStat("luck", evt.effects.luck);
            player.AddStat("willpower", evt.effects.willpower);
            
            OnMilestoneAchieved?.Invoke(evt.id);
            
            Debug.Log($"[LifeEngine] 触发人生事件: {evt.title}");
        }

        private void CalculateCareerProgress(int days, PlayerState player)
        {
            float workEfficiency = 1f + (player.intelligence / 100f);
            float socialBonus = player.social / 200f;
            
            float progress = days * (workEfficiency + socialBonus) * 0.01f;
            
            if (player.careerLevel < 10 && progress >= 1f)
            {
                player.careerLevel++;
                Debug.Log($"[LifeEngine] 职级提升: {player.careerLevel}");
            }
            
            player.totalScore += progress * player.luck * 0.1f;
        }

        private void EndLifeSimulation(PlayerState player)
        {
            var result = CalculateLifeResult(player);
            OnLifeEnded?.Invoke(result);
            
            Debug.Log($"[LifeEngine] 人生模拟结束，满意度: {result.satisfactionScore}");
        }

        public LifeResult CalculateLifeResult(PlayerState player)
        {
            int satisfactionScore = 0;
            
            satisfactionScore += player.careerLevel * 10;
            satisfactionScore += player.GetAverageStats();
            satisfactionScore += player.happiness;
            satisfactionScore += player.lifeSatisfaction;
            
            satisfactionScore = Mathf.Clamp(satisfactionScore, 0, 100);
            
            LifeEnding ending;
            if (satisfactionScore >= 80)
            {
                ending = LifeEnding.Excellent;
            }
            else if (satisfactionScore >= 60)
            {
                ending = LifeEnding.Good;
            }
            else if (satisfactionScore >= 40)
            {
                ending = LifeEnding.Average;
            }
            else
            {
                ending = LifeEnding.Poor;
            }

            return new LifeResult
            {
                ending = ending,
                satisfactionScore = satisfactionScore,
                careerLevel = player.careerLevel,
                totalScore = player.totalScore,
                milestones = new List<string>(achievedMilestones),
                finalStats = ClonePlayerStats(player)
            };
        }

        private PlayerState ClonePlayerStats(PlayerState player)
        {
            return player.Clone();
        }

        public int GetCurrentLifeDay()
        {
            return currentLifeDay;
        }

        public List<LifeEvent> GetUpcomingLifeEvents()
        {
            var upcoming = new List<LifeEvent>();
            
            foreach (var kvp in lifeEvents)
            {
                if (kvp.Value.day > currentLifeDay && !achievedMilestones.Contains(kvp.Key))
                {
                    upcoming.Add(kvp.Value);
                }
            }
            
            upcoming.Sort((a, b) => a.day.CompareTo(b.day));
            return upcoming;
        }

        public List<string> GetAchievedMilestones()
        {
            return new List<string>(achievedMilestones);
        }

        public void Reset()
        {
            currentLifeDay = 0;
            achievedMilestones.Clear();
            Debug.Log("[LifeEngine] 人生引擎已重置");
        }
    }

    [Serializable]
    public class LifeEvent
    {
        public string id;
        public int day;
        public string title;
        public string description;
        public StatEffects effects;
        public bool isAutoTrigger;
    }

    [Serializable]
    public class LifeResult
    {
        public LifeEnding ending;
        public int satisfactionScore;
        public int careerLevel;
        public float totalScore;
        public List<string> milestones;
        public PlayerState finalStats;
    }

    public enum LifeEnding
    {
        Excellent,
        Good,
        Average,
        Poor
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
    }
