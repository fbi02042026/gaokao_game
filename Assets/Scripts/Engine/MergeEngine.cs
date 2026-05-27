using System;
using System.Collections.Generic;
using UnityEngine;
using GaokaoLife.Models;

namespace GaokaoLife.Engine
{
    public class MergeEngine : MonoBehaviour
    {
        public static MergeEngine Instance { get; private set; }

        private Dictionary<string, MergeRecipe> mergeRecipes;
        private List<string> unlockedCombinations;

        public event Action<string, string, string> OnMergeSuccess;
        public event Action<string> OnItemUnlocked;

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

            mergeRecipes = new Dictionary<string, MergeRecipe>();
            unlockedCombinations = new List<string>();
            InitializeDefaultRecipes();
            
            Debug.Log("[MergeEngine] 合成引擎初始化完成");
        }

        private void InitializeDefaultRecipes()
        {
            AddRecipe("textbook_basic", "textbook_advanced", "scholarship");
            AddRecipe("sports_equipment", "nutrition_plan", "athletic_talent");
            AddRecipe("social_connections", "leadership_book", "leadership_skill");
            AddRecipe("art_supplies", "creative_workshop", "artistic_talent");
            AddRecipe("computer", "programming_books", "tech_talent");
            AddRecipe("music_instrument", "practice_time", "musical_talent");
            AddRecipe("medical_knowledge", "internship_experience", "medical_talent");
            AddRecipe("debate_skills", "writing_practice", "rhetoric_talent");
            
            Debug.Log($"[MergeEngine] 初始化了 {mergeRecipes.Count} 个合成配方");
        }

        public void AddRecipe(string item1, string item2, string result)
        {
            string key = GenerateRecipeKey(item1, item2);
            mergeRecipes[key] = new MergeRecipe
            {
                item1 = item1,
                item2 = item2,
                result = result,
                description = $"合成: {result}"
            };
        }

        private string GenerateRecipeKey(string item1, string item2)
        {
            if (string.Compare(item1, item2, StringComparison.Ordinal) < 0)
            {
                return $"{item1}+{item2}";
            }
            return $"{item2}+{item1}";
        }

        public bool TryMerge(string item1, string item2, PlayerState player)
        {
            string key = GenerateRecipeKey(item1, item2);
            
            if (!mergeRecipes.ContainsKey(key))
            {
                Debug.Log($"[MergeEngine] 没有找到合成配方: {item1} + {item2}");
                return false;
            }

            if (!player.ownedItemIds.Contains(item1) || !player.ownedItemIds.Contains(item2))
            {
                Debug.Log("[MergeEngine] 玩家没有所需的物品");
                return false;
            }

            var recipe = mergeRecipes[key];
            
            player.ownedItemIds.Remove(item1);
            player.ownedItemIds.Remove(item2);
            
            if (!player.ownedItemIds.Contains(recipe.result))
            {
                player.ownedItemIds.Add(recipe.result);
                unlockedCombinations.Add(key);
                
                OnMergeSuccess?.Invoke(item1, item2, recipe.result);
                OnItemUnlocked?.Invoke(recipe.result);
                
                Debug.Log($"[MergeEngine] 合成成功: {item1} + {item2} = {recipe.result}");
                return true;
            }
            else
            {
                Debug.Log("[MergeEngine] 结果物品已拥有");
                return false;
            }
        }

        public MergeRecipe GetRecipe(string item1, string item2)
        {
            string key = GenerateRecipeKey(item1, item2);
            return mergeRecipes.ContainsKey(key) ? mergeRecipes[key] : null;
        }

        public List<MergeRecipe> GetAvailableRecipes(PlayerState player)
        {
            var available = new List<MergeRecipe>();
            
            foreach (var kvp in mergeRecipes)
            {
                var recipe = kvp.Value;
                if (player.ownedItemIds.Contains(recipe.item1) && 
                    player.ownedItemIds.Contains(recipe.item2))
                {
                    available.Add(recipe);
                }
            }

            return available;
        }

        public List<MergeRecipe> GetAllRecipes()
        {
            return new List<MergeRecipe>(mergeRecipes.Values);
        }

        public int GetUnlockedRecipeCount()
        {
            return unlockedCombinations.Count;
        }

        public int GetTotalRecipeCount()
        {
            return mergeRecipes.Count;
        }

        public float GetUnlockProgress()
        {
            if (mergeRecipes.Count == 0) return 0f;
            return (float)unlockedCombinations.Count / mergeRecipes.Count;
        }

