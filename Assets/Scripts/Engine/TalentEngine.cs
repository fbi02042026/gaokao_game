using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class TalentListWrapper { public List<Talent> talents; }

public class TalentEngine : MonoBehaviour
{
    public static TalentEngine Instance { get; private set; }

    private List<Talent> allTalents;
    private Talent currentTalent;
    private int pityCounter = 0;
    private const int PITY_RARE = 4;
    private const int PITY_EPIC = 8;

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }

        LoadFromResources();
    }

    void LoadFromResources()
    {
        var jsonAsset = Resources.Load<TextAsset>("Data/talents");
        if (jsonAsset != null)
        {
            var wrapper = JsonUtility.FromJson<TalentListWrapper>("{\"talents\":" + jsonAsset.text + "}");
            allTalents = wrapper?.talents ?? new List<Talent>();
            Debug.Log($"[TalentEngine] 从 Resources 加载 {allTalents.Count} 个天赋");
        }
        else
        {
            Debug.LogWarning("[TalentEngine] Data/talents.json 未找到，天赋数据为空");
            allTalents = new List<Talent>();
        }
    }

    public void LoadTalents(List<Talent> talents)
    {
        allTalents = talents ?? new List<Talent>();
        Debug.Log($"[TalentEngine] 加载 {allTalents.Count} 个天赋");
    }

    public void SetCurrentTalent(Talent talent)
    {
        currentTalent = talent;
        Debug.Log($"[TalentEngine] 选中天赋: {talent?.name}");
    }

    public Talent GetCurrentTalent()
    {
        return currentTalent;
    }

    public List<Talent> DrawTalents()
    {
        if (allTalents == null || allTalents.Count == 0)
        {
            Debug.LogWarning("[TalentEngine] 天赋池为空");
            return new List<Talent>();
        }

        var pool = allTalents.Where(t => t.rarity != "legendary").ToList();
        var result = new List<Talent>();
        bool hasRareOrBetter = false;

        for (int i = 0; i < 3 && pool.Count > 0; i++)
        {
            Talent picked = null;

            if (pityCounter >= PITY_EPIC)
            {
                var epics = pool.Where(t => t.rarity == "epic").ToList();
                if (epics.Count > 0)
                {
                    picked = epics[Random.Range(0, epics.Count)];
                    Debug.Log($"[TalentEngine] 保底触发! 必出史诗");
                }
            }
            else if (pityCounter >= PITY_RARE)
            {
                var rares = pool.Where(t => t.rarity == "rare" || t.rarity == "epic").ToList();
                if (rares.Count > 0)
                {
                    picked = rares[Random.Range(0, rares.Count)];
                    Debug.Log($"[TalentEngine] 保底触发! 必出稀有");
                }
            }

            if (picked == null)
            {
                var weighted = pool.Select(t => new { Talent = t, Weight = GetRarityWeight(t.rarity) }).ToList();
                int totalWeight = weighted.Sum(w => w.Weight);
                if (totalWeight <= 0)
                {
                    picked = pool[Random.Range(0, pool.Count)];
                }
                else
                {
                    int rand = Random.Range(0, totalWeight);
                    int cumulative = 0;
                    foreach (var w in weighted)
                    {
                        cumulative += w.Weight;
                        if (rand < cumulative)
                        {
                            picked = w.Talent;
                            break;
                        }
                    }
                    if (picked == null) picked = weighted[^1].Talent;
                }
            }

            result.Add(picked);
            pool.Remove(picked);

            if (picked.rarity == "epic" || picked.rarity == "rare")
                hasRareOrBetter = true;
        }

        if (hasRareOrBetter)
            pityCounter = 0;
        else
            pityCounter++;

        Debug.Log($"[TalentEngine] 抽卡结果 保底计数:{pityCounter}: {string.Join(", ", result.Select(t => t.name))}");
        return result;
    }

    private int GetRarityWeight(string rarity)
    {
        return rarity switch
        {
            "common" => 60,
            "rare" => 30,
            "epic" => 10,
            _ => 10
        };
    }

    public static string RarityColor(string rarity)
    {
        return rarity switch
        {
            "common" => "#9E9E9E",
            "rare" => "#6B9DF7",
            "epic" => "#DDA0DD",
            "legendary" => "#FFD93D",
            _ => "#9E9E9E"
        };
    }

    public string GetTalentEffect(string talentId, string stage)
    {
        var talent = allTalents.Find(t => t.id == talentId);
        if (talent?.effects == null) return "";

        return stage switch
        {
            "highschool" => talent.effects.highschool,
            "gaokao" => talent.effects.gaokao,
            "career" => talent.effects.career,
            _ => ""
        };
    }

    public int GetMajorMatchModifier(string talentId, string majorCategory)
    {
        var talent = allTalents.Find(t => t.id == talentId);
        if (talent?.effects?.major == null) return 0;

        if (talent.effects.major.boost != null && talent.effects.major.boost.Contains(majorCategory))
            return 20;
        if (talent.effects.major.warn != null && talent.effects.major.warn.Contains(majorCategory))
            return -25;

        return 0;
    }

    public int ApplyGaokaoModifiers(string talentId, int baseScore)
    {
        var talent = allTalents.Find(t => t.id == talentId);
        if (talent?.statModifiers?.gaokao == null) return baseScore;

        var mod = talent.statModifiers.gaokao;

        int score = baseScore + mod.baseBonus;
        score += Random.Range(-mod.volatility, mod.volatility + 1);

        if (Random.value < mod.critChance)
        {
            score += mod.critBonus;
            Debug.Log($"[TalentEngine] {talent.name} 爆分! +{mod.critBonus}");
        }

        return Mathf.Clamp(score, 200, 750);
    }

    public void ApplyHighschoolModifiers(string talentId, PlayerState state)
    {
        var talent = allTalents.Find(t => t.id == talentId);
        if (talent?.statModifiers?.highschool == null) return;

        var bonus = talent.statModifiers.highschool;
        var changes = new Dictionary<string, int>();
        if (bonus.intellect != 0) changes["intellect"] = bonus.intellect;
        if (bonus.mental != 0) changes["mental"] = bonus.mental;
        if (bonus.social != 0) changes["social"] = bonus.social;
        if (bonus.health != 0) changes["health"] = bonus.health;
        state.ApplyChanges(changes);

        Debug.Log($"[TalentEngine] {talent.name} 高中属性修正: intellect={bonus.intellect} mental={bonus.mental} social={bonus.social} health={bonus.health}");
    }

    public string GetCareerStyle(string talentId)
    {
        var talent = allTalents.Find(t => t.id == talentId);
        return talent?.effects?.career ?? "";
    }

    public Talent GetTalentById(string id)
    {
        return allTalents?.Find(t => t.id == id);
    }

    public List<Talent> GetAllTalents()
    {
        return allTalents ?? new List<Talent>();
    }

    public void ResetPityCounter()
    {
        pityCounter = 0;
    }

    public int GetPityCounter()
    {
        return pityCounter;
    }

    public List<Talent> GetPlayerTalents(PlayerState state)
    {
        if (state == null || state.unlockedTalents == null || allTalents == null)
            return new List<Talent>();

        return allTalents.Where(t => state.unlockedTalents.Contains(t.id)).ToList();
    }

    public int GetTalentSubjectCompatibility(string talentId, List<string> playerSubjects)
    {
        if (playerSubjects == null || playerSubjects.Count == 0) return 0;

        var talent = allTalents.Find(t => t.id == talentId);
        if (talent?.effects?.major == null) return 0;

        int score = 0;
        var supportedCategories = talent.effects.major.boost ?? new string[0];

        foreach (var subject in playerSubjects)
        {
            switch (subject)
            {
                case "物理":
                    if (supportedCategories.Any(c => c == "工学" || c == "理学"))
                        score += 15;
                    break;
                case "化学":
                    if (supportedCategories.Any(c => c == "医学" || c == "工学"))
                        score += 15;
                    break;
                case "生物":
                    if (supportedCategories.Any(c => c == "医学" || c == "农学"))
                        score += 15;
                    break;
                case "历史":
                    if (supportedCategories.Any(c => c == "历史学" || c == "文学" || c == "哲学"))
                        score += 15;
                    break;
                case "政治":
                    if (supportedCategories.Any(c => c == "法学" || c == "管理学" || c == "经济学"))
                        score += 15;
                    break;
                case "地理":
                    if (supportedCategories.Any(c => c == "教育学"))
                        score += 10;
                    break;
            }
        }

        return Mathf.Clamp(score, 0, 45);
    }

    public List<Talent> DrawTalentsForPlayer(PlayerState state)
    {
        if (allTalents == null || allTalents.Count == 0)
        {
            Debug.LogWarning("[TalentEngine] 天赋池为空");
            return new List<Talent>();
        }

        var pool = allTalents.Where(t => t.rarity != "legendary").ToList();
        var result = new List<Talent>();
        var playerSubjects = state?.selectedSubjects ?? new List<string>();

        bool hasRareOrBetter = false;

        for (int i = 0; i < 3 && pool.Count > 0; i++)
        {
            Talent picked = null;

            if (pityCounter >= PITY_EPIC)
            {
                var epics = pool.Where(t => t.rarity == "epic").ToList();
                if (epics.Count > 0)
                {
                    picked = epics[Random.Range(0, epics.Count)];
                    Debug.Log($"[TalentEngine] 保底触发! 必出史诗");
                }
            }
            else if (pityCounter >= PITY_RARE)
            {
                var rares = pool.Where(t => t.rarity == "rare" || t.rarity == "epic").ToList();
                if (rares.Count > 0)
                {
                    picked = rares[Random.Range(0, rares.Count)];
                    Debug.Log($"[TalentEngine] 保底触发! 必出稀有");
                }
            }

            if (picked == null)
            {
                var weighted = pool.Select(t => new
                {
                    Talent = t,
                    Weight = GetRarityWeight(t.rarity) + GetTalentSubjectCompatibility(t.id, playerSubjects)
                }).ToList();

                int totalWeight = weighted.Sum(w => w.Weight);
                if (totalWeight <= 0)
                {
                    picked = pool[Random.Range(0, pool.Count)];
                }
                else
                {
                    int rand = Random.Range(0, totalWeight);
                    int cumulative = 0;
                    foreach (var w in weighted)
                    {
                        cumulative += w.Weight;
                        if (rand < cumulative)
                        {
                            picked = w.Talent;
                            break;
                        }
                    }
                    if (picked == null) picked = weighted[^1].Talent;
                }
            }

            result.Add(picked);
            pool.Remove(picked);

            if (picked.rarity == "epic" || picked.rarity == "rare")
                hasRareOrBetter = true;
        }

        if (hasRareOrBetter)
            pityCounter = 0;
        else
            pityCounter++;

        Debug.Log($"[TalentEngine] 定向抽卡(选科:{string.Join(",", playerSubjects)}) 保底计数:{pityCounter}: {string.Join(", ", result.Select(t => t.name))}");
        return result;
    }

    public string GetSubjectRecommendations(string talentId)
    {
        var talent = allTalents.Find(t => t.id == talentId);
        if (talent?.effects?.major?.boost == null) return "";

        var categories = talent.effects.major.boost;
        var subjects = new List<string>();

        foreach (var cat in categories)
        {
            switch (cat)
            {
                case "工学": subjects.Add("物理"); subjects.Add("化学"); break;
                case "理学": subjects.Add("物理"); break;
                case "医学": subjects.Add("化学"); subjects.Add("生物"); break;
                case "农学": subjects.Add("生物"); break;
                case "哲学": subjects.Add("历史"); subjects.Add("政治"); break;
                case "法学": subjects.Add("政治"); break;
                case "经济学": subjects.Add("政治"); break;
                case "管理学": subjects.Add("政治"); break;
                case "文学": subjects.Add("历史"); break;
                case "历史学": subjects.Add("历史"); break;
                case "教育学": subjects.Add("历史"); subjects.Add("物理"); break;
                case "艺术学": subjects.Add("历史"); break;
            }
        }

        var unique = subjects.Distinct().ToList();
        return unique.Count > 0 ? string.Join("、", unique) : "";
    }
}