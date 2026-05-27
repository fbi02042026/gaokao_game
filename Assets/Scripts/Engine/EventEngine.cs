using System;
using System.Collections.Generic;
using UnityEngine;
using GaokaoLife.Models;

namespace GaokaoLife.Engine
{
    public class EventEngine : MonoBehaviour
    {
        public static EventEngine Instance { get; private set; }

        private List<GameEvent> eventPool;
        private List<GameEvent> availableEvents;
        private GameEvent currentEvent;

        private Dictionary<string, List<GameEvent>> phaseEvents;
        
        public event Action<GameEvent> OnEventTriggered;
        public event Action<GameEvent, int> OnChoiceSelected;
        public event Action<List<GameEvent>> OnEventsUpdated;

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

            Initialize();
        }

        private void Initialize()
        {
            eventPool = new List<GameEvent>();
            availableEvents = new List<GameEvent>();
            phaseEvents = new Dictionary<string, List<GameEvent>>();
            
            Debug.Log("[EventEngine] 事件引擎初始化完成");
        }

        public void LoadEvents(List<GameEvent> events)
        {
            eventPool.Clear();
            eventPool.AddRange(events);
            
            OrganizeEventsByPhase();
            
            Debug.Log($"[EventEngine] 加载了 {events.Count} 个事件");
        }

        private void OrganizeEventsByPhase()
        {
            phaseEvents.Clear();
            
            foreach (var evt in eventPool)
            {
                string phaseKey = evt.phase.ToString();
                if (!phaseEvents.ContainsKey(phaseKey))
                {
                    phaseEvents[phaseKey] = new List<GameEvent>();
                }
                phaseEvents[phaseKey].Add(evt);
            }
        }

        public void UpdateAvailableEvents(PlayerState player)
        {
            availableEvents.Clear();
            
            string phaseKey = player.currentPhase.ToString();
            List<GameEvent> phaseEventList;
            
            if (phaseEvents.TryGetValue(phaseKey, out phaseEventList))
            {
                foreach (var evt in phaseEventList)
                {
                    if (evt.CanTrigger(player))
                    {
                        availableEvents.Add(evt);
                    }
                }
            }
            
            availableEvents.Sort((a, b) => b.priority.CompareTo(a.priority));
            
            OnEventsUpdated?.Invoke(availableEvents);
            
            Debug.Log($"[EventEngine] 可用事件数量: {availableEvents.Count}");
        }

        public GameEvent TriggerRandomEvent(PlayerState player)
        {
            UpdateAvailableEvents(player);
            
            if (availableEvents.Count == 0)
            {
                Debug.Log("[EventEngine] 没有可用的事件");
                return null;
            }

            float randomValue = UnityEngine.Random.value;
            int eventIndex = Mathf.Clamp(
                (int)(randomValue * availableEvents.Count),
                0,
                availableEvents.Count - 1
            );
            
            currentEvent = availableEvents[eventIndex];
            OnEventTriggered?.Invoke(currentEvent);
            
            Debug.Log($"[EventEngine] 触发事件: {currentEvent.title}");
            return currentEvent;
        }

        public GameEvent TriggerSpecificEvent(PlayerState player, string eventId)
        {
            var evt = eventPool.Find(e => e.id == eventId);
            if (evt == null)
            {
                Debug.LogWarning($"[EventEngine] 找不到事件: {eventId}");
                return null;
            }

            if (!evt.CanTrigger(player))
            {
                Debug.LogWarning($"[EventEngine] 事件无法触发: {eventId}");
                return null;
            }

            currentEvent = evt;
            OnEventTriggered?.Invoke(currentEvent);
            
            Debug.Log($"[EventEngine] 触发特定事件: {evt.title}");
            return evt;
        }

        public void ExecuteChoice(PlayerState player, int choiceIndex)
        {
            if (currentEvent == null)
            {
                Debug.LogWarning("[EventEngine] 没有当前事件");
                return;
            }

            currentEvent.ExecuteChoice(player, choiceIndex);
            OnChoiceSelected?.Invoke(currentEvent, choiceIndex);
            
            Debug.Log($"[EventEngine] 执行选择 {choiceIndex}: {currentEvent.choices[choiceIndex].text}");
            
            if (currentEvent.immediateEffect != null)
            {
                ApplyImmediateEffect(player, currentEvent.immediateEffect);
            }
        }

        private void ApplyImmediateEffect(PlayerState player, EventEffect effect)
        {
            if (effect == null) return;
            
            if (effect.stats != null)
            {
                player.AddStat("intelligence", effect.stats.intelligence);
                player.AddStat("physical", effect.stats.physical);
                player.AddStat("emotion", effect.stats.emotion);
                player.AddStat("social", effect.stats.social);
                player.AddStat("creativity", effect.stats.creativity);
                player.AddStat("luck", effect.stats.luck);
                player.AddStat("willpower", effect.stats.willpower);
            }
            
            player.stressLevel = Mathf.Clamp(player.stressLevel + effect.stressChange, 0, 100);
            player.happiness = Mathf.Clamp(player.happiness + effect.happinessChange, 0, 100);
            
            foreach (var talentId in effect.unlockTalents)
            {
                player.UnlockTalent(talentId);
            }
            
            foreach (var memory in effect.addMemories)
            {
                player.AddMemory(memory);
            }
        }

        public GameEvent GetCurrentEvent()
        {
            return currentEvent;
        }

        public List<GameEvent> GetAvailableEvents()
        {
            return new List<GameEvent>(availableEvents);
        }

        public List<GameEvent> GetEventsByCategory(EventCategory category)
        {
            return eventPool.FindAll(e => e.category == category);
        }

        public List<GameEvent> GetEventsByPhase(GamePhase phase)
        {
            string phaseKey = phase.ToString();
            if (phaseEvents.ContainsKey(phaseKey))
            {
                return new List<GameEvent>(phaseEvents[phaseKey]);
            }
            return new List<GameEvent>();
        }

        public void AddCustomEvent(GameEvent evt)
        {
            if (!eventPool.Contains(evt))
            {
                eventPool.Add(evt);
                OrganizeEventsByPhase();
                Debug.Log($"[EventEngine] 添加自定义事件: {evt.title}");
            }
        }

        public bool RemoveEvent(string eventId)
        {
            var evt = eventPool.Find(e => e.id == eventId);
            if (evt != null)
            {
                eventPool.Remove(evt);
                OrganizeEventsByPhase();
                Debug.Log($"[EventEngine] 移除事件: {eventId}");
                return true;
            }
            return false;
        }

        public int GetTotalEventCount()
        {
            return eventPool.Count;
        }

        public int GetAvailableEventCount()
        {
            return availableEvents.Count;
        }

        public Dictionary<string, int> GetEventStats()
        {
            var stats = new Dictionary<string, int>();
            
            foreach (EventCategory category in Enum.GetValues(typeof(EventCategory)))
            {
                stats[category.ToString()] = eventPool.FindAll(e => e.category == category).Count;
            }
            
            return stats;
        }

        public void ClearCurrentEvent()
        {
            currentEvent = null;
        }

        public void Reset()
        {
            currentEvent = null;
            availableEvents.Clear();
            Debug.Log("[EventEngine] 事件引擎已重置");
        }
    }
}
