using System;

[Serializable]
public class Major
{
    public string id;
    public string name;
    public string category;
    public string description;
    public string[] career;
    public string salary;
    public NpcReview[] npcReviews;
}

[Serializable]
public class NpcReview
{
    public string npc;
    public string text;
}