        public void Reset()
        {
            unlockedCombinations.Clear();
            Debug.Log("[MergeEngine] 合成引擎已重置");
        }
    }

    [Serializable]
    public class MergeRecipe
    {
        public string item1;
        public string item2;
        public string result;
        public string description;
    }

    public class ScoreEngine : MonoBehaviour
    {
        public static ScoreEngine Instance { get; private set; }

        private CollegeData collegeData;

        public event Action<int> OnScoreCalculated;

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

            Debug.Log("[ScoreEngine] 评分引擎初始化完成");
        }

        public void LoadCollegeData(CollegeData data)
        {
            collegeData = data;
        }

        public int CalculateGaokaoScore(PlayerState player)
        {
            int chinese = player.subjectScores["chinese"];
            int math = player.subjectScores["math"];
            int english = player.subjectScores["english"];
            
            int comprehensive = 0;
            int subjectCount = 0;
            
            foreach (var kvp in player.subjectScores)
            {
                if (kvp.Key != "chinese" && kvp.Key != "math" && kvp.Key != "english")
                {
                    comprehensive += kvp.Value;
                    subjectCount++;
                }
            }
            
            if (subjectCount > 0)
            {
                comprehensive /= subjectCount;
            }

            float intelligenceFactor = 1f + (player.intelligence - 50) / 200f;
            float emotionFactor = 1f + (player.emotion - 50) / 100f;
            float stability = TalentEngine.Instance?.CalculateExamStability(player) ?? 0.7f;
            
            int totalScore = (int)(
                (chinese + math + english + comprehensive) * 
                intelligenceFactor * 
                emotionFactor * 
                stability
            );

            player.gaokaoScore = totalScore;
            OnScoreCalculated?.Invoke(totalScore);
            
            Debug.Log($"[ScoreEngine] 高考分数计算: {totalScore}");
            return totalScore;
        }

        public List<College> GetAvailableColleges(int score, Province province)
        {
            if (collegeData == null)
            {
                return new List<College>();
            }

            return collegeData.GetCollegesByScore(score, province);
        }

        public College RecommendCollege(PlayerState player)
        {
            if (collegeData == null) return null;

            var available = GetAvailableColleges(player.gaokaoScore, player.province);
            
            if (available.Count == 0) return null;

            available.Sort((a, b) => {
                float scoreA = CalculateCollegeMatchScore(a, player);
                float scoreB = CalculateCollegeMatchScore(b, player);
                return scoreB.CompareTo(scoreA);
            });

            return available[0];
        }

        private float CalculateCollegeMatchScore(College college, PlayerState player)
        {
            float score = college.reputation;
            
            if (college.strengthMajors.Count > 0)
            {
                score += 10;
            }
            
            score += college.campusLifeScore * 0.5f;
            score += college.locationScore * 0.3f;
            
            return score;
        }

        public Major RecommendMajor(PlayerState player)
        {
            if (collegeData == null) return null;

            var recommended = collegeData.GetRecommendedMajors(player);
            return recommended.Count > 0 ? recommended[0] : null;
        }

        public List<(College college, Major major, float probability)> CalculateAdmissionResults(
            PlayerState player, 
            List<string> collegePreferences, 
            List<string> majorPreferences)
        {
            var results = new List<(College, Major, float)>();
            
            if (collegeData == null) return results;

            foreach (var collegeId in collegePreferences)
            {
                var college = collegeData.GetCollege(collegeId);
                if (college == null) continue;
                
                if (!college.IsWithinScore(player.gaokaoScore))
                {
                    continue;
                }

                foreach (var majorId in majorPreferences)
                {
                    var major = collegeData.GetMajor(majorId);
                    if (major == null) continue;
                    
                    float probability = CalculateAdmissionProbability(college, major, player);
                    results.Add((college, major, probability));
                }
            }

            results.Sort((a, b) => b.Item3.CompareTo(a.Item3));
            return results;
        }

        private float CalculateAdmissionProbability(College college, Major major, PlayerState player)
        {
            float baseProbability = 0.5f;
            
            int scoreDiff = player.gaokaoScore - college.GetMinScore();
            float scoreFactor = Mathf.Clamp(scoreDiff / 50f, -0.3f, 0.3f);
            
            float majorMatchScore = major.GetMatchScore(player) / 100f;
            
            float luckFactor = player.luck / 200f;
            
            float reputationFactor = college.reputation / 500f;
            
            return Mathf.Clamp(
                baseProbability + scoreFactor + majorMatchScore + luckFactor - reputationFactor,
                0.05f,
                0.95f
            );
        }
    }
}
