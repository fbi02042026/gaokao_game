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

        for (int i = 0; i < 3 && pool.Count > 0; i++)
        {
            var weighted = pool.Select(t => new { Talent = t, Weight = GetRarityWeight(t.rarity) }).ToList();
            int totalWeight = weighted.Sum(w => w.Weight);
            int rand = Random.Range(0, totalWeight);
            int cumulative = 0;
            foreach (var w in weighted)
            {
                cumulative += w.Weight;
                if (rand < cumulative)
                {
                    result.Add(w.Talent);
                    pool.Remove(w.Talent);
                    break;
                }
            }
        }

        Debug.Log($"[TalentEngine] 抽卡结果: {string.Join(", ", result.Select(t => t.name))}");
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

    public List<Talent> GetPlayerTalents(PlayerState state)
    {
        if (state == null || state.unlockedTalents == null || allTalents == null)
            return new List<Talent>();

        return allTalents.Where(t => state.unlockedTalents.Contains(t.id)).ToList();
    }
}