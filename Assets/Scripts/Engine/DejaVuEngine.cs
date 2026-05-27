using System;
using System.Collections.Generic;
using UnityEngine;
using GaokaoLife.Models;

namespace GaokaoLife.Engine
{
    public class DejaVuEngine : MonoBehaviour
    {
        public static DejaVuEngine Instance { get; private set; }

        private MemoryData memoryData;
        private List<string> activatedDejaVu;

        public event Action<Memory, GameEvent> OnDejaVuTriggered;
        public event Action<string> OnMemoryUnlocked;
        public event Action<Memory, MemoryChoice> OnMemoryChoiceSelected;

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

            activatedDejaVu = new List<string>();
            Debug.Log("[DejaVuEngine] 既视感引擎初始化完成");
        }

        public void LoadMemories(MemoryData data)
        {
            memoryData = data;
            Debug.Log($"[DejaVuEngine] 加载了 {data.memories.Count} 个记忆配置");
        }

        public void InitializePlayerMemories(PlayerState player)
        {
            if (!player.hasPastLifeMemory)
            {
                Debug.Log("[DejaVuEngine] 玩家没有前世记忆天赋");
                return;
            }

            activatedDejaVu.Clear();
            Debug.Log($"[DejaVuEngine] 玩家拥有前世记忆，解锁 {player.memories.Count} 个记忆");
        }

        public Memory CheckForDejaVu(GameEvent currentEvent, PlayerState player)
        {
            if (!player.hasPastLifeMemory)
            {
                return null;
            }

            if (memoryData == null)
            {
                return null;
            }

            var relatedMemories = memoryData.GetMemoriesByEvent(currentEvent.id);
            
            foreach (var memory in relatedMemories)
            {
                if (memory.isDejaVuTrigger && !activatedDejaVu.Contains(memory.id))
                {
                    if (ShouldShowDejaVu(memory, player))
                    {
                        activatedDejaVu.Add(memory.id);
                        OnDejaVuTriggered?.Invoke(memory, currentEvent);
                        
                        Debug.Log($"[DejaVuEngine] 触发既视感: {memory.title}");
                        return memory;
                    }
                }
            }

            return null;
        }

        private bool ShouldShowDejaVu(Memory memory, PlayerState player)
        {
            if (player.HasMemory(memory.id))
            {
                return false;
            }

            if (memory.unlockPhase != player.currentPhase)
            {
                return false;
            }

            return true;
        }

        public void ProcessMemoryChoice(Memory memory, int choiceIndex, PlayerState player)
        {
            if (choiceIndex < 0 || choiceIndex >= memory.choices.Count)
            {
                Debug.LogWarning($"[DejaVuEngine] 无效的回忆选择: {choiceIndex}");
                return;
            }

            var choice = memory.choices[choiceIndex];
            
            player.happiness = Mathf.Clamp(player.happiness + choice.happinessBonus, 0, 100);
            
            if (choice.statBonus != 0)
            {
                player.AddStat("emotion", choice.statBonus);
            }
            
            foreach (var talentId in choice.unlockTalents)
            {
                player.UnlockTalent(talentId);
            }

            if (!string.IsNullOrEmpty(choice.specialEffect))
            {
                ApplySpecialEffect(choice.specialEffect, player);
            }

            player.AddMemory(memory.id);
            
            OnMemoryChoiceSelected?.Invoke(memory, choice);
            
            Debug.Log($"[DejaVuEngine] 处理记忆选择: {choice.text}");
        }

        private void ApplySpecialEffect(string effectType, PlayerState player)
        {
            switch (effectType)
            {
                case "boost_luck":
                    player.AddStat("luck", 20);
                    break;
                case "boost_willpower":
                    player.AddStat("willpower", 20);
                    break;
                case "reduce_stress":
                    player.stressLevel = Mathf.Max(0, player.stressLevel - 20);
                    break;
                case "reveal_hidden":
                    Debug.Log("[DejaVuEngine] 解锁隐藏选项");
                    break;
                case "change_fate":
                    player.AddStat("luck", 30);
                    player.AddStat("willpower", 10);
                    break;
            }
        }

        public Memory GetMemory(string memoryId)
        {
            return memoryData?.GetMemory(memoryId);
        }

        public List<Memory> GetPlayerMemories(PlayerState player)
        {
            var memories = new List<Memory>();
            
            if (memoryData == null) return memories;

            foreach (var memoryId in player.memories)
            {
                var memory = memoryData.GetMemory(memoryId);
                if (memory != null)
                {
                    memories.Add(memory);
                }
            }

            return memories;
        }

        public List<Memory> GetActivatedDejaVuMemories()
        {
            var memories = new List<Memory>();
            
            if (memoryData == null) return memories;

            foreach (var memoryId in activatedDejaVu)
            {
                var memory = memoryData.GetMemory(memoryId);
                if (memory != null)
                {
                    memories.Add(memory);
                }
            }

            return memories;
        }

        public List<Memory> GetAvailableDejaVuForEvent(GameEvent evt, PlayerState player)
        {
            var available = new List<Memory>();
            
            if (!player.hasPastLifeMemory || memoryData == null)
            {
                return available;
            }

            var relatedMemories = memoryData.GetMemoriesByEvent(evt.id);
            
            foreach (var memory in relatedMemories)
            {
                if (memory.isDejaVuTrigger && 
                    !activatedDejaVu.Contains(memory.id) && 
                    memory.unlockPhase == player.currentPhase)
                {
                    available.Add(memory);
                }
            }

            return available;
        }

        public int GetDejaVuCount(PlayerState player)
        {
            return activatedDejaVu.Count;
        }

        public float GetDejaVuActivationRate()
        {
            if (memoryData == null || memoryData.memories.Count == 0)
            {
                return 0f;
            }
            
            return (float)activatedDejaVu.Count / memoryData.memories.Count;
        }

        public void UnlockMemory(string memoryId, PlayerState player)
        {
            if (!player.memories.Contains(memoryId))
            {
                player.AddMemory(memoryId);
                OnMemoryUnlocked?.Invoke(memoryId);
                
                Debug.Log($"[DejaVuEngine] 解锁记忆: {memoryId}");
            }
        }

        public bool IsDejaVuActivated(string memoryId)
        {
            return activatedDejaVu.Contains(memoryId);
        }

        public void Reset()
        {
            activatedDejaVu.Clear();
            Debug.Log("[DejaVuEngine] 既视感引擎已重置");
        }
    }
}
