using System;

[Serializable]
public class Talent
{
    public string id;
    public string name;
    public string icon;
    public string rarity;
    public TalentEffects effects;
    public TalentStatModifiers statModifiers;
}

[Serializable]
public class TalentEffects
{
    public string highschool;
    public string gaokao;
    public MajorEffect major;
    public string career;
}

[Serializable]
public class MajorEffect
{
    public string[] boost;
    public string[] warn;
}

[Serializable]
public class TalentStatModifiers
{
    public StatBonus highschool;
    public GaokaoModifier gaokao;
}

[Serializable]
public class StatBonus
{
    public int intellect;
    public int mental;
    public int social;
    public int health;

    public StatBonus Sum(StatBonus other)
    {
        return new StatBonus
        {
            intellect = this.intellect + other.intellect,
            mental = this.mental + other.mental,
            social = this.social + other.social,
            health = this.health + other.health
        };
    }
}

[Serializable]
public class GaokaoModifier
{
    public int baseBonus;
    public int volatility;
    public float critChance;
    public int critBonus;
}