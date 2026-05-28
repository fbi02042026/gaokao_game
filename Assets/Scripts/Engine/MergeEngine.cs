using System;
using System.Collections.Generic;
using UnityEngine;

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
