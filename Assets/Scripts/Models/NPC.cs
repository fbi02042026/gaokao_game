using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NPC
    {
        public string id;
        public string name;
        public string role;
        
        public NPCType type;
        public string description;
        
        public int initialImpression;
        public int currentImpression;
        public List<string> relationshipLevels;
        
        public List<string> availableEvents;
        public List<string> unlockedMemories;
        
        public List<NPCDialogue> dialogues;
        public List<NPCGift> preferredGifts;
        
        public bool isAvailable;
        public int meetCount;

        public NPC()
        {
            relationshipLevels = new List<string>();
            availableEvents = new List<string>();
            unlockedMemories = new List<string>();
            dialogues = new List<NPCDialogue>();
            preferredGifts = new List<NPCGift>();
            isAvailable = true;
            meetCount = 0;
        }

        public string GetRelationshipLevel()
        {
            if (currentImpression >= 90) return "挚友";
            if (currentImpression >= 70) return "好友";
            if (currentImpression >= 50) return "普通朋友";
            if (currentImpression >= 30) return "点头之交";
            if (currentImpression >= 10) return "陌生人";
            return "陌生人";
        }

        public void ModifyImpression(int delta)
        {
            currentImpression = Mathf.Clamp(currentImpression + delta, 0, 100);
            Debug.Log($"[NPC] {name} 好感度变化: {delta} (当前: {currentImpression})");
        }

        public bool HasUnlockedMemory(string memoryId)
        {
            return unlockedMemories.Contains(memoryId);
        }

        public void UnlockMemory(string memoryId)
        {
            if (!unlockedMemories.Contains(memoryId))
            {
                unlockedMemories.Add(memoryId);
                Debug.Log($"[NPC] {name} 解锁回忆: {memoryId}");
            }
        }

        public NPCDialogue GetDialogue(string dialogueId)
        {
            return dialogues.Find(d => d.id == dialogueId);
        }

        public bool CanTriggerEvent(string eventId)
        {
            return availableEvents.Contains(eventId) && isAvailable;
        }
    }

    public enum NPCType
    {
        Teacher,
        Classmate,
        Family,
        Friend,
        Romance,
        Stranger,
        Celebrity
    }

    [Serializable]
    public class NPCDialogue
    {
        public string id;
        public string text;
        public string speaker;
        public List<DialogueChoice> choices;
        public bool isMemory;
        public string memoryTrigger;

        public NPCDialogue()
        {
            choices = new List<DialogueChoice>();
            isMemory = false;
        }
    }

    [Serializable]
    public class DialogueChoice
    {
        public string text;
        public string resultText;
        public int impressionChange;
        public List<string> unlocks;
        public int happinessChange;
        public bool isOptimal;
    }

    [Serializable]
    public class NPCGift
    {
        public string itemId;
        public string itemName;
        public int value;
        public int impressionBonus;
        public string preference;
    }

    [Serializable]
    public class NPCData
    {
        public List<NPC> npcs;

        public NPCData()
        {
            npcs = new List<NPC>();
        }

        public NPC GetNPC(string id)
        {
            return npcs.Find(n => n.id == id);
        }

        public List<NPC> GetNPCsByType(NPCType type)
        {
            return npcs.FindAll(n => n.type == type);
        }

        public List<NPC> GetAvailableNPCs()
        {
            return npcs.FindAll(n => n.isAvailable);
        }
    }
