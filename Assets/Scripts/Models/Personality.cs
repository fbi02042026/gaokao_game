using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Personality
{
    public string id;
    public string name;
    public string description;
    public string icon;
    public string tagline;

    public PersonalityType type;

    public int baseIntellect;
    public int baseMental;
    public int baseSocial;
    public int baseHealth;

    public List<string> strengths;
    public List<string> weaknesses;

    public List<string> suitableMajors;
    public List<string> suitableCareers;

    public List<string> bonusTalents;
    public List<string> lockedTalents;

    public string specialAbility;
    public float specialEffectValue;

    public int gaokaoBonus;
    public int careerBonus;

    public Personality()
    {
        strengths = new List<string>();
        weaknesses = new List<string>();
        suitableMajors = new List<string>();
        suitableCareers = new List<string>();
        bonusTalents = new List<string>();
        lockedTalents = new List<string>();
    }

    public PlayerState ModifyPlayerState(PlayerState player)
    {
        player.intellect = Mathf.Clamp(player.intellect + baseIntellect, 0, 100);
        player.mental = Mathf.Clamp(player.mental + baseMental, 0, 100);
        player.social = Mathf.Clamp(player.social + baseSocial, 0, 100);
        player.health = Mathf.Clamp(player.health + baseHealth, 0, 100);

        foreach (var talentId in bonusTalents)
        {
            player.UnlockTalent(talentId);
        }

        return player;
    }

    public int CalculateGaokaoScore(PlayerState player, List<int> subjectScores)
    {
        int total = 0;
        foreach (var score in subjectScores)
        {
            total += score;
        }

        float personalityMultiplier = 1f + (gaokaoBonus / 100f);
        float mentalStability = 1f + (player.mental / 500f);

        return (int)(total * personalityMultiplier * mentalStability);
    }

    public string GetCareerSuggestion(PlayerState player)
    {
        if (suitableCareers.Count == 0)
        {
            return "待探索";
        }

        int bestMatch = 0;
        string bestCareer = suitableCareers[0];

        foreach (var career in suitableCareers)
        {
            int matchScore = 0;

            matchScore += (100 - Mathf.Abs(player.intellect - 70)) / 10;
            matchScore += (100 - Mathf.Abs(player.social - 70)) / 10;
            matchScore += (100 - Mathf.Abs(player.health - 70)) / 10;

            if (matchScore > bestMatch)
            {
                bestMatch = matchScore;
                bestCareer = career;
            }
        }

        return bestCareer;
    }
}

[Serializable]
public class PersonalityData
{
    public List<Personality> personalities;

    public PersonalityData()
    {
        personalities = new List<Personality>();
    }

    public Personality GetPersonality(string id)
    {
        return personalities.Find(p => p.id == id);
    }

    public Personality GetPersonalityByType(PersonalityType type)
    {
        return personalities.Find(p => p.type == type);
    }

    public Personality CalculatePersonality(PlayerState player)
    {
        int maxStat = Mathf.Max(
            player.intellect,
            player.mental,
            player.social,
            player.health
        );

        PersonalityType calculatedType;

        if (player.intellect >= 80 && player.health >= 70)
        {
            calculatedType = PersonalityType.Academic;
        }
        else if (player.social >= 80)
        {
            calculatedType = PersonalityType.Creative;
        }
        else if (player.social >= 70 && player.mental >= 60)
        {
            calculatedType = PersonalityType.Social;
        }
        else if (player.health >= 80)
        {
            calculatedType = PersonalityType.Athletic;
        }
        else if (maxStat >= 70)
        {
            calculatedType = PersonalityType.Perfectionist;
        }
        else if (player.mental <= 30)
        {
            calculatedType = PersonalityType.Rebel;
        }
        else if (player.studyDays >= 800 && player.intellect < 60)
        {
            calculatedType = PersonalityType.LateBloomer;
        }
        else
        {
            calculatedType = PersonalityType.Balanced;
        }

        player.personality = calculatedType;
        return GetPersonalityByType(calculatedType);
    }
}

[Serializable]
public class Memory
{
    public string id;
    public string title;
    public string content;

    public string relatedEventId;
    public string relatedNPCId;

    public GamePhase unlockPhase;
    public bool isDejaVuTrigger;

    public string dejaVuText;
    public List<MemoryChoice> choices;

    public Memory()
    {
        choices = new List<MemoryChoice>();
        isDejaVuTrigger = false;
    }
}

[Serializable]
public class MemoryChoice
{
    public string text;
    public int satisfactionBonus;
    public int statBonus;
    public List<string> unlockTalents;
    public string specialEffect;
}

[Serializable]
public class MemoryData
{
    public List<Memory> memories;

    public MemoryData()
    {
        memories = new List<Memory>();
    }

    public Memory GetMemory(string id)
    {
        return memories.Find(m => m.id == id);
    }

    public List<Memory> GetMemoriesByEvent(string eventId)
    {
        return memories.FindAll(m => m.relatedEventId == eventId);
    }

    public List<Memory> GetDejaVuMemories()
    {
        return memories.FindAll(m => m.isDejaVuTrigger);
    }
}