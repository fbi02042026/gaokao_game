using System;
using System.Collections.Generic;

[Serializable]
public class Achievement
{
    public string id;
    public string name;
    public string description;
    public string icon;
    public string category;
    public int totalRequired;
}

[Serializable]
public class AchievementData
{
    public List<Achievement> achievements;
}