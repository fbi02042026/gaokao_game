using UnityEngine;

public class ScoreEngine : MonoBehaviour
{
    public static ScoreEngine Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    public int CalculateScore(PlayerState state, string talentId)
    {
        float baseScore = 400 + state.intellect * 2.5f + Random.Range(-30f, 31f);
        float mentalBonus = state.mental * 0.3f;
        float healthBonus = state.health * 0.2f;
        float total = baseScore + mentalBonus + healthBonus;

        int rawScore = Mathf.RoundToInt(total);

        var talentEngine = TalentEngine.Instance;
        if (talentEngine != null)
            rawScore = talentEngine.ApplyGaokaoModifiers(talentId, rawScore);

        return Mathf.Clamp(rawScore, 200, 750);
    }

    public SubjectScores SplitSubjects(int totalScore)
    {
        float chinese = Random.Range(90, 121);
        float mathRatio = Random.Range(0.5f, 1f);
        float englishRatio = Random.Range(0.5f, 1f);
        float comprehensive = totalScore - chinese - mathRatio * 150 - englishRatio * 150;

        return new SubjectScores
        {
            chinese = Mathf.RoundToInt(chinese),
            math = Mathf.RoundToInt(mathRatio * 150),
            english = Mathf.RoundToInt(englishRatio * 150),
            comprehensive = Mathf.RoundToInt(comprehensive)
        };
    }

    public int CalculateProbability(int score, int minScore, int avgScore)
    {
        if (avgScore <= minScore) return 50;

        float prob = (float)(score - minScore) / (avgScore - minScore) * 100f;
        return Mathf.Clamp(Mathf.RoundToInt(prob), 5, 95);
    }

    public string GetProbabilityLabel(int probability)
    {
        if (probability >= 80) return "稳";
        if (probability >= 60) return "保";
        if (probability >= 30) return "冲";
        return "垫";
    }
}

[System.Serializable]
public class SubjectScores
{
    public int chinese;
    public int math;
    public int english;
    public int comprehensive;

    public int GetTotal()
    {
        return chinese + math + english + comprehensive;
    }
}