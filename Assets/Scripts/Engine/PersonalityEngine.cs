using UnityEngine;
using System.Collections.Generic;

public class PersonalityResult
{
    public Personality personality;
    public int rational;
    public int impulsive;
    public int adventurous;
}

public class PersonalityEngine : MonoBehaviour
{
    private static readonly (Personality personality, Dictionary<string, float> weights)[] PERSONALITIES = {
        (new Personality { id = "rational", name = "理性探索者", icon = "🔬",
            tagline = "用数据说话，用选择定义人生",
            description = "你倾向于分析利弊，用理性做决策" },
            new Dictionary<string, float> { {"intellect", 3f}, {"mental", 1f} }),

        (new Personality { id = "creator", name = "自由创作者", icon = "🎨",
            tagline = "不走寻常路，活出自己的颜色",
            description = "你追求自由和创意，不愿被条条框框束缚" },
            new Dictionary<string, float> { {"mental", 2f}, {"social", 2f} }),

        (new Personality { id = "pragmatist", name = "实用主义者", icon = "💼",
            tagline = "理想很美，但我选靠谱",
            description = "你务实稳重，优先考虑就业和稳定" },
            new Dictionary<string, float> { {"intellect", 1f}, {"health", 2f} }),

        (new Personality { id = "fighter", name = "逆袭斗士", icon = "🔥",
            tagline = "人生没有定局，我来翻盘",
            description = "你不服输，总想挑战更高的目标" },
            new Dictionary<string, float> { {"mental", 3f}, {"health", 1f} }),

        (new Personality { id = "zen", name = "佛系随缘", icon = "🌊",
            tagline = "该来的总会来，急什么",
            description = "你心态超好，不焦虑不内卷" },
            new Dictionary<string, float> { {"health", 3f}, {"mental", 1f} }),

        (new Personality { id = "emotional", name = "情感驱动", icon = "❤️",
            tagline = "人心比分数重要",
            description = "你重视人和情感，选择时更多考虑身边人的感受" },
            new Dictionary<string, float> { {"social", 3f}, {"mental", 1f} }),

        (new Personality { id = "steady", name = "稳扎稳打", icon = "🏔️",
            tagline = "不求最高，但求最稳",
            description = "你谨慎求稳，绝不冒险，保底永远要有" },
            new Dictionary<string, float> { {"health", 2f}, {"intellect", 2f} }),

        (new Personality { id = "idealist", name = "理想主义者", icon = "🌟",
            tagline = "为热爱而活，不将就",
            description = "你追随内心，即使专业冷门也要选自己喜欢的" },
            new Dictionary<string, float> { {"mental", 2f}, {"social", 1f}, {"intellect", 1f} })
    };

    public Personality CalculatePersonality(PlayerState state)
    {
        float bestScore = float.MinValue;
        Personality best = PERSONALITIES[0].personality;

        foreach (var p in PERSONALITIES)
        {
            float score = 0;
            if (p.weights.ContainsKey("intellect")) score += state.intellect * p.weights["intellect"];
            if (p.weights.ContainsKey("mental")) score += state.mental * p.weights["mental"];
            if (p.weights.ContainsKey("social")) score += state.social * p.weights["social"];
            if (p.weights.ContainsKey("health")) score += state.health * p.weights["health"];

            if (score > bestScore) { bestScore = score; best = p.personality; }
        }

        Debug.Log($"[PersonalityEngine] 计算结果: {best.name} (score={bestScore:F0})");
        return best;
    }

    public PersonalityResult GetPersonalityScores(PlayerState state)
    {
        var personality = CalculatePersonality(state);
        int total = state.intellect + state.mental + state.social + state.health;
        if (total == 0) total = 1;

        var result = new PersonalityResult
        {
            personality = personality,
            rational = Mathf.RoundToInt((float)state.intellect / total * 100),
            impulsive = Mathf.RoundToInt((float)state.mental / total * 100),
            adventurous = Mathf.Clamp(Mathf.RoundToInt((float)(state.mental + state.social - state.health * 2) / total * 100 + 50), 0, 100)
        };

        Debug.Log($"[PersonalityEngine] 理性={result.rational} 冲动={result.impulsive} 冒险={result.adventurous}");
        return result;
    }

    public Personality GetPersonalityById(string id)
    {
        foreach (var p in PERSONALITIES)
            if (p.personality.id == id) return p.personality;
        return null;
    }

    public List<Personality> GetAllPersonalities()
    {
        var list = new List<Personality>();
        foreach (var p in PERSONALITIES) list.Add(p.personality);
        return list;
    }
}