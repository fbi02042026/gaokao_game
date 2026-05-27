using System;
using System.Collections.Generic;
using UnityEngine;

namespace GaokaoLife.Models
{
    [Serializable]
    public class College
    {
        public string id;
        public string name;
        public string nameEn;
        
        public CollegeLevel level;
        public string location;
        public Province province;
        
        public int ranking;
        public float reputation;
        public int tuition;
        
        public List<string> tags;
        public List<string> strengthMajors;
        public List<string> notableAlumni;
        
        public int minScore2024;
        public int minScore2023;
        public int minScore2022;
        
        public int admissionCapacity;
        public float employmentRate;
        public float furtherStudyRate;
        
        public float campusLifeScore;
        public float locationScore;
        public float costScore;
        
        public List<string> availableMajors;
        
        public College()
        {
            tags = new List<string>();
            strengthMajors = new List<string>();
            notableAlumni = new List<string>();
            availableMajors = new List<string>();
        }

        public int GetMinScore(int year)
        {
            switch (year)
            {
                case 2024:
                    return minScore2024;
                case 2023:
                    return minScore2023;
                case 2022:
                    return minScore2022;
                default:
                    return minScore2024;
            }
        }

        public float GetOverallScore()
        {
            return (reputation + campusLifeScore + locationScore) / 3f;
        }

        public bool IsWithinScore(int score, int year = 2024)
        {
            return score >= GetMinScore(year);
        }

        public string GetLevelName()
        {
            switch (level)
            {
                case CollegeLevel.Top985:
                    return "顶尖985";
                case CollegeLevel.Regular985:
                    return "普通985";
                case CollegeLevel.Top211:
                    return "顶尖211";
                case CollegeLevel.Regular211:
                    return "普通211";
                case CollegeLevel.FirstBatch:
                    return "一本";
                case CollegeLevel.SecondBatch:
                    return "二本";
                case CollegeLevel.Private:
                    return "民办/专科";
                default:
                    return "未知";
            }
        }
    }

    public enum CollegeLevel
    {
        Top985,
        Regular985,
        Top211,
        Regular211,
        FirstBatch,
        SecondBatch,
        Private
    }

    [Serializable]
    public class Major
    {
        public string id;
        public string name;
        public string category;
        
        public int difficulty;
        public int popularity;
        public int careerProspects;
        
        public List<string> requiredSubjects;
        public List<string> relatedMajors;
        
        public int avgSalary;
        public int topSalary;
        public float employmentRate;
        
        public List<string> suitablePersonalities;
        public List<string> suitableTalents;
        
        public string description;
        public string careerPaths;
        
        public List<string> recommendedColleges;
        
        public Major()
        {
            requiredSubjects = new List<string>();
            relatedMajors = new List<string>();
            suitablePersonalities = new List<string>();
            suitableTalents = new List<string>();
            recommendedColleges = new List<string>();
        }

        public int GetMatchScore(PlayerState player)
        {
            int score = 0;
            
            foreach (var personality in suitablePersonalities)
            {
                if (player.personality.ToString() == personality)
                {
                    score += 20;
                }
            }
            
            foreach (var talent in suitableTalents)
            {
                if (player.HasTalent(talent))
                {
                    score += 15;
                }
            }
            
            score += (int)(careerProspects / 5);
            
            return score;
        }

        public string GetCategoryName()
        {
            switch (category)
            {
                case "engineering":
                    return "工学";
                case "science":
                    return "理学";
                case "medicine":
                    return "医学";
                case "arts":
                    return "艺术学";
                case "literature":
                    return "文学";
                case "economics":
                    return "经济学";
                case "management":
                    return "管理学";
                case "law":
                    return "法学";
                case "education":
                    return "教育学";
                case "history":
                    return "历史学";
                case "philosophy":
                    return "哲学";
                default:
                    return "其他";
            }
        }
    }

    [Serializable]
    public class CollegeData
    {
        public List<College> colleges;
        public List<Major> majors;

        public CollegeData()
        {
            colleges = new List<College>();
            majors = new List<Major>();
        }

        public College GetCollege(string id)
        {
            return colleges.Find(c => c.id == id);
        }

        public Major GetMajor(string id)
        {
            return majors.Find(m => m.id == id);
        }

        public List<College> GetCollegesByLevel(CollegeLevel level)
        {
            return colleges.FindAll(c => c.level == level);
        }

        public List<College> GetCollegesByProvince(Province province)
        {
            return colleges.FindAll(c => c.province == province);
        }

        public List<College> GetCollegesByScore(int score, Province province)
        {
            return colleges.FindAll(c => 
                c.province == province && 
                c.IsWithinScore(score));
        }

        public List<Major> GetMajorsByCategory(string category)
        {
            return majors.FindAll(m => m.category == category);
        }

        public List<Major> GetRecommendedMajors(PlayerState player)
        {
            var recommended = new List<Major>();
            
            foreach (var major in majors)
            {
                int matchScore = major.GetMatchScore(player);
                if (matchScore >= 30)
                {
                    recommended.Add(major);
                }
            }
            
            recommended.Sort((a, b) => b.GetMatchScore(player).CompareTo(a.GetMatchScore(player)));
            
            return recommended;
        }
    }
}
