using System;
using System.Collections.Generic;

[Serializable]
public class Province
{
    public string id;
    public string name;
    public string code;
    public string gaokaoMode;
    public string abbreviation;
    
    public float difficulty;
    public int totalStudents;
    
    public int gaokaoPapers;
    public List<string> subjectCombinations;
    
    public ProvinceScoreLine scoreLine2024;
    public ProvinceScoreLine scoreLine2023;
    
    public List<string> nearbyProvinces;
    public bool useIndependentPapers;
    
    public Province()
    {
        nearbyProvinces = new List<string>();
        subjectCombinations = new List<string>();
    }

    public int GetScoreLine(string collegeLevel, int year = 2024)
    {
        var line = year == 2024 ? scoreLine2024 : scoreLine2023;
        
        if (line == null) return 0;
        
        switch (collegeLevel)
        {
            case "Top985":
                return line.top985;
            case "Regular985":
                return line.regular985;
            case "Top211":
                return line.top211;
            case "Regular211":
                return line.regular211;
            case "FirstBatch":
                return line.firstBatch;
            case "SecondBatch":
                return line.secondBatch;
            default:
                return 0;
        }
    }

    public float GetCompetitionRate()
    {
        return (float)totalStudents / 100000f;
    }
}

[Serializable]
public class ProvinceScoreLine
{
    public int top985;
    public int regular985;
    public int top211;
    public int regular211;
    public int firstBatch;
    public int secondBatch;
    
    public int physicsTop;
    public int historyTop;
}

public class ProvinceData
{
    public List<Province> provinces;

    public ProvinceData()
    {
        provinces = new List<Province>();
    }

    public Province GetProvince(string id)
    {
        return provinces.Find(p => p.id == id);
    }

    public Province GetProvinceByName(string name)
    {
        return provinces.Find(p => p.name == name);
    }
}