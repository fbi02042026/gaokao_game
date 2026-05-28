using System;
using System.Collections.Generic;

[Serializable]
public class College
{
    public string id;
    public string name;
    public string level;
    public string province;
    public string type;
    public string[] majors;
    public CollegeScoreEntry[] scores;

    public int GetMinScore(string provinceName, int year = 2024)
    {
        if (scores == null) return 0;
        foreach (var s in scores)
        {
            if (s.province == provinceName && s.year == year)
                return s.min;
        }
        return 0;
    }

    public int GetAvgScore(string provinceName, int year = 2024)
    {
        if (scores == null) return 0;
        foreach (var s in scores)
        {
            if (s.province == provinceName && s.year == year)
                return s.avg;
        }
        return 0;
    }
}

[Serializable]
public class CollegeScoreEntry
{
    public string province;
    public int year;
    public int min;
    public int avg;
